namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using DigitalLearningSolutions.Data.Models.Courses;

    public class CourseSummaryViewModel
    {
        public CourseSummaryViewModel(CourseDetails courseDetails)
        {
            CurrentVersion = courseDetails.CurrentVersion;
            CreatedTime = courseDetails.CreatedTime.ToString("dd/MM/yyyy");
            LastAccessed = courseDetails.LastAccessed?.ToString("dd/MM/yyyy");
            Completions = courseDetails.CompletedCount;
            ActiveLearners = courseDetails.InProgressCount;
        }

        public int CurrentVersion { get; set; }
        public string CreatedTime { get; set; }
        public string? LastAccessed { get; set; }
        public int Completions { get; set; }
        public int ActiveLearners { get; set; }
    }
}
