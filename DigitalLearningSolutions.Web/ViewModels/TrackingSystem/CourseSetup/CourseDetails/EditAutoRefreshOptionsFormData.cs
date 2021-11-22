namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Attributes;

    public class EditAutoRefreshOptionsFormData
    {
        public EditAutoRefreshOptionsFormData() { }

        protected EditAutoRefreshOptionsFormData(EditAutoRefreshOptionsFormData formData)
        {
            RefreshToCustomisationId = formData.RefreshToCustomisationId;
            AutoRefreshMonths = formData.AutoRefreshMonths;
            ApplyLpDefaultsToSelfEnrol = formData.ApplyLpDefaultsToSelfEnrol;
        }

        protected EditAutoRefreshOptionsFormData(CourseDetails courseDetails)
        {
            RefreshToCustomisationId = courseDetails.RefreshToCustomisationId;
            AutoRefreshMonths = courseDetails.AutoRefreshMonths.ToString();
            ApplyLpDefaultsToSelfEnrol = courseDetails.ApplyLpDefaultsToSelfEnrol;
        }

        [Required(ErrorMessage = "Select a course")]
        public int? RefreshToCustomisationId { get; set; }

        [WholeNumberWithinInclusiveRange(0, 12, "Enter a whole number from 0 to 12")]
        public string? AutoRefreshMonths { get; set; }

        public bool ApplyLpDefaultsToSelfEnrol { get; set; }
    }
}
