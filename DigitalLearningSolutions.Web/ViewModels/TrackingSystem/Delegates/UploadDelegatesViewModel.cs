namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre;
    using Microsoft.AspNetCore.Http;

    public class UploadDelegatesViewModel : WelcomeEmailViewModel
    {
        public UploadDelegatesViewModel() { }

        public UploadDelegatesViewModel(DateTime welcomeEmailDate)
        {
            Day = welcomeEmailDate.Day;
            Month = welcomeEmailDate.Month;
            Year = welcomeEmailDate.Year;
        }

        [Required(ErrorMessage = "Delegates update file is required")]
        [AllowedExtensions(new[] { ".xlsx" }, "Delegates update file must be in xlsx format")]
        [MaxFileSize(5 * 1024 * 1024, "Maximum allowed file size is 5MB")]
        public IFormFile? DelegatesFile { get; set; }

        public DateTime GetWelcomeEmailDate()
        {
            return new DateTime(Year!.Value, Month!.Value, Day!.Value);
        }
    }
}
