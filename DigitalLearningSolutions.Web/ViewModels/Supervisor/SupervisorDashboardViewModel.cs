﻿namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using System.Collections.Generic;

    public class SupervisorDashboardViewModel
    {
        public DashboardData DashboardData { get; set; }
        public IEnumerable<SupervisorDashboardToDoItem> SupervisorDashboardToDoItems { get; set; }
    }
}
