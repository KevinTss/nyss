﻿using RX.Nyss.Web.Features.Common.Dto;

namespace RX.Nyss.Web.Features.DataCollectors.Dto
{
    public class MapOverviewDataCollectorResponseDto
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public DataCollectorStatus Status { get; set; }
    }
}
