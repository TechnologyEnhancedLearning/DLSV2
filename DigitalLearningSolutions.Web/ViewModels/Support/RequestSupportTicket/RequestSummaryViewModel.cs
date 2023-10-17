

namespace DigitalLearningSolutions.Web.ViewModels.Support.RequestSupportTicket
{
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Web.Models;
    using DocumentFormat.OpenXml.Office2010.ExcelAc;
    using FluentMigrator.Infrastructure;
    using System.Collections.Generic;
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
            //RequestSubject = data.RequestSubject;
            //RequestDescription = data.RequestDescription;
        }
        [Required(ErrorMessage = "Please enter request summary")]
        public string? RequestSubject { get; set; }
        public string? RequestDescription { get; set; }
        public int? RequestTypeId { get; set; }
        public string? RequestType { get; set; }
    }
}
