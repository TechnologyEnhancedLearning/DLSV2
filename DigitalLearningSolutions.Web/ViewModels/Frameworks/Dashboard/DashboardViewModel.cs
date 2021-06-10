namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using System.Collections.Generic;

    public class DashboardViewModel
    {
        public bool IsFrameworkDeveloper { get; set; }
        public bool IsFrameworkContributor { get; set; }
        public bool IsWorkforceManager { get; set; }
        public bool IsWorkforceContributor { get; set; }
        public DashboardData dashboardData { get; set; }
        public IEnumerable<DashboardToDoItem> dashboardToDoItems { get; set; }
    }
}
