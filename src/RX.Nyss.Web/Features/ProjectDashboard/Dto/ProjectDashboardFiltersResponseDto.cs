﻿using System.Collections.Generic;
using RX.Nyss.Web.Features.Common.Dto;

namespace RX.Nyss.Web.Features.ProjectDashboard.Dto
{
    public class ProjectDashboardFiltersResponseDto
    {
        public IEnumerable<HealthRiskDto> HealthRisks { get; set; }
    }
}
