namespace DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;

    public class DelegatePersonalInformationViewModel : InternalPersonalInformationViewModel
    {
        public DelegatePersonalInformationViewModel() { }

        public DelegatePersonalInformationViewModel(RegistrationData data)
        {
            FirstName = data.FirstName;
            LastName = data.LastName;
            Centre = data.Centre;
            CentreSpecificEmail = data.CentreSpecificEmail;
        }

        [Required(ErrorMessage = "Enter a first name")]
        [MaxLength(250, ErrorMessage = CommonValidationErrorMessages.TooLongFirstName)]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Enter a last name")]
        [MaxLength(250, ErrorMessage = CommonValidationErrorMessages.TooLongLastName)]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Enter an email address")]
        [MaxLength(255, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(CommonValidationErrorMessages.WhitespaceInEmail)]
        public new string? CentreSpecificEmail { get; set; }
    }
}
