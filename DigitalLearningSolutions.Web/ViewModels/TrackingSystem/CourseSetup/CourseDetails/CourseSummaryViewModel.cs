namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails
{
    using System;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class CourseSummaryViewModel
    {
        public CourseSummaryViewModel(CourseDetails courseDetails)
        {
            CurrentVersion = courseDetails.CurrentVersion;
            CreatedTime = courseDetails.CreatedTime;
            LastAccessed = courseDetails.LastAccessed;
            Completions = courseDetails.CompletedCount;
            ActiveLearners = courseDetails.InProgressCount;
        }

        public int CurrentVersion { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? LastAccessed { get; set; }
        public int Completions { get; set; }
        public int ActiveLearners { get; set; }
    }
}
