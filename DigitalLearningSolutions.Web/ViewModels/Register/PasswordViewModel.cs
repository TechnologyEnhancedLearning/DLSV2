namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.ComponentModel.DataAnnotations;

    public class PasswordViewModel
    {
        [Required(ErrorMessage = "Please enter a password.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"(?=.*?[^\w\s])(?=.*?[0-9])(?=.*?[A-Za-z]).*",
            ErrorMessage = "Password must contain at least 1 letter, 1 number and 1 symbol")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirm password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
