namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class InternalPersonalInformationViewModel
    {
        public InternalPersonalInformationViewModel(){}

        public InternalPersonalInformationViewModel(InternalDelegateRegistrationData data)
        {
            CentreSpecificEmail = data.CentreSpecificEmail;
            Centre = data.Centre;
            IsCentreSpecificRegistration = data.IsCentreSpecificRegistration;
        }

        [MaxLength(255, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(CommonValidationErrorMessages.WhitespaceInEmail)]
        public string? CentreSpecificEmail { get; set; }

        [Required(ErrorMessage = "Select a centre")]
        public int? Centre { get; set; }

        public string? CentreName { get; set; }

        public bool IsCentreSpecificRegistration { get; set; }

        public IEnumerable<SelectListItem> CentreOptions { get; set; } = new List<SelectListItem>();
    }
}
