namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class PersonalInformationViewModel
    {
        public PersonalInformationViewModel() { }

        public PersonalInformationViewModel(RegistrationData data)
        {
            FirstName = data.FirstName;
            LastName = data.LastName;
            Centre = data.Centre;
            PrimaryEmail = data.PrimaryEmail;
            SecondaryEmail = data.SecondaryEmail;
        }

        public PersonalInformationViewModel(DelegateRegistrationData data) : this((RegistrationData)data)
        {
            IsCentreSpecificRegistration = data.IsCentreSpecificRegistration;
        }

        public PersonalInformationViewModel(DelegateRegistrationByCentreData data) : this(
            (DelegateRegistrationData)data
        )
        {
            Alias = data.Alias;
        }

        [Required(ErrorMessage = "Enter a first name")]
        [MaxLength(250, ErrorMessage = CommonValidationErrorMessages.TooLongFirstName)]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Enter a last name")]
        [MaxLength(250, ErrorMessage = CommonValidationErrorMessages.TooLongLastName)]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Enter a primary email address")]
        [MaxLength(255, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(CommonValidationErrorMessages.WhitespaceInEmail)]
        public string? PrimaryEmail { get; set; }

        [MaxLength(255, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(CommonValidationErrorMessages.WhitespaceInEmail)]
        public string? SecondaryEmail { get; set; }

        [Required(ErrorMessage = "Select a centre")]
        public int? Centre { get; set; }

        [MaxLength(250, ErrorMessage = CommonValidationErrorMessages.TooLongAlias)]
        public string? Alias { get; set; }

        public bool IsCentreSpecificRegistration { get; set; }

        public string? CentreName { get; set; }

        public IEnumerable<SelectListItem> CentreOptions { get; set; } = new List<SelectListItem>();
    }
}
