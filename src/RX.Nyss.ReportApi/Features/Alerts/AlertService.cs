﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using RX.Nyss.Common;
using RX.Nyss.Data;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Data.Models;
using RX.Nyss.ReportApi.Configuration;
using RX.Nyss.ReportApi.Services;
using RX.Nyss.ReportApi.Utils.Logging;

namespace RX.Nyss.ReportApi.Features.Alerts
{
    public interface IAlertService
    {
        Task<Alert> ReportAdded(Report report);
        Task ReportDismissed(int reportId);
        Task SendNotificationsForNewAlert(Alert alert, GatewaySetting gatewaySetting);
    }

    public class AlertService: IAlertService
    {
        private readonly INyssContext _nyssContext;
        private readonly IReportLabelingService _reportLabelingService;
        private readonly ILoggerAdapter _loggerAdapter;
        private readonly IEmailToSmsPublisherService _emailToSmsPublisherService;
        private readonly IConfig _config;


        public AlertService(INyssContext nyssContext, IReportLabelingService reportLabelingService, ILoggerAdapter loggerAdapter, IEmailToSmsPublisherService emailToSmsPublisherService, IConfig config)
        {
            _nyssContext = nyssContext;
            _reportLabelingService = reportLabelingService;
            _loggerAdapter = loggerAdapter;
            _emailToSmsPublisherService = emailToSmsPublisherService;
            _config = config;
        }

        public async Task<Alert> ReportAdded(Report report)
        {
            if (report.DataCollector.DataCollectorType != DataCollectorType.Human
                || (report.ReportType != ReportType.Single && report.ReportType != ReportType.NonHuman)
                || report.IsTraining)
            {
                return null;
            }

            var projectHealthRisk = await _nyssContext.ProjectHealthRisks
                .Where(phr => phr == report.ProjectHealthRisk)
                .Include(phr => phr.AlertRule)
                .Include(phr => phr.HealthRisk)
                .SingleAsync();

            if (projectHealthRisk.HealthRisk.HealthRiskType == HealthRiskType.Activity)
            {
                return null;
            }

            await _reportLabelingService.ResolveLabelsOnReportAdded(report, projectHealthRisk);
            await _nyssContext.SaveChangesAsync();

            var triggeredAlert = await HandleAlerts(report);
            await _nyssContext.SaveChangesAsync();
            return triggeredAlert;
        }

        public async Task ReportDismissed(int reportId)
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var inspectedAlert = await _nyssContext.AlertReports
                .Where(ar => ar.ReportId == reportId)
                .Where(ar => ar.Alert.Status == AlertStatus.Pending || ar.Alert.Status == AlertStatus.Dismissed)
                .Select(ar => ar.Alert)
                .SingleOrDefaultAsync();

            if (inspectedAlert == null)
            {
                _loggerAdapter.Warn($"The alert for report with id {reportId} does not exist or has status different than pending or dismissed.");
                return;
            }

            var report = await _nyssContext.Reports
                .Include(r => r.ProjectHealthRisk)
                .ThenInclude(phr => phr.AlertRule)
                .Where(r => r.Id == reportId)
                .SingleAsync();

            if (report == null)
            {
                _loggerAdapter.Warn($"The report with id {reportId} does not exist.");
                return;
            }

            if (report.Status == ReportStatus.Pending)
            {
                report.Status = ReportStatus.Rejected;
            }

            var alertRule = report.ProjectHealthRisk.AlertRule;

            await RecalculateAlert(report, alertRule);
            await RejectAlertWhenRequirementsAreNotMet(reportId, alertRule, inspectedAlert);

            await _nyssContext.SaveChangesAsync();
            transactionScope.Complete();
        }

