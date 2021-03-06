using System;
using RX.Nyss.Data.Models;
using RX.Nyss.ReportApi.Features.Reports.Contracts;
using RX.Nyss.ReportApi.Features.Reports.Models;

namespace RX.Nyss.ReportApi.Features.Reports.Handlers
{
    public class ReportData
    {
        public DataCollector DataCollector { get; set; }
        public ParsedReport ParsedReport { get; set; }
        public ProjectHealthRisk ProjectHealthRisk { get; set; }
        public DateTime ReceivedAt { get; set; }
    }

    public class ErrorReportData
    {
        public string Sender { get; set; }
        public string LanguageCode { get; set; }
        public ReportErrorType ReportErrorType { get; set; }
    }

    public class ReportValidationResult
    {
        public bool IsSuccess { get; set; }
        public ReportData ReportData { get; set; }
        public ErrorReportData ErrorReportData { get; set; }
        public GatewaySetting GatewaySetting { get; set; }
    }
}
