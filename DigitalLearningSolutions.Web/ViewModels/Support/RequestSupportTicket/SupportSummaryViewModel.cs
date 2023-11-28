using DigitalLearningSolutions.Data.Models.Support;
using DigitalLearningSolutions.Web.Models;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace DigitalLearningSolutions.Web.ViewModels.Support.RequestSupportTicket
{
    public class SupportSummaryViewModel
    {
        public SupportSummaryViewModel(RequestSupportTicketData data)
        {

            Description = data.RequestDescription;
            RequestSubject = data.RequestSubject;
            RequestType = data.RequestType;
            UserName = data.UserName;
            UserCentreEmail = data.UserCentreEmail;
            AdminUserID = data.AdminUserID ?? 0;
            if (RequestAttachment != null)
                RequestAttachment.AddRange(data.RequestAttachment);
            else RequestAttachment = data.RequestAttachment;
            //CentreName = (JsonArray)data.CentreName;
        }

        public SupportSummaryViewModel() { }

        public string? Summary { get; set; }
        public string? Description { get; set; }
        public string? RequestSubject { get; set; }
        public string? RequestType { get; set; }
        public string UserName { get; set; }
        public string UserCentreEmail { get; set; }
        public int AdminUserID { get; set; }
        public JsonArray CentreName { get; set; }
        public List<RequestAttachment> RequestAttachment { get; set; }
    }
}
