namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration;

    public static class AddRegistrationPromptMappingHelper
    {
        public static AddRegistrationPromptSummaryViewModel MapToSummary(AddRegistrationPromptData data, string promptName)
        {
            return new AddRegistrationPromptSummaryViewModel
                (promptName, data.SelectPromptViewModel.Mandatory, data.ConfigureAnswersViewModel.OptionsString);
        }
    }
}
