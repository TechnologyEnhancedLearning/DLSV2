

namespace DigitalLearningSolutions.Web.ViewModels.Support.RequestSupportTicket
{
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models;
    using FluentMigrator.Infrastructure;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class RequestAttachmentViewModel
    {
        public RequestAttachmentViewModel() { }
        public RequestAttachmentViewModel(RequestSupportTicketData data)
        {
            RequestAttachment = data.RequestAttachment;
            ImageFiles = data.ImageFiles;
        }
        public string? Attachment { get; set; }
        public List<RequestAttachment>? RequestAttachment { get; set; }
        [Required(ErrorMessage = "Please select atleast one image")]
        [DataType(DataType.Upload)]
        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".png", ".jpg", ".jpeg" })]
        public List<IFormFile>? ImageFiles { get; set; }
    }
}
