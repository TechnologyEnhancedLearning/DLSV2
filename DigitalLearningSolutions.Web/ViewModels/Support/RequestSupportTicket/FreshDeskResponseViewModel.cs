namespace DigitalLearningSolutions.Web.ViewModels.Support.RequestSupportTicket
{
    public class FreshDeskResponseViewModel
    {
        public FreshDeskResponseViewModel() { }
        public FreshDeskResponseViewModel(long? ticketId,string? errorMessage) {
            TicketId = ticketId;
            ErrorMessage = errorMessage;
        }
        public long? TicketId { get; set; }
        public string? ErrorMessage { get; set;}
    }
}
