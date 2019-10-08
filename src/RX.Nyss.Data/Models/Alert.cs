﻿using System;
using System.Collections.Generic;
using RX.Nyss.Data.Concepts;

namespace RX.Nyss.Data.Models
{
    public class Alert
    {
        public int Id { get; set; }

        public AlertStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Comments { get; set; }

        public virtual ProjectHealthRisk ProjectHealthRisk { get; set; }

        public virtual ICollection<AlertReport> AlertReports { get; set; }
    }
}
