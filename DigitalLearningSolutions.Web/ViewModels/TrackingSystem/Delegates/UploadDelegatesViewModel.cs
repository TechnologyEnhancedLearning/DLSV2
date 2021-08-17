namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre;
    using Microsoft.AspNetCore.Http;

    public class UploadDelegatesViewModel : WelcomeEmailViewModel
    {
        [Required(ErrorMessage = "Delegates update file is required.")]
        [AllowedExtensions(new[] { ".xlsx" }, "Delegates update file must be in xlsx format.")]
        public IFormFile? DelegatesFile { get; set; }

        public DateTime? GetWelcomeEmailDate()
        {
            return ShouldSendEmail ? new DateTime(Year!.Value, Month!.Value, Day!.Value) : (DateTime?)null;
        }
    }
}
