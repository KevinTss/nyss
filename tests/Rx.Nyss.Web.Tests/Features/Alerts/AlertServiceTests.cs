﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockQueryable.NSubstitute;
using NSubstitute;
using RX.Nyss.Common.Extensions;
using RX.Nyss.Common.Utils;
using RX.Nyss.Common.Utils.DataContract;
using RX.Nyss.Common.Utils.Logging;
using RX.Nyss.Data;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Data.Models;
using RX.Nyss.Web.Configuration;
using RX.Nyss.Web.Features.Alerts;
using RX.Nyss.Web.Features.Alerts.Dto;
using RX.Nyss.Web.Services;
using RX.Nyss.Web.Services.Authorization;
using Shouldly;
using Xunit;

namespace RX.Nyss.Web.Tests.Features.Alerts
{
    public class AlertServiceTests
    {
        private readonly INyssContext _nyssContext;
        private readonly IEmailPublisherService _emailPublisherService;
        private readonly ISmsTextGeneratorService _smsTextGeneratorService;
        private readonly AlertService _alertService;
        private readonly List<Alert> _alerts;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IAuthorizationService _authorizationService;
        private readonly DateTime _now = DateTime.UtcNow;
        private readonly User _currentUser = new GlobalCoordinatorUser();

        public AlertServiceTests()
        {
            _nyssContext = Substitute.For<INyssContext>();
            _emailPublisherService = Substitute.For<IEmailPublisherService>();
            var emailTextGeneratorService = Substitute.For<IEmailTextGeneratorService>();
            _smsTextGeneratorService = Substitute.For<ISmsTextGeneratorService>();
            var config = Substitute.For<INyssWebConfig>();
            var loggerAdapter = Substitute.For<ILoggerAdapter>();

            _dateTimeProvider = Substitute.For<IDateTimeProvider>();
            _authorizationService = Substitute.For<IAuthorizationService>();
            _alertService = new AlertService(_nyssContext, _emailPublisherService, emailTextGeneratorService, config, _smsTextGeneratorService, loggerAdapter, _dateTimeProvider,
                _authorizationService);

            _alerts = TestData.GetAlerts();
            var alertsDbSet = _alerts.AsQueryable().BuildMockDbSet();
            _nyssContext.Alerts.Returns(alertsDbSet);

            var gatewaySettings = TestData.GetGatewaySettings();
            var gatewaySettingsDbSet = gatewaySettings.AsQueryable().BuildMockDbSet();
            _nyssContext.GatewaySettings.Returns(gatewaySettingsDbSet);

            emailTextGeneratorService.GenerateEscalatedAlertEmail(TestData.ContentLanguageCode)
                .Returns((TestData.EscalationEmailSubject, TestData.EscalationEmailBody));

            _dateTimeProvider.UtcNow.Returns(_now);
            _authorizationService.GetCurrentUser().Returns(_currentUser);
        }

        [Theory]
        [InlineData(AlertStatus.Closed)]
        [InlineData(AlertStatus.Dismissed)]
        [InlineData(AlertStatus.Escalated)]
        [InlineData(AlertStatus.Rejected)]
        public async Task EscalateAlert_WhenAlertIsNotPending_ShouldReturnError(AlertStatus status)
        {
            _alerts.First().Status = status;

            var result = await _alertService.Escalate(TestData.AlertId);

            result.IsSuccess.ShouldBeFalse();
            result.Message.Key.ShouldBe(ResultKey.Alert.EscalateAlert.WrongStatus);
        }

        [Fact]
        public async Task EscalateAlert_WhenAlertAcceptedReportCountIsLowerThanThreshold_ShouldReturnError()
        {
            _alerts.First().Status = AlertStatus.Pending;

            _alerts.First().AlertReports = new List<AlertReport>
            {
                new AlertReport
                {
                    Report = new Report
                    {
                        Status = ReportStatus.Accepted,
                        RawReport = new RawReport
                        {
                            ApiKey = TestData.ApiKey,
                            Village = new Village()
                        }
                    }
                },
                new AlertReport
                {
                    Report = new Report
                    {
                        Status = ReportStatus.Accepted,
                        RawReport = new RawReport
                        {
                            ApiKey = TestData.ApiKey,
                            Village = new Village()
                        }
                    }
                },
                new AlertReport
                {
                    Report = new Report
                    {
                        Status = ReportStatus.Rejected,
                        RawReport = new RawReport
                        {
                            ApiKey = TestData.ApiKey,
                            Village = new Village()
                        }
                    }
                }
            };

            _alerts.First().ProjectHealthRisk.AlertRule.CountThreshold = 3;

            var result = await _alertService.Escalate(TestData.AlertId);

            result.IsSuccess.ShouldBeFalse();
            result.Message.Key.ShouldBe(ResultKey.Alert.EscalateAlert.ThresholdNotReached);
        }

