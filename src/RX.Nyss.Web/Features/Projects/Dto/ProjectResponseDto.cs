﻿using System.Collections.Generic;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Web.Features.Alerts.Dto;

namespace RX.Nyss.Web.Features.Projects.Dto
{
    public class ProjectResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string TimeZoneId { get; set; }

        public ProjectState State { get; set; }

        public IEnumerable<ProjectHealthRiskResponseDto> ProjectHealthRisks { get; set; }

        public IEnumerable<EmailAlertRecipientDto> EmailAlertRecipients { get; set; }

        public IEnumerable<SmsAlertRecipientDto> SmsAlertRecipients { get; set; }

        public ProjectFormDataResponseDto FormData { get; set; }

        public int ContentLanguageId { get; set; }
    }
}
