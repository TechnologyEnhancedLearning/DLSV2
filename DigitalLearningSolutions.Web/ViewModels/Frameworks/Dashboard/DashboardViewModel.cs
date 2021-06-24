namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using System.Collections.Generic;

    public class DashboardViewModel
    {
        public DashboardViewModel(string? username, bool isFrameworkDeveloper, bool isFrameworkContributor, bool isWorkforceManager, bool isWorkforceContributor, DashboardData dashboardData, IEnumerable<DashboardToDoItem> dashboardToDoItems)
        {
            Username = string.IsNullOrWhiteSpace(username) ? "User" : username;
            IsFrameworkDeveloper = isFrameworkDeveloper;
            IsFrameworkContributor = isFrameworkContributor;
            IsWorkforceManager = isWorkforceManager;
            IsWorkforceContributor = isWorkforceContributor;
            DashboardData = dashboardData;
            DashboardToDoItems = dashboardToDoItems;
        }
        public string Username { get; set; }
        public bool IsFrameworkDeveloper { get; set; }
        public bool IsFrameworkContributor { get; set; }
        public bool IsWorkforceManager { get; set; }
        public bool IsWorkforceContributor { get; set; }
        public DashboardData DashboardData { get; set; }
        public IEnumerable<DashboardToDoItem> DashboardToDoItems { get; set; }
    }
}