        public async Task SendNotificationsForNewAlert(Alert alert, GatewaySetting gatewaySetting)
        {
            var phoneNumbersOfSupervisorsInAlert = await _nyssContext.AlertReports
                    .Where(ar => ar.Alert.Id == alert.Id)
                    .Select(ar => ar.Report.DataCollector.Supervisor.PhoneNumber)
                    .Distinct()
                    .ToListAsync();

            var message = await CreateNotificationMessageForNewAlert(alert);

            await _emailToSmsPublisherService.SendMessages(gatewaySetting.EmailAddress, gatewaySetting.Name, phoneNumbersOfSupervisorsInAlert, message);
        }

        private async Task<Alert> HandleAlerts(Report report)
        {
            var reportGroupLabel = report.ReportGroupLabel;
            var projectHealthRisk = report.ProjectHealthRisk;

            var existingAlert = await IncludeAllReportsWithLabelInExistingAlert(reportGroupLabel);

            if (existingAlert != null)
            {
                return null;
            }

            var reportsWithLabel = await _nyssContext.Reports
                .Where(r => r.ReportGroupLabel == reportGroupLabel)
                .Where(r => !r.ReportAlerts.Any(ra => ra.Alert.Status == AlertStatus.Closed))
                .Where(r => StatusConstants.ReportStatusesConsideredForAlertProcessing.Contains(r.Status))
                .Where(r=> !r.IsTraining)
                .Where(r => !r.MarkedAsError)
                .ToListAsync();

            if (projectHealthRisk.AlertRule.CountThreshold == 0 || reportsWithLabel.Count < projectHealthRisk.AlertRule.CountThreshold)
            {
                return null;
            }

            var alert = await CreateNewAlert(projectHealthRisk);
            await AddReportsToAlert(alert, reportsWithLabel);
            return alert;
        }

        private async Task<Alert> IncludeAllReportsWithLabelInExistingAlert(Guid reportGroupLabel, int? alertIdToIgnore = null)
        {
            var existingActiveAlertForLabel = await _nyssContext.Reports
                .Where(r => StatusConstants.ReportStatusesConsideredForAlertProcessing.Contains(r.Status))
                .Where(r => !r.MarkedAsError)
                .Where(r => !r.IsTraining)
                .Where(r => r.ReportGroupLabel == reportGroupLabel)
                .SelectMany(r => r.ReportAlerts)
                .Where(ar => !alertIdToIgnore.HasValue || ar.AlertId != alertIdToIgnore.Value)
                .Select(ra => ra.Alert)
                .OrderByDescending(a => a.Status == AlertStatus.Escalated)
                .FirstOrDefaultAsync(a => a.Status == AlertStatus.Pending || a.Status == AlertStatus.Escalated);

            if (existingActiveAlertForLabel != null)
            {
                var reportsInLabelWithNoActiveAlert = await _nyssContext.Reports
                    .Where(r => StatusConstants.ReportStatusesConsideredForAlertProcessing.Contains(r.Status))
                    .Where(r => !r.MarkedAsError)
                    .Where(r => !r.IsTraining)
                    .Where(r => r.ReportGroupLabel == reportGroupLabel)
                    .Where(r => !r.ReportAlerts.Any(ra => ra.Alert.Status == AlertStatus.Pending || ra.Alert.Status == AlertStatus.Escalated || ra.Alert.Status == AlertStatus.Closed)
                              || r.ReportAlerts.Any(ra => ra.AlertId == alertIdToIgnore) )
                    .ToListAsync();

                await AddReportsToAlert(existingActiveAlertForLabel, reportsInLabelWithNoActiveAlert);
            }

            return existingActiveAlertForLabel;
        }

        private async Task<Alert> CreateNewAlert(ProjectHealthRisk projectHealthRisk)
        {
            var newAlert = new Alert
            {
                ProjectHealthRisk = projectHealthRisk,
                CreatedAt = DateTime.UtcNow,
                Status = AlertStatus.Pending
            };
            await _nyssContext.Alerts.AddAsync(newAlert);
            return newAlert;
        }

