namespace DigitalLearningSolutions.Data.Services
{
    using System;
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

            var delegateCount = userDataService.GetNumberOfApprovedDelegatesAtCentre(centreId);
            var courseCount =
                courseDataService.GetNumberOfActiveCoursesAtCentreFilteredByCategory(
                    centreId,
                    adminUser!.CategoryIdFilter
                );
            var adminCount = userDataService.GetNumberOfActiveAdminsAtCentre(centreId);
            var supportTicketCount = adminUser.IsCentreManager
                ? ticketDataService.GetNumberOfUnarchivedTicketsForCentreId(centreId)
                : ticketDataService.GetNumberOfUnarchivedTicketsForAdminId(adminId);
            var centreRank = centresService.GetCentreRankForCentre(centreId);

            return new CentreDashboardInformation(centre, adminUser, delegateCount, courseCount, adminCount, supportTicketCount, centreRank);
        }
    }
}
