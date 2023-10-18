﻿

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
            RequestSubject = data.RequestSubject;
            RequestDescription = data.RequestDescription;
        }
        public string? RequestSubject { get; set; }
        [MaxLength(250, ErrorMessage = "Description must be 250 characters or fewer")]
        public string? RequestDescription { get; set; }
        public int? RequestTypeId { get; set; }
        public string? RequestType { get; set; }
    }
}
