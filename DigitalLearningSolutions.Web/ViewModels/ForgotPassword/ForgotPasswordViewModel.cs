namespace DigitalLearningSolutions.Web.ViewModels.ForgotPassword
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Helpers;

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Enter your email")]
        [MaxLength(255, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        public string EmailAddress { get; set; }

        public ForgotPasswordViewModel() { }

        public ForgotPasswordViewModel(string emailAddress)
        {
            EmailAddress = emailAddress;
        }
    }
}
