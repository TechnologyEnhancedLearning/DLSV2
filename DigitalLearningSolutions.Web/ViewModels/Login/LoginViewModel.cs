namespace DigitalLearningSolutions.Web.ViewModels.Login
{
    using System.ComponentModel.DataAnnotations;

    public class LoginViewModel
    {
        public LoginViewModel()
        {
            Username = string.Empty;
            Password = string.Empty;
            RememberMe = false;
        }

        public LoginViewModel(string? returnUrl)
        {
            Username = string.Empty;
            Password = string.Empty;
            RememberMe = false;
            ReturnUrl = returnUrl;
        }

        [Required(ErrorMessage = "Please enter a username to log in.")]
        public string? Username { get; set; }

        [Required (ErrorMessage = "Please enter a password to log in.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
