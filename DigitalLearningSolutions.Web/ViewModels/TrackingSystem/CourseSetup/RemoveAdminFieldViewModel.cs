namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    public class RemoveAdminFieldViewModel
    {
        public RemoveAdminFieldViewModel() { }

        public RemoveAdminFieldViewModel(int customisationId, string promptName, int delegateCount)
        {
            CustomisationId = customisationId;
            PromptName = promptName;
            DelegateCount = delegateCount;
        }

        public int CustomisationId { get; set; }

        public string? PromptName { get; set; }

        public bool Confirm { get; set; }

        public int DelegateCount { get; set; }
    }
}
