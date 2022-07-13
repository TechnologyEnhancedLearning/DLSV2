namespace DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre
{
    public class ConfirmationViewModel
    {
        public ConfirmationViewModel(string delegateNumber)
        {
            DelegateNumber = delegateNumber;
        }

        public string DelegateNumber { get; set; }
    }
}
