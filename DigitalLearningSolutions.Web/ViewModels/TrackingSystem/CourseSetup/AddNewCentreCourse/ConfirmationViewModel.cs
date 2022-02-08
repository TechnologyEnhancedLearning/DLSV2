namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    public class ConfirmationViewModel
    {
        public ConfirmationViewModel(int customisationId, string applicationName, string customisationName)
        {
            CustomisationId = customisationId;
            CustomisationName = string.IsNullOrEmpty(customisationName)
                ? applicationName
                : $"{applicationName} - {customisationName}";
        }

        public int CustomisationId { get; set; }
        public string CustomisationName { get; set; }
    }
}
