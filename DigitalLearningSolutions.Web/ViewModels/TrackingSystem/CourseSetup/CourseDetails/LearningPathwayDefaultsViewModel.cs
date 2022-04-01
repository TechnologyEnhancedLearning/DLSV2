namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class LearningPathwayDefaultsViewModel
    {
        public LearningPathwayDefaultsViewModel(CourseDetails courseDetails)
        {
            CustomisationId = courseDetails.CustomisationId;
            CompleteWithinMonths = DisplayStringHelper.ConvertNumberToMonthsString(courseDetails.CompleteWithinMonths);
            CompletionValidFor = DisplayStringHelper.ConvertNumberToMonthsString(courseDetails.ValidityMonths);
            Mandatory = courseDetails.Mandatory;
            AutoRefresh = courseDetails.AutoRefresh;
            RefreshToCustomisationId = courseDetails.RefreshToCustomisationId;
            RefreshToCourseName = courseDetails.RefreshToCourseName;
            AutoRefreshMonths = SetMonthDisplayString(courseDetails.AutoRefreshMonths) + " before expiry";
            ApplyLpDefaultsToSelfEnrol = courseDetails.ApplyLpDefaultsToSelfEnrol;
        }

        public int CustomisationId { get; set; }
        public string? CompleteWithinMonths { get; set; }
        public string? CompletionValidFor { get; set; }
        public bool Mandatory { get; set; }
        public bool AutoRefresh { get; set; }
        public int RefreshToCustomisationId { get; set; }
        public string? RefreshToCourseName { get; set; }
        public string AutoRefreshMonths { get; set; }
        public bool ApplyLpDefaultsToSelfEnrol { get; set; }

        private string SetMonthDisplayString(int numberOfMonths)
        {
            return numberOfMonths + (numberOfMonths == 1 ? " month" : " months");
        }
    }
}
