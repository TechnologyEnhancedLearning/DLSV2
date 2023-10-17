

namespace DigitalLearningSolutions.Web.Models
{
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Support.RequestSupportTicket;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;

    public class RequestSupportTicketData
    {
        public RequestSupportTicketData() { }
        public RequestSupportTicketData(string userName, string userCentreEmail, int adminUserID, string centreName)
        {
            CentreName = centreName;
            AdminUserID = adminUserID;
            UserCentreEmail = UserCentreEmail;
            UserName = userName;
        }
        public string? CentreName { get; set; }
        public string? UserCentreEmail { get; set; }
        public int? AdminUserID { get; set; }
        public string UserName { get; set; }
        public int? RequestTypeId { get; set; }
        public string? RequestType { get; set; }
        public string? RequestSubject { get; set; }
        public string? RequestDescription { get; set; }
        public List<IFormFile>? ImageFiles { get; set; }
        public List<RequestAttachment> RequestAttachment { get; set; }
        public void setRequestType(int requestType, string reqType)
        {
            RequestTypeId = requestType;
            RequestType = reqType;
        }
        public void setRequestSubjectDetails(RequestSummaryViewModel model)
        {
            RequestSubject = model.RequestSubject;
            RequestDescription = model.RequestDescription;

        }
        public void setImageFiles(List<RequestAttachment> requestAttachment)
        {
            if (RequestAttachment != null)
                RequestAttachment.AddRange(requestAttachment);
            else RequestAttachment = requestAttachment;
        }
    }
}
