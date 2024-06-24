using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.Support;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Services
{
    public interface IRequestSupportTicketService
    {
        IEnumerable<RequestType> GetRequestTypes();
        string? GetUserCentreEmail(int userId, int centreId);

    }
    public class RequestSupportTicketService : IRequestSupportTicketService
    {
        private readonly IRequestSupportTicketDataService requestSupportTicketDataService;
        public RequestSupportTicketService(IRequestSupportTicketDataService requestSupportTicketDataService)
        {
            this.requestSupportTicketDataService = requestSupportTicketDataService; 
        }
        public IEnumerable<RequestType> GetRequestTypes()
        {
           return  requestSupportTicketDataService.GetRequestTypes();
        }

        public string? GetUserCentreEmail(int userId, int centreId)
        {
            return requestSupportTicketDataService.GetUserCentreEmail(userId, centreId);
        }
    }
}
