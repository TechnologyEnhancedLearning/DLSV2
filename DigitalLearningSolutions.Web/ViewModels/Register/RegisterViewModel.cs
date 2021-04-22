namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please enter a first name.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a last name.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Please enter an email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? Email { get; set; }
    }
}
