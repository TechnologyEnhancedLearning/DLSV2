namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    public class BulkAdminFieldAnswersViewModel
    {
        public BulkAdminFieldAnswersViewModel() { }

        public BulkAdminFieldAnswersViewModel(string? optionsString, bool isAddPromptJourney, int? promptNumber)
        {
            OptionsString = optionsString;
            IsAddPromptJourney = isAddPromptJourney;
            PromptNumber = promptNumber;
        }

        public string? OptionsString { get; set; }

        public bool IsAddPromptJourney { get; set; }

        public int? PromptNumber { get; set; }
    }
}
