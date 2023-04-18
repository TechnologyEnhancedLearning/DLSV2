namespace DigitalLearningSolutions.Web.ViewModels.Login
{
    public class AccountInactiveViewModel
    {
        public AccountInactiveViewModel(string supportEmail)
        {
            SupportEmail = supportEmail;
        }

        public string SupportEmail { get; }
    }
}
