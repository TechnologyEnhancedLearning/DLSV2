using DigitalLearningSolutions.Web.Models.Enums;

namespace DigitalLearningSolutions.Web.ViewModels.Support.RequestSupportTicket
{
    public class RequestSupportTicketViewModel:BaseSupportViewModel
    {
        public RequestSupportTicketViewModel(
            DlsSubApplication dlsSubApplication,
            SupportPage currentPage,
            string currentSystemBaseUrl
        ) : base(dlsSubApplication, currentPage, currentSystemBaseUrl) { }
    }
}
