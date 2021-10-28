namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Attributes;

    public class EditAutoRefreshOptionsViewModel
    {
        public EditAutoRefreshOptionsViewModel() { }

        public EditAutoRefreshOptionsViewModel(CourseDetails courseDetails)
        {
            CustomisationId = courseDetails.CustomisationId;
            RefreshToCustomisationId = courseDetails.RefreshToCustomisationId;
            AutoRefreshMonths = courseDetails.AutoRefreshMonths.ToString();
            ApplyLpDefaultsToSelfEnrol = courseDetails.ApplyLpDefaultsToSelfEnrol;
        }

        public EditAutoRefreshOptionsViewModel(
            int customisationId,
            int refreshToCustomisationId,
            string? autoRefreshMonths,
            bool applyLpDefaultsToSelfEnrol
            )
        {
            CustomisationId = customisationId;
            RefreshToCustomisationId = refreshToCustomisationId;
            AutoRefreshMonths = autoRefreshMonths;
            ApplyLpDefaultsToSelfEnrol = applyLpDefaultsToSelfEnrol;
        }

        public int CustomisationId { get; set; }
        public int RefreshToCustomisationId { get; set; }

        [WholeNumberWithinInclusiveRange(0, 12, "Enter a whole number from 0 to 12")]
        public string? AutoRefreshMonths { get; set; }

        public bool ApplyLpDefaultsToSelfEnrol { get; set; }
    }
}
