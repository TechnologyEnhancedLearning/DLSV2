namespace DigitalLearningSolutions.Web.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models;

    public interface IDashboardInformationService
    {
        CentreDashboardInformation? GetDashboardInformationForCentre(int centreId, int adminId);
    }

    public class DashboardInformationService : IDashboardInformationService
    {
        private readonly ICentresDataService centresDataService;
        private readonly ICentresService centresService;
        private readonly ICourseDataService courseDataService;
        private readonly ISupportTicketDataService ticketDataService;
        private readonly IUserDataService userDataService;

        public DashboardInformationService(
            ISupportTicketDataService ticketDataService,
            IUserDataService userDataService,
            ICourseDataService courseDataService,
            ICentresService centresService,
            ICentresDataService centresDataService
        )
        {
            this.ticketDataService = ticketDataService;
            this.userDataService = userDataService;
            this.courseDataService = courseDataService;
            this.centresService = centresService;
            this.centresDataService = centresDataService;
        }

        public CentreDashboardInformation? GetDashboardInformationForCentre(int centreId, int adminId)
        {
            var adminUser = userDataService.GetAdminUserById(adminId);
            var centre = centresDataService.GetCentreDetailsById(centreId);
            if (centre == null || adminUser == null)
            {
                return null;
            }

            (var delegates, var delegateCount) = userDataService.GetDelegateUserCards("", 0, 10, "SearchableName", "Ascending", centreId,
            "Any", "Any", "Any", "Any", "Any", "Any", 0, null, "Any", "Any", "Any", "Any", "Any", "Any");
          
            var courseCount =
                courseDataService.GetNumberOfActiveCoursesAtCentreFilteredByCategory(
                    centreId,
                    adminUser.CategoryId
                );
         
            var adminCount = userDataService.GetNumberOfAdminsAtCentre(centreId);
            var supportTicketCount = adminUser.IsCentreManager
                ? ticketDataService.GetNumberOfUnarchivedTicketsForCentreId(centreId)
                : ticketDataService.GetNumberOfUnarchivedTicketsForAdminId(adminId);
            var centreRank = centresService.GetCentreRankForCentre(centreId);

            return new CentreDashboardInformation(centre, adminUser, delegateCount, courseCount,  adminCount, supportTicketCount, centreRank);
        }
    }
}
