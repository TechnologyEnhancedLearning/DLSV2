namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System;
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

        public string EmailHref => GenerateEmailHref();

        private string GenerateEmailHref()
        {
            var launchCourseLink =
                $"{ConfigHelper.GetAppConfig()[ConfigHelper.AppRootPathName]}/LearningMenu/{CustomisationId}";
            var subject = Uri.EscapeDataString($"Digital Learning Solutions Course Link - {CourseName}");
            var content = Uri.EscapeDataString($"To start your {CourseName} course, go to {launchCourseLink}");
            return $"mailto:?subject={subject}&body={content}";
        }
    }
}
