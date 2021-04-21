namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.ComponentModel.DataAnnotations;

    public class PasswordViewModel
    {
        public PasswordViewModel()
        {
            Password = string.Empty;
            ConfirmPassword = string.Empty;
        }

        [Required(ErrorMessage = "Please enter a password to register.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
    }
}
