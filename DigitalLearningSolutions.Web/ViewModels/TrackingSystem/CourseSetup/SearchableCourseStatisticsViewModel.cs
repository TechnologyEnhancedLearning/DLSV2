namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.Models.Courses;

    public class SearchableCourseStatisticsViewModel
    {
        public SearchableCourseStatisticsViewModel(CourseStatistics courseStatistics)
        {
            CustomisationId = courseStatistics.CustomisationId;
            CentreId = courseStatistics.CentreId;
            Active = courseStatistics.Active;
            DelegateCount = courseStatistics.DelegateCount;
            CompletedCount = courseStatistics.CompletedCount;
            InProgressCount = courseStatistics.InProgressCount;
            PassRate = courseStatistics.PassRate;
            CourseName = courseStatistics.CourseName;
        }

        public int CustomisationId { get; set; }
        public int CentreId { get; set; }
        public bool Active { get; set; }
        public int DelegateCount { get; set; }
        public int CompletedCount { get; set; }
        public int InProgressCount { get; set; }
        public string CourseName { get; set; }
        public double PassRate { get; set; }
    }
}
