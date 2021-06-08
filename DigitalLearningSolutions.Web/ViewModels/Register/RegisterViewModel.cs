namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Enter your first name")]
        [MaxLength(100, ErrorMessage = "First name must be 100 characters or fewer")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Enter your last name")]
        [MaxLength(100, ErrorMessage = "Last name must be 100 characters or fewer")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Enter your email address")]
        [MaxLength(100, ErrorMessage = "Email address must be 100 characters or fewer")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Select a centre")]
        public int? Centre { get; set; }
    }
}
