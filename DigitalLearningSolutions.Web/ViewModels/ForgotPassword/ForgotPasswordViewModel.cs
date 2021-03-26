namespace DigitalLearningSolutions.Web.ViewModels.ForgotPassword
{
    public class ForgotPasswordViewModel
    {
        public string? EmailAddress;

        public bool ErrorHasOccurred;

        public string? EmailErrorMessage;

        public ForgotPasswordViewModel() { }

        public ForgotPasswordViewModel(string emailAddress, bool errorHasOccurred, string emailErrorMessage)
        {
            this.EmailAddress = emailAddress;
            this.ErrorHasOccurred = errorHasOccurred;
            this.EmailErrorMessage = emailErrorMessage;
        }
    }
}
