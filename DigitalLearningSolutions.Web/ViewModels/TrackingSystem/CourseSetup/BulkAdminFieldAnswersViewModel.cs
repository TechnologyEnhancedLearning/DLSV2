namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    public class BulkAdminFieldAnswersViewModel
    {
        public BulkAdminFieldAnswersViewModel() { }

        public BulkAdminFieldAnswersViewModel(
            string? optionsString,
            bool isAddPromptJourney,
            int customisationId,
            int promptNumber
        )
        {
            OptionsString = optionsString;
            IsAddPromptJourney = isAddPromptJourney;
            CustomisationId = customisationId;
            PromptNumber = promptNumber;
        }

        public string? OptionsString { get; set; }

        public bool IsAddPromptJourney { get; set; }

        public int CustomisationId { get; set; }

        public int PromptNumber { get; set; }
    }
}
