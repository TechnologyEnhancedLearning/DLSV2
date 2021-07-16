namespace DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre
{
    public class ConfirmationViewModel
    {
        public ConfirmationViewModel(string delegateNumber, bool emailWillBeSent, bool passwordSet)
        {
            DelegateNumber = delegateNumber;
            EmailWillBeSent = emailWillBeSent;
            PasswordSet = passwordSet;
        }

        public string DelegateNumber { get; set; }
        public bool EmailWillBeSent { get; set; }
        public bool PasswordSet { get; set; }
    }
}
