

namespace DigitalLearningSolutions.Web.ViewModels.Support.RequestSupportTicket
{
    using DigitalLearningSolutions.Web.Models;
    using System.ComponentModel.DataAnnotations;
    public class RequestSummaryViewModel
    {
        public RequestSummaryViewModel()
        {

        }
        public RequestSummaryViewModel(RequestSupportTicketData data)
        {
            RequestTypeId = data.RequestTypeId;
            RequestType = data.RequestType;
            RequestSubject = data.RequestSubject;
            RequestDescription = data.RequestDescription;
        }
        [MaxLength(250, ErrorMessage = "Summary must be 250 characters or fewer")]
        [Required(ErrorMessage = "Please enter request summary")]
        public string? RequestSubject { get; set; }

        [Required(ErrorMessage = "Please enter request description")]
        public string? RequestDescription { get; set; }

        public int? RequestTypeId { get; set; }

        public string? RequestType { get; set; }
    }
}
