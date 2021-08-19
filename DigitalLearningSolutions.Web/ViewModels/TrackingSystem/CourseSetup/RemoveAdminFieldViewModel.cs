namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    public class RemoveAdminFieldViewModel
    {
        public RemoveAdminFieldViewModel() { }

        public RemoveAdminFieldViewModel(int customisationId, string promptName, int answerCount)
        {
            CustomisationId = customisationId;
            PromptName = promptName;
            AnswerCount = answerCount;
        }

        public int CustomisationId { get; set; }

        public string? PromptName { get; set; }

        public bool Confirm { get; set; }

        public int AnswerCount { get; set; }
    }
}
