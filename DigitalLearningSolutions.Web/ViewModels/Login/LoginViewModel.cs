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

        [Display(Order = -1)]
        [Required(ErrorMessage = "Please enter a username to log in.")]
        public string Username { get; set; }

        [Display(Order = 2)]
        [Required (ErrorMessage = "Please enter a password to log in.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Order = 3)]
        public bool RememberMe { get; set; }
    }
}
