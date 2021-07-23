namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Data.Models.Courses;

    public class LearningPathwayDefaultsViewModel
    {
        public LearningPathwayDefaultsViewModel(CourseDetails courseDetails)
        {
            CustomisationId = courseDetails.CustomisationId;
            CompleteWithinMonths = courseDetails.CompleteWithinMonths;
            CompletionValidFor = courseDetails.ValidityMonths;
            Mandatory = courseDetails.Mandatory;
            AutoRefresh = courseDetails.AutoRefresh;
            RefreshToCustomisationId = courseDetails.RefreshToCustomisationId;
            RefreshToCourseName = courseDetails.RefreshToCourseName;
            AutoRefreshMonths = courseDetails.AutoRefreshMonths;
            ApplyLpDefaultsToSelfEnrol = courseDetails.ApplyLpDefaultsToSelfEnrol;
        }

        public int CustomisationId { get; set; }
        public int CompleteWithinMonths { get; set; }
        public int CompletionValidFor { get; set; }
        public bool Mandatory { get; set; }
        public bool AutoRefresh { get; set; }
        public int RefreshToCustomisationId { get; set; }
        public string? RefreshToCourseName { get; set; }
        public int AutoRefreshMonths { get; set; }
        public bool ApplyLpDefaultsToSelfEnrol { get; set; }
    }
}
