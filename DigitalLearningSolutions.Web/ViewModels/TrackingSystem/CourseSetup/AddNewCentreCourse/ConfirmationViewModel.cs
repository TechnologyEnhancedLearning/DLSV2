namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    public class ConfirmationViewModel
    {
        public ConfirmationViewModel(int customisationId, string customisationName)
        {
            CustomisationId = customisationId;
            CustomisationName = customisationName;
        }

        public int CustomisationId { get; set; }
        public string CustomisationName { get; set; }
    }
}
