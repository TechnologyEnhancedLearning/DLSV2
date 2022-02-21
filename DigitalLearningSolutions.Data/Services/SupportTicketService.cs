namespace DigitalLearningSolutions.Data.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;

    public interface ISupportTicketService
    {
        int GetNumberOfTicketsForCentreAdmin(int centreId, int adminId);
    }

    public class SupportTicketService : ISupportTicketService
    {
        private readonly ISupportTicketDataService ticketDataService;
        private readonly IUserDataService userDataService;

        public SupportTicketService(ISupportTicketDataService ticketDataService, IUserDataService userDataService)
        {
            this.ticketDataService = ticketDataService;
            this.userDataService = userDataService;
        }

        public int GetNumberOfTicketsForCentreAdmin(int centreId, int adminId)
        {
            var admin = userDataService.GetAdminUserById(adminId);

            if (admin == null)
            {
                return 0;
            }

            return admin.IsCentreManager
                ? ticketDataService.GetNumberOfUnarchivedTicketsForCentreId(centreId)
                : ticketDataService.GetNumberOfUnarchivedTicketsForAdminId(adminId);
        }
    }
}
