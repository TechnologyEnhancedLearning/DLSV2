namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration
{
    public class BulkRegistrationPromptAnswersViewModel
    {
        public BulkRegistrationPromptAnswersViewModel(){}

        public BulkRegistrationPromptAnswersViewModel(string? optionsString, bool isAddPromptJourney, int? promptNumber)
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
