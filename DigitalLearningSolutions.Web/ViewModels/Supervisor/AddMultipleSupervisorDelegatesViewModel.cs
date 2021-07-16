namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    public class AddMultipleSupervisorDelegatesViewModel
    {
        public AddMultipleSupervisorDelegatesViewModel() { }
        public AddMultipleSupervisorDelegatesViewModel(string? delegateEmails)
        {
            DelegateEmails = delegateEmails;
        }
        public string? DelegateEmails { get; set; }
    }
}
