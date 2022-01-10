namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Web.Attributes;
    using Microsoft.AspNetCore.Http;

    public class EditCentreDetailsViewModel
    {
        public EditCentreDetailsViewModel() { }

        public EditCentreDetailsViewModel(Centre centre)
        {
            NotifyEmail = centre.NotifyEmail;
            BannerText = centre.BannerText;
            CentreSignature = centre.SignatureImage;
            CentreLogo = centre.CentreLogo;
        }

        [MaxLength(250, ErrorMessage = "Email address must be 250 characters or fewer")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [NoWhitespace("Email address must not contain any whitespace characters")]
        public string? NotifyEmail { get; set; }

        [Required(ErrorMessage = "Enter the centre support details")]
        [MaxLength(250, ErrorMessage = "Centre support details must be 250 characters or fewer")]
        public string? BannerText { get; set; }

        public byte[]? CentreSignature { get; set; }

        [AllowedExtensions(new[] { ".png", ".tiff", ".jpg", ".jpeg", ".bmp", ".gif" })]
        public IFormFile? CentreSignatureFile { get; set; }

        public byte[]? CentreLogo { get; set; }

        [AllowedExtensions(new[] { ".png", ".tiff", ".jpg", ".jpeg", ".bmp", ".gif" })]
        public IFormFile? CentreLogoFile { get; set; }
    }
}
