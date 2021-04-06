namespace DigitalLearningSolutions.Web.ViewModels.ForgotPassword
{
    using System.ComponentModel.DataAnnotations;

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Please enter a valid email address.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "The email address must be a valid email address, such as example@domain.com")]
        public string EmailAddress { get; set; }

        public ForgotPasswordViewModel() { }

        public ForgotPasswordViewModel(string emailAddress)
        {
            EmailAddress = emailAddress;
        }
    }
}
