namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterViewModel
    {
        public RegisterViewModel()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
        }

        [Required(ErrorMessage = "Please enter a first name to register.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a last name to register.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Please enter an email address to register.")]
        // QQ is this the correct error message
        [EmailAddress(ErrorMessage = "Please enter a valid email address to register.")]
        public string Email { get; set; }
    }
}
