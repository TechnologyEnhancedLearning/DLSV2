namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Attributes;

    public class EditCentreManagerDetailsViewModel
    {
        public EditCentreManagerDetailsViewModel(){ }

        public EditCentreManagerDetailsViewModel(Centre centre)
        {
            FirstName = centre.ContactForename;
            LastName = centre.ContactSurname;
            Email = centre.ContactEmail;
            Telephone = centre.ContactTelephone;
        }

        [Required(ErrorMessage = "Enter a first name")]
        [MaxLength(250, ErrorMessage = "First name must be 250 characters or fewer")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Enter a last name")]
        [MaxLength(250, ErrorMessage = "Last name must be 250 characters or fewer")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Enter an email address")]
        [MaxLength(250, ErrorMessage = "Email address must be 250 characters or fewer")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [NoWhitespace("Email address must not contain any whitespace characters")]
        public string? Email { get; set; }

        [MaxLength(250, ErrorMessage = "Telephone number must be 250 characters or fewer")]
        public string? Telephone { get; set; }
    }
}
