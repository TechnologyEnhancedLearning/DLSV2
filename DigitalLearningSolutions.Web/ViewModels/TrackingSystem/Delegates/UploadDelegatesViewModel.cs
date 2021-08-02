namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre;
    using Microsoft.AspNetCore.Http;

    public class UploadDelegatesViewModel : WelcomeEmailViewModel
    {
        [Required(ErrorMessage = "Delegates update file is required.")]
        [AllowedExtensions(new[] { ".xlsx" }, "Delegates update file must be in xlsx format.")]
        public IFormFile? DelegatesDetailsFile { get; set; }
    }
}
