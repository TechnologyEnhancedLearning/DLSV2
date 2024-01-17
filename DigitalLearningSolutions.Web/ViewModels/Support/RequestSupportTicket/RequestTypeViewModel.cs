using DigitalLearningSolutions.Data.Models.Support;
using System.Collections.Generic;
using DigitalLearningSolutions.Web.Models;

namespace DigitalLearningSolutions.Web.ViewModels.Support.RequestSupportTicket
{
    public class RequestTypeViewModel
    {
        public RequestTypeViewModel() { }
        public RequestTypeViewModel(List<RequestType> requestTypes, RequestSupportTicketData data)
        {
            RequestTypes = requestTypes;
            Id = data.RequestTypeId;
            //RequestDescription = data.RequestDescription;
            Type = data.RequestType;
            //RequestSubject = data.RequestSubject;
        }
        public IEnumerable<RequestType>? RequestTypes { get; set; }
        public int? Id { get; set; }
        public string? Type { get; set; }
    }
}
