namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;

    public class CourseSummaryViewModel
    {
        public CourseSummaryViewModel(CourseDetails courseDetails)
        {
            CurrentVersion = courseDetails.CurrentVersion;
            CreatedDate = courseDetails.CreatedDate.ToString(DateHelper.StandardDateFormat);
            LastAccessed = courseDetails.LastAccessed?.ToString(DateHelper.StandardDateFormat);
            Completions = courseDetails.CompletedCount;
            ActiveLearners = courseDetails.InProgressCount;
        }

        public int CurrentVersion { get; set; }
        public string CreatedDate { get; set; }
        public string? LastAccessed { get; set; }
        public int Completions { get; set; }
        public int ActiveLearners { get; set; }
    }
}
