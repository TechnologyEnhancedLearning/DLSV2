namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Enter your first name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Enter your last name")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Enter your email address")]
        [EmailAddress(ErrorMessage = "Enter an email address with an @ in it, such as example@domain.com")]
        public string? Email { get; set; }
    }
}
