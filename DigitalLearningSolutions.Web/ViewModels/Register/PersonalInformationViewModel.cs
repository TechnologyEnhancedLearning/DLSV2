namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;

    public class PersonalInformationViewModel : InternalPersonalInformationViewModel
    {
        public PersonalInformationViewModel() { }

        public PersonalInformationViewModel(RegistrationData data)
        {
            FirstName = data.FirstName;
            LastName = data.LastName;
            Centre = data.Centre;
            PrimaryEmail = data.Email;
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

        [Required(ErrorMessage = "Enter an email")]
        [MaxLength(255, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(CommonValidationErrorMessages.WhitespaceInEmail)]
        public string? PrimaryEmail { get; set; }

        [MaxLength(250, ErrorMessage = CommonValidationErrorMessages.TooLongAlias)]
        public string? Alias { get; set; }
    }
}
