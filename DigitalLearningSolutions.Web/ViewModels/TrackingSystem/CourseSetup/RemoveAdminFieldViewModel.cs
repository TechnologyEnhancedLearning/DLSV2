namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    public class RemoveAdminFieldViewModel
    {
        public RemoveAdminFieldViewModel() { }

        public RemoveAdminFieldViewModel(string promptName, int answerCount)
        {
            PromptName = promptName;
            AnswerCount = answerCount;
        }

        public string? PromptName { get; set; }

        public bool Confirm { get; set; }

        public int AnswerCount { get; set; }
    }
}
