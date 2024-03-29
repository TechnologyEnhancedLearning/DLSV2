﻿namespace DigitalLearningSolutions.Data.Models.Supervisor
{
    using System;
    public class SupervisorDashboardToDoItem
    {
        public int ID { get; set; }
        public int SupervisorDelegateId { get; set; }
        public string DelegateName { get; set; } = string.Empty;
        public string ProfileName { get; set; } = string.Empty;
        public DateTime Requested { get; set; }
        public bool SignOffRequest { get; set; }
        public bool ResultsReviewRequest { get; set; }
    }
}
