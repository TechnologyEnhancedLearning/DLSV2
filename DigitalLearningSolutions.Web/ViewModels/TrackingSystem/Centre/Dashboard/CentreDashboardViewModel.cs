namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Dashboard
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Helpers;

    public class CentreDashboardViewModel
    {
        public CentreDashboardViewModel(
            CentreDashboardInformation dashboardInformation,
            string userIpAddress,
            int unacknowledgedNotificationsCount
        )
        {
            CentreDetails = new DashboardCentreDetailsViewModel(
                dashboardInformation.Centre,
                userIpAddress,
                dashboardInformation.CentreRank
            );
            FirstName = string.IsNullOrWhiteSpace(dashboardInformation.FirstName)
                ? "User"
                : dashboardInformation.FirstName;
            CourseCategory = dashboardInformation.CategoryName ?? "all";
            NumberOfDelegates = dashboardInformation.DelegateCount;
            NumberOfAllCourses = dashboardInformation.AllCourseCount;
            NumberOfActiveCourses = dashboardInformation.ActiveCourseCount;
            NumberOfInactiveArchivedCourses = dashboardInformation.InactiveArchivedCourseCount;
            NumberOfAdmins = dashboardInformation.AdminCount;
            NumberOfSupportTickets = dashboardInformation.SupportTicketCount;
            ViewNotificationsButtonText = "View " + unacknowledgedNotificationsCount + " notification" +
                                          DisplayStringHelper.GetPluralitySuffix(unacknowledgedNotificationsCount);
        }

        public string FirstName { get; set; }

        public string CourseCategory { get; set; }

        public int NumberOfDelegates { get; set; }

        public int NumberOfAllCourses { get; set; }
        public int NumberOfActiveCourses { get; set; }
        public int NumberOfInactiveArchivedCourses { get; set; }

        public int NumberOfAdmins { get; set; }

        public int NumberOfSupportTickets { get; set; }

        public DashboardCentreDetailsViewModel CentreDetails { get; set; }

        public string ViewNotificationsButtonText { get; set; }
    }
}
