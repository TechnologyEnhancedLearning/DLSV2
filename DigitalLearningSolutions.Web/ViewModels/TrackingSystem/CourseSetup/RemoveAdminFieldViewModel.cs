namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    public class RemoveAdminFieldViewModel
    {
        public RemoveAdminFieldViewModel() { }

        public RemoveAdminFieldViewModel(int customisationId, string promptName, int adminCount)
        {
            CustomisationId = customisationId;
            PromptName = promptName;
            AdminCount = adminCount;
        }

        public int CustomisationId { get; set; }

        public string? PromptName { get; set; }

        public bool Confirm { get; set; }

        public int AdminCount { get; set; }
    }
}
