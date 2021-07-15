namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchableCourseStatisticsViewModel : BaseFilterableViewModel
    {
        public SearchableCourseStatisticsViewModel(CourseStatistics courseStatistics)
        {
            CustomisationId = courseStatistics.CustomisationId;
            DelegateCount = courseStatistics.DelegateCount;
            InProgressCount = courseStatistics.InProgressCount;
            CourseName = courseStatistics.CourseName;
            CategoryName = courseStatistics.CategoryName;
            LearningMinutes = courseStatistics.LearningMinutes;
            Tags = FilterableTagHelper.GetCurrentTagsForCourseStatistics(courseStatistics);
        }

        public int CustomisationId { get; set; }
        public int DelegateCount { get; set; }
        public int InProgressCount { get; set; }
        public string CourseName { get; set; }
        public string CategoryName { get; set; }
        public string LearningMinutes { get; set; }
        public string GenerateEmailHref => $"mailto:?subject={GenerateEmailSubject}&body={GenerateEmailContent}";
        private string GenerateEmailSubject => $"Digital Learning Solutions Course Link - {CourseName}";
        private string GenerateEmailContent => $"To start your {CourseName} course, click {LaunchCourseLink}";

        private string LaunchCourseLink =>
            $"{ConfigHelper.GetAppConfig()[ConfigHelper.AppRootPathName]}/LearningMenu/{CustomisationId}";
    }
}
