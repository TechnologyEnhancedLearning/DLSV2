namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration
{
    public class RemoveRegistrationPromptViewModel
    {
        public RemoveRegistrationPromptViewModel() {}

        public RemoveRegistrationPromptViewModel(string promptName, int delegateCount)
        {
            PromptName = promptName;
            DelegateCount = delegateCount;
        }

        public string? PromptName { get; set; }

        public bool Confirm { get; set; }

        public int DelegateCount { get; set; }
    }
}
