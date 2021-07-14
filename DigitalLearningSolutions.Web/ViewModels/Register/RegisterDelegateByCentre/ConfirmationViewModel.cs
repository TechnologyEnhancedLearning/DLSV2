namespace DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre
{
    public class ConfirmationViewModel
    {
        public ConfirmationViewModel(string delegateNumber, bool emailSent, bool passwordSet)
        {
            DelegateNumber = delegateNumber;
            EmailSent = emailSent;
            PasswordSet = passwordSet;
        }

        public string DelegateNumber { get; set; }
        public bool EmailSent { get; set; }
        public bool PasswordSet { get; set; }
    }
}
