

namespace DigitalLearningSolutions.Web.ViewModels.Support.RequestSupportTicket
{
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models;
    using FluentMigrator.Infrastructure;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class RequestAttachmentViewModel
    {
        public RequestAttachmentViewModel(
            )
        {
            SizeLimit = 20;
            AllowedExtensions = new string[] { ".png", ".jpg", ".jpeg", ".PNG", ".JPG", ".JPEG" };
        }
        public RequestAttachmentViewModel(RequestSupportTicketData data)
        {
            RequestAttachment = data.RequestAttachment;
            ImageFiles = data.ImageFiles;
            FileSizeFlag = false;
            FileExtensionFlag = false;


        }
        public string? Attachment { get; set; }
        public int? SizeLimit { get; set; }
        public string[]? AllowedExtensions { get; set; }
        public List<RequestAttachment>? RequestAttachment { get; set; }
        public bool? FileSizeFlag { get; set; }
        public bool? FileExtensionFlag { get; set; }
        public string? FileExtensionError { get; set; }
        public string? FileSizeError { get; set; }

        [Required(ErrorMessage = "Please select at least one image")]
        [AllowedExtensions(new[] { ".png", ".jpg", ".jpeg" }, "Upload file must be in image formats like jpg,jpeg,png")]
        [MaxFileSize(20 * 1024 * 1024, "Maximum allowed file size is 20MB")]
        public List<IFormFile>? ImageFiles { get; set; }


    }
    public static class FileSizeCalc
    {
        public enum SizeUnits
        {
            Byte, KB, MB, GB, TB, PB, EB, ZB, YB
        }

        public static string ToSize(this Int64 value, SizeUnits unit)
        {
            return (value / (double)Math.Pow(1024, (Int64)unit)).ToString("0.00");
        }
    }

}