        [Fact]
        public async Task EscalateAlert_WhenAlertMeetsTheCriteria_ShouldSendEmailAndSmsMessages()
        {
            var emailAddress = "test@test.com";
            var phonenumber = "+1234578";
            var smsText = "hey";

            _smsTextGeneratorService.GenerateEscalatedAlertSms(TestData.ContentLanguageCode).Returns(smsText);

            _alerts.First().Status = AlertStatus.Pending;

            _alerts.First().AlertReports = new List<AlertReport>
            {
                new AlertReport
                {
                    Report = new Report
                    {
                        Status = ReportStatus.Accepted,
                        RawReport = new RawReport
                        {
                            ApiKey = TestData.ApiKey,
                            Village = new Village()
                        }
                    }
                }
            };

            _alerts.First().ProjectHealthRisk.AlertRule.CountThreshold = 1;
            _alerts.First().ProjectHealthRisk.Project.EmailAlertRecipients = new List<EmailAlertRecipient> { new EmailAlertRecipient { EmailAddress = emailAddress } };
            _alerts.First().ProjectHealthRisk.Project.SmsAlertRecipients = new List<SmsAlertRecipient> { new SmsAlertRecipient { PhoneNumber = phonenumber } };


            await _alertService.Escalate(TestData.AlertId);

            await _emailPublisherService.ReceivedWithAnyArgs(2).SendEmail((null, null), null, null);

            await _emailPublisherService.Received(1)
                .SendEmail((emailAddress, emailAddress), TestData.EscalationEmailSubject, TestData.EscalationEmailBody);

            await _emailPublisherService.Received(1)
                .SendEmail((TestData.GatewayEmail, TestData.GatewayEmail), phonenumber, smsText, true);
        }

        [Fact]
        public async Task EscalateAlert_WhenLastReportGatewayNotExists_ShouldSendSmsThroughFirstInSociety()
        {
            var phonenumber = "+1234578";
            var smsText = "hey";
            _smsTextGeneratorService.GenerateEscalatedAlertSms(TestData.ContentLanguageCode).Returns(smsText);

            _alerts.First().Status = AlertStatus.Pending;

            _alerts.First().AlertReports = new List<AlertReport>
            {
                new AlertReport
                {
                    Report = new Report
                    {
                        Status = ReportStatus.Accepted,
                        RawReport = new RawReport
                        {
                            ApiKey = "Some_missing_key",
                            Village = new Village()
                        }
                    }
                }
            };

            _alerts.First().ProjectHealthRisk.AlertRule.CountThreshold = 1;
            _alerts.First().ProjectHealthRisk.Project.SmsAlertRecipients = new List<SmsAlertRecipient> { new SmsAlertRecipient { PhoneNumber = phonenumber } };

            await _alertService.Escalate(TestData.AlertId);

            await _emailPublisherService.Received(1)
                .SendEmail((TestData.GatewayEmail, TestData.GatewayEmail), phonenumber, smsText, true);
        }

