namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Dashboard
{
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Web.Helpers;

    public class CentreDashboardViewModel
    {
        public CentreDashboardViewModel(
            Centre centre,
            string? firstName,
            string? categoryName,
            string userIpAddress,
            int delegates,
            int courses,
            int admins,
            int supportTickets,
            int? centreRank,
            int unacknowledgedNotificationsCount
        )
        {
            CentreDetails = new DashboardCentreDetailsViewModel(centre, userIpAddress, centreRank);
            FirstName = string.IsNullOrWhiteSpace(firstName) ? "User" : firstName;
            CourseCategory = categoryName ?? "all";
            NumberOfDelegates = delegates;
            NumberOfCourses = courses;
            NumberOfAdmins = admins;
            NumberOfSupportTickets = supportTickets;
            ViewNotificationsButtonText = "View " + unacknowledgedNotificationsCount + " notification" +
                                          DisplayStringHelper.GetPluralitySuffix(unacknowledgedNotificationsCount);
        }

        public string FirstName { get; set; }

        public string CourseCategory { get; set; }

        public int NumberOfDelegates { get; set; }

        public int NumberOfCourses { get; set; }

        public int NumberOfAdmins { get; set; }

        public int NumberOfSupportTickets { get; set; }

        public DashboardCentreDetailsViewModel CentreDetails { get; set; }

        public string ViewNotificationsButtonText { get; set; }
    }
}
