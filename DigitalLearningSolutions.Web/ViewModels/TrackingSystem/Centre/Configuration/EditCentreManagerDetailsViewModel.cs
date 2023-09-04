namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditCentreManagerDetailsViewModel
    {
        public EditCentreManagerDetailsViewModel() { }

        public EditCentreManagerDetailsViewModel(Centre centre)
        {
            CentreId = centre.CentreId;
            FirstName = centre.ContactForename;
            LastName = centre.ContactSurname;
            Email = centre.ContactEmail;
            Telephone = centre.ContactTelephone;
        }

        public int CentreId { get; set; }

        [Required(ErrorMessage = "Enter a first name")]
        [MaxLength(250, ErrorMessage = CommonValidationErrorMessages.TooLongFirstName)]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Enter a last name")]
        [MaxLength(250, ErrorMessage = CommonValidationErrorMessages.TooLongLastName)]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Enter an email")]
        [MaxLength(250, ErrorMessage = "Email must be 250 characters or fewer")]
        [EmailAddress(ErrorMessage = "Enter an email in the correct format, like name@example.com")]
        [NoWhitespace(ErrorMessage = "Email must not contain any whitespace characters")]
        public string? Email { get; set; }

        [MaxLength(250, ErrorMessage = "Telephone number must be 250 characters or fewer")]
        [RegularExpression(@"[0-9 ]+", ErrorMessage = "Enter a Telephone number in the correct format.")]
        public string? Telephone { get; set; }
    }
}
