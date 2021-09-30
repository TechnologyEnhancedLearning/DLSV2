namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    public class BulkAdminFieldAnswersViewModel
    {
        public BulkAdminFieldAnswersViewModel() { }

        public BulkAdminFieldAnswersViewModel(
            string? optionsString
        )
        {
            OptionsString = optionsString;
        }

        public string? OptionsString { get; set; }
    }
}
