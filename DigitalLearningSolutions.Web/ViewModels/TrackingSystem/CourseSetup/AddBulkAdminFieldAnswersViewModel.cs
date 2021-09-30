namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    public class AddBulkAdminFieldAnswersViewModel : BulkAdminFieldAnswersViewModel
    {
        public AddBulkAdminFieldAnswersViewModel() { }

        public AddBulkAdminFieldAnswersViewModel(
            string? optionsString
        )
        {
            OptionsString = optionsString;
        }

        public AddBulkAdminFieldAnswersViewModel(
            string? optionsString,
            int promptNumber
        )
        {
            OptionsString = optionsString;
            PromptNumber = promptNumber;
        }

        private int PromptNumber { get; set; }
    }
}
