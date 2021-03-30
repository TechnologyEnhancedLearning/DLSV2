namespace DigitalLearningSolutions.Web.ViewModels.ForgotPassword
{
    public class ForgotPasswordViewModel
    {
        public string? EmailAddress;
        public string? EmailErrorMessage;

        public ForgotPasswordViewModel() { }

        public ForgotPasswordViewModel(string emailAddress, string emailErrorMessage)
        {
            EmailAddress = emailAddress;
            EmailErrorMessage = emailErrorMessage;
        }
    }
}