        [Fact]
        public async Task EscalateAlert_WhenAlertMeetsTheCriteria_ChangesAlertStatus()
        {
            _alerts.First().Status = AlertStatus.Pending;

            _alerts.First().AlertReports = new List<AlertReport>
            {
                new AlertReport
                {
                    Report = new Report
                    {
                        Status = ReportStatus.Accepted,
                        RawReport = new RawReport
                        {
                            ApiKey = TestData.ApiKey,
                            Village = new Village()
                        }
                    }
                }
            };

            _alerts.First().ProjectHealthRisk.AlertRule.CountThreshold = 1;

            var result = await _alertService.Escalate(TestData.AlertId);

            _alerts.First().Status.ShouldBe(AlertStatus.Escalated);
            _alerts.First().EscalatedAt.ShouldBe(_now);
            _alerts.First().EscalatedBy.ShouldBe(_currentUser);
            await _nyssContext.Received(1).SaveChangesAsync();
            result.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public async Task DismissAlert_WhenAlertIsClosed_ShouldReturnError()
        {
            _alerts.First().Status = AlertStatus.Closed;

            var result = await _alertService.Dismiss(TestData.AlertId);

            result.IsSuccess.ShouldBeFalse();
            result.Message.Key.ShouldBe(ResultKey.Alert.DismissAlert.WrongStatus);
        }

        [Fact]
        public async Task DismissAlert_WhenAlertAcceptedReportAndPendingReportCountIsGreaterOrEqualToThreshold_ShouldReturnError()
        {
            _alerts.First().Status = AlertStatus.Pending;

            _alerts.First().AlertReports = new List<AlertReport>
            {
                new AlertReport
                {
                    Report = new Report
                    {
                        Status = ReportStatus.Accepted,
                        RawReport = new RawReport { Village = new Village() }
                    }
                },
                new AlertReport
                {
                    Report = new Report
                    {
                        Status = ReportStatus.Accepted,
                        RawReport = new RawReport { Village = new Village() }
                    }
                },
                new AlertReport
                {
                    Report = new Report
                    {
                        Status = ReportStatus.Pending,
                        RawReport = new RawReport { Village = new Village() }
                    }
                },
                new AlertReport
                {
                    Report = new Report
                    {
                        Status = ReportStatus.Rejected,
                        RawReport = new RawReport { Village = new Village() }
                    }
                },
                new AlertReport
                {
                    Report = new Report
                    {
                        Status = ReportStatus.Rejected,
                        RawReport = new RawReport { Village = new Village() }
                    }
                },
                new AlertReport
                {
                    Report = new Report
                    {
                        Status = ReportStatus.Rejected,
                        RawReport = new RawReport { Village = new Village() }
                    }
                },
                new AlertReport
                {
                    Report = new Report
                    {
                        Status = ReportStatus.Rejected,
                        RawReport = new RawReport { Village = new Village() }
                    }
                }
            };

            _alerts.First().ProjectHealthRisk.AlertRule.CountThreshold = 3;

            var result = await _alertService.Dismiss(TestData.AlertId);

            result.IsSuccess.ShouldBeFalse();
            result.Message.Key.ShouldBe(ResultKey.Alert.DismissAlert.PossibleEscalation);
        }

        [Fact]
        public async Task DismissAlert_WhenAlertMeetsTheCriteria_ChangesAlertStatus()
        {
            _alerts.First().Status = AlertStatus.Pending;

            _alerts.First().AlertReports = new List<AlertReport>
            {
                new AlertReport
                {
                    Report = new Report
                    {
                        Status = ReportStatus.Rejected,
                        RawReport = new RawReport { Village = new Village() }
                    }
                }
            };

            _alerts.First().ProjectHealthRisk.AlertRule.CountThreshold = 1;

            var result = await _alertService.Dismiss(TestData.AlertId);

            _alerts.First().Status.ShouldBe(AlertStatus.Dismissed);
            _alerts.First().DismissedAt.ShouldBe(_now);
            _alerts.First().DismissedBy.ShouldBe(_currentUser);
            await _nyssContext.Received(1).SaveChangesAsync();
            result.IsSuccess.ShouldBeTrue();
        }

        [Theory]
        [InlineData(AlertStatus.Closed)]
        [InlineData(AlertStatus.Rejected)]
        [InlineData(AlertStatus.Dismissed)]
        [InlineData(AlertStatus.Pending)]
        public async Task CloseAlert_WhenAlertIsNotPending_ShouldReturnError(AlertStatus status)
        {
            _alerts.First().Status = status;

            var result = await _alertService.Close(TestData.AlertId, "");

            result.IsSuccess.ShouldBeFalse();
            result.Message.Key.ShouldBe(ResultKey.Alert.CloseAlert.WrongStatus);
        }

        [Fact]
        public async Task CloseAlert_WhenAlertMeetsTheCriteria_ChangesAlertStatus()
        {
            var comments = "abc";

            _alerts.First().Status = AlertStatus.Escalated;

            var result = await _alertService.Close(TestData.AlertId, comments);

            _alerts.First().Status.ShouldBe(AlertStatus.Closed);
            _alerts.First().ClosedAt.ShouldBe(_now);
            _alerts.First().ClosedBy.ShouldBe(_currentUser);
            _alerts.First().Comments.ShouldBe(comments);
            await _nyssContext.Received(1).SaveChangesAsync();
            result.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public async Task GetAlertLogs_ShouldMapAlertCreation()
        {
            var result = await _alertService.GetLogs(TestData.AlertId);

            result.IsSuccess.ShouldBeTrue();
            result.Value.Items.ElementAt(0).Date.ShouldBe(TestData.AlertCreatedAt.ApplyTimeZone(TestData.TimeZone));
            result.Value.Items.ElementAt(0).LogType.ShouldBe(AlertLogResponseDto.LogType.TriggeredAlert);
            result.Value.Items.ElementAt(0).UserName.ShouldBe(null);
        }

        [Fact]
        public async Task GetAlertLogs_ShouldMapReportAcceptance()
        {
            var date = new DateTime(2020, 1, 5);
            var userName = "User";
            var user = new GlobalCoordinatorUser { Name = userName };

            _alerts.First().AlertReports.First().Report.Status = ReportStatus.Accepted;
            _alerts.First().AlertReports.First().Report.AcceptedAt = date;
            _alerts.First().AlertReports.First().Report.AcceptedBy = user;

            var result = await _alertService.GetLogs(TestData.AlertId);

            result.IsSuccess.ShouldBeTrue();
            result.Value.Items.ElementAt(1).Date.ShouldBe(date.ApplyTimeZone(TestData.TimeZone));
            result.Value.Items.ElementAt(1).LogType.ShouldBe(AlertLogResponseDto.LogType.AcceptedReport);
            result.Value.Items.ElementAt(1).UserName.ShouldBe(userName);
        }

        [Fact]
        public async Task GetAlertLogs_ShouldMapReportDismissal()
        {
            var date = new DateTime(2020, 1, 5);
            var userName = "User";
            var user = new GlobalCoordinatorUser { Name = userName };

            _alerts.First().AlertReports.First().Report.Status = ReportStatus.Rejected;
            _alerts.First().AlertReports.First().Report.RejectedAt = date;
            _alerts.First().AlertReports.First().Report.RejectedBy = user;

            var result = await _alertService.GetLogs(TestData.AlertId);

            result.IsSuccess.ShouldBeTrue();
            result.Value.Items.ElementAt(1).Date.ShouldBe(date.ApplyTimeZone(TestData.TimeZone));
            result.Value.Items.ElementAt(1).LogType.ShouldBe(AlertLogResponseDto.LogType.RejectedReport);
            result.Value.Items.ElementAt(1).UserName.ShouldBe(userName);
        }

        [Fact]
        public async Task GetAlertLogs_ShouldMapAlertEscalation()
        {
            var date = new DateTime(2020, 1, 5);
            var userName = "User";
            var user = new GlobalCoordinatorUser { Name = userName };

            _alerts.First().Status = AlertStatus.Escalated;
            _alerts.First().EscalatedAt = date;
            _alerts.First().EscalatedBy = user;

            var result = await _alertService.GetLogs(TestData.AlertId);

            result.IsSuccess.ShouldBeTrue();
            result.Value.Items.ElementAt(1).Date.ShouldBe(date.ApplyTimeZone(TestData.TimeZone));
            result.Value.Items.ElementAt(1).LogType.ShouldBe(AlertLogResponseDto.LogType.EscalatedAlert);
            result.Value.Items.ElementAt(1).UserName.ShouldBe(userName);
        }

        [Fact]
        public async Task GetAlertLogs_ShouldMapAlertDismissal()
        {
            var date = new DateTime(2020, 1, 5);
            var userName = "User";
            var user = new GlobalCoordinatorUser { Name = userName };

            _alerts.First().Status = AlertStatus.Dismissed;
            _alerts.First().DismissedAt = date;
            _alerts.First().DismissedBy = user;

            var result = await _alertService.GetLogs(TestData.AlertId);

            result.IsSuccess.ShouldBeTrue();
            result.Value.Items.ElementAt(1).Date.ShouldBe(date.ApplyTimeZone(TestData.TimeZone));
            result.Value.Items.ElementAt(1).LogType.ShouldBe(AlertLogResponseDto.LogType.DismissedAlert);
            result.Value.Items.ElementAt(1).UserName.ShouldBe(userName);
        }

        [Fact]
        public async Task GetAlertLogs_ShouldMapAlertClosure()
        {
            var date = new DateTime(2020, 1, 5);
            var userName = "User";
            var user = new GlobalCoordinatorUser { Name = userName };

            _alerts.First().Status = AlertStatus.Closed;
            _alerts.First().ClosedAt = date;
            _alerts.First().ClosedBy = user;

            var result = await _alertService.GetLogs(TestData.AlertId);

            result.IsSuccess.ShouldBeTrue();
            result.Value.Items.ElementAt(1).Date.ShouldBe(date.ApplyTimeZone(TestData.TimeZone));
            result.Value.Items.ElementAt(1).LogType.ShouldBe(AlertLogResponseDto.LogType.ClosedAlert);
            result.Value.Items.ElementAt(1).UserName.ShouldBe(userName);
        }

        [Fact]
        public async Task GetAlertLogs_ShouldOrderItemsByDate()
        {
            var reportAcceptedAt = new DateTime(2020, 1, 5);
            var escalatedAt = new DateTime(2020, 1, 10);
            var closedAt = new DateTime(2020, 1, 15);
            var user = new GlobalCoordinatorUser();

            _alerts.First().Status = AlertStatus.Closed;
            _alerts.First().EscalatedAt = escalatedAt;
            _alerts.First().EscalatedBy = user;
            _alerts.First().ClosedAt = closedAt;
            _alerts.First().ClosedBy = user;

            _alerts.First().AlertReports.First().Report.Status = ReportStatus.Accepted;
            _alerts.First().AlertReports.First().Report.AcceptedAt = reportAcceptedAt;
            _alerts.First().AlertReports.First().Report.AcceptedBy = user;

            var result = await _alertService.GetLogs(TestData.AlertId);

            result.IsSuccess.ShouldBeTrue();
            result.Value.Items.Count().ShouldBe(4);
            result.Value.Items.ElementAt(0).Date.ShouldBe(TestData.AlertCreatedAt.ApplyTimeZone(TestData.TimeZone));
            result.Value.Items.ElementAt(1).Date.ShouldBe(reportAcceptedAt.ApplyTimeZone(TestData.TimeZone));
            result.Value.Items.ElementAt(2).Date.ShouldBe(escalatedAt.ApplyTimeZone(TestData.TimeZone));
            result.Value.Items.ElementAt(3).Date.ShouldBe(closedAt.ApplyTimeZone(TestData.TimeZone));
        }

        private static class TestData
        {
            public const int AlertId = 1;
            public const int ReportId = 23;
            public const int ContentLanguageId = 4;
            public const int NationalSocietyId = 5;
            public const string ContentLanguageCode = "en";
            public const string EscalationEmailSubject = "subject";
            public const string EscalationEmailBody = "body";
            public const string ApiKey = "123";
            public const string GatewayEmail = "gw1@example.com";
            public static readonly DateTime AlertCreatedAt = new DateTime(2020, 1, 1);
            public static readonly string TimeZoneName = "UTC";
            public static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneName);
            public static readonly GlobalCoordinatorUser DefaultUser = new GlobalCoordinatorUser { Name = "DefaultUser" };

            public static List<Alert> GetAlerts() =>
                new List<Alert>
                {
                    new Alert
                    {
                        Id = AlertId,
                        Status = AlertStatus.Escalated,
                        CreatedAt = AlertCreatedAt,
                        EscalatedBy = DefaultUser,
                        DismissedBy = DefaultUser,
                        ClosedBy = DefaultUser,
                        AlertReports = new List<AlertReport>
                        {
                            new AlertReport
                            {
                                Report = new Report
                                {
                                    Id = ReportId,
                                    Status = ReportStatus.Pending,
                                    AcceptedBy = DefaultUser,
                                    RejectedBy = DefaultUser,
                                    RawReport = new RawReport
                                    {
                                        ApiKey = ApiKey,
                                        Village = new Village()
                                    }
                                }
                            }
                        },
                        ProjectHealthRisk = new ProjectHealthRisk
                        {
                            AlertRule = new AlertRule(),
                            HealthRisk = new HealthRisk
                            {
                                LanguageContents = new List<HealthRiskLanguageContent>
                                {
                                    new HealthRiskLanguageContent
                                    {
                                        ContentLanguage = new ContentLanguage
                                        {
                                            Id = ContentLanguageId,
                                            LanguageCode = ContentLanguageCode
                                        }
                                    }
                                }
                            },
                            Project = new Project
                            {
                                TimeZone = TimeZoneName,
                                EmailAlertRecipients = new List<EmailAlertRecipient> { new EmailAlertRecipient { EmailAddress = "aaa@aaa.com" } },
                                SmsAlertRecipients = new List<SmsAlertRecipient> { new SmsAlertRecipient { PhoneNumber = "+12345678" } },
                                NationalSociety = new NationalSociety
                                {
                                    Id = NationalSocietyId,
                                    ContentLanguage = new ContentLanguage
                                    {
                                        Id = ContentLanguageId,
                                        LanguageCode = ContentLanguageCode
                                    }
                                }
                            }
                        }
                    }
                };

            public static List<GatewaySetting> GetGatewaySettings() =>
                new List<GatewaySetting>
                {
                    new GatewaySetting
                    {
                        ApiKey = ApiKey,
                        GatewayType = GatewayType.SmsEagle,
                        EmailAddress = GatewayEmail,
                        NationalSocietyId = NationalSocietyId
                    }
                };
        }
    }
}
