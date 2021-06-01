namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Dashboard
{
    using DigitalLearningSolutions.Data.Models;

    public class CentreDashboardViewModel
    {
        public CentreDashboardViewModel(Centre centre, string? firstName, string? categoryName, string userIpAddress)
        {
            CentreDetails = new DashboardCentreDetailsViewModel(centre, userIpAddress);
            FirstName = firstName ?? "User";
            CourseCategory = categoryName ?? "all";
        }

        public string FirstName { get; set; }

        public string CourseCategory { get; set; }

        public int Delegates { get; set; }

        public int Courses { get; set; }

        public int Admins { get; set; }

        public int HelpTickets { get; set; }

        public DashboardCentreDetailsViewModel CentreDetails { get; set; }
    }
}