        private Task AddReportsToAlert(Alert alert, List<Report> reports)
        {
            reports.Where(r => r.Status == ReportStatus.New).ToList()
                .ForEach(r => r.Status = ReportStatus.Pending);

            var alertReports = reports.Select(r => new AlertReport { Report = r, Alert = alert });
            return _nyssContext.AlertReports.AddRangeAsync(alertReports);
        }

        private async Task RecalculateAlert(Report report, AlertRule alertRule)
        {
            var pointsWithLabels = await _reportLabelingService.CalculateNewLabelsInLabelGroup(report.ReportGroupLabel, alertRule.KilometersThreshold.Value * 1000 * 2, report.Id);
            await _reportLabelingService.UpdateLabelsInDatabaseDirect(pointsWithLabels);
        }

        private async Task RejectAlertWhenRequirementsAreNotMet(int reportId, AlertRule alertRule, Alert inspectedAlert)
        {
            var alertReportsWitUpdatedLabels = await _nyssContext.AlertReports
                .Where(ar => ar.AlertId == inspectedAlert.Id)
                .Where(ar => ar.ReportId != reportId)
                .Select(ar => ar.Report)
                .ToListAsync();

            var updatedReportsGroupedByLabel = alertReportsWitUpdatedLabels.GroupBy(r => r.ReportGroupLabel).ToList();

            var noGroupWithinAlertSatisfiesCountThreshold = !updatedReportsGroupedByLabel.Any(g => g.Count() >= alertRule.CountThreshold);
            if (noGroupWithinAlertSatisfiesCountThreshold)
            {
                if (inspectedAlert.Status != AlertStatus.Dismissed)
                {
                    inspectedAlert.Status = AlertStatus.Rejected;
                }

                foreach (var groupNotSatisfyingThreshold in updatedReportsGroupedByLabel)
                {
                    var reportGroupLabel = groupNotSatisfyingThreshold.Key;
                    await IncludeAllReportsWithLabelInExistingAlert(reportGroupLabel, inspectedAlert.Id);
                }
            }
        }

        private async Task<string> CreateNotificationMessageForNewAlert(Alert alert)
        {
            var alertData = await _nyssContext.Alerts.Where(a => a.Id == alert.Id)
                .Select(a => new
                {
                    ProjectId = a.ProjectHealthRisk.Project.Id,
                    HealthRiskName = a.ProjectHealthRisk.HealthRisk.LanguageContents
                        .Where(lc => lc.ContentLanguage == a.ProjectHealthRisk.Project.NationalSociety.ContentLanguage)
                        .Select(lc => lc.Name)
                        .FirstOrDefault(),

                    a.ProjectHealthRisk.Project.NationalSociety.ContentLanguage.LanguageCode,

                    VillageOfLastReport = a.AlertReports.OrderByDescending(ar => ar.Report.ReceivedAt)
                        .Select(ar => ar.Report.Village.Name)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();


            //ToDo: get this from blob storage instead of hardcoded text
            var smsTemplateEn = "An alert for {{healthRisk/event}} has been triggered by one of your DC. Last report sent from {{village}}. Please check alert list and cross-check alert: {{linkToAlert}}";
            var smsTemplateFr = "Une alerte pour {{healthRisk/event}} a été déclenchée par l'un de vos DC. Dernier rapport envoyé par {{village}}. Veuillez vérifier la liste des alertes et recouper l'alerte: {{linkToAlert}}";
            var smsTemplate = alertData.LanguageCode.ToLower() == "en"
                ? smsTemplateEn
                : smsTemplateFr;

            var baseUrl = new Uri(_config.BaseUrl);
            var relativeUrl = $"projects/{alertData.ProjectId}/alerts/{alert.Id}/assess";
            var linkToAlert = new Uri(baseUrl, relativeUrl);

            smsTemplate = smsTemplate.Replace("{{healthRisk/event}}", alertData.HealthRiskName);
            smsTemplate = smsTemplate.Replace("{{village}}", alertData.VillageOfLastReport);
            smsTemplate = smsTemplate.Replace("{{linkToAlert}}", linkToAlert.ToString());

            return smsTemplate;
        }
    }
}
