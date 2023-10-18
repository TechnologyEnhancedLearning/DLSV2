

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

        [Required(ErrorMessage = "Please select at least one image")]
        [AllowedExtensions(new[] { ".png", ".jpg", ".jpeg" }, "Delegates update file must be in image format")]
        [MaxFileSize(20 * 1024 * 1024, "Maximum allowed file size is 20MB")]
        public List<IFormFile>? ImageFiles { get; set; }
    }
}
