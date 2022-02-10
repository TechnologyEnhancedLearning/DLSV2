namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchableCourseStatisticsViewModel : BaseFilterableViewModel
    {
        public SearchableCourseStatisticsViewModel(CourseStatisticsWithAdminFieldResponseCounts courseStatistics)
        {
            CustomisationId = courseStatistics.CustomisationId;
            DelegateCount = courseStatistics.DelegateCount;
            InProgressCount = courseStatistics.InProgressCount;
            CourseName = courseStatistics.CourseName;
            CategoryName = courseStatistics.CategoryName;
            CourseTopic = courseStatistics.CourseTopic;
            LearningMinutes = courseStatistics.LearningMinutes;
            Tags = FilterableTagHelper.GetCurrentTagsForCourseStatistics(courseStatistics);
            Assessed = courseStatistics.IsAssessed;
            AdminFieldWithResponseCounts = courseStatistics.AdminFieldsWithResponses;
        }

        public int CustomisationId { get; set; }
        public int DelegateCount { get; set; }
        public int InProgressCount { get; set; }
        public string CourseName { get; set; }
        public string CategoryName { get; set; }
        public string CourseTopic { get; set; }
        public string LearningMinutes { get; set; }
        public bool Assessed { get; set; }

        public IEnumerable<CustomPromptWithResponseCounts> AdminFieldWithResponseCounts { get; set; }
        public bool HasAdminFields => AdminFieldWithResponseCounts.Any();

        public string CategoryFilter => nameof(CourseStatistics.CategoryName) + FilteringHelper.Separator +
                                        nameof(CourseStatistics.CategoryName) +
                                        FilteringHelper.Separator + CategoryName;

        public string TopicFilter => nameof(CourseStatistics.CourseTopic) + FilteringHelper.Separator +
                                     nameof(CourseStatistics.CourseTopic) +
                                     FilteringHelper.Separator + CourseTopic;

        public string HasAdminFieldsFilter => nameof(CourseStatisticsWithAdminFieldResponseCounts.HasAdminFields) +
                                              FilteringHelper.Separator +
                                              nameof(CourseStatisticsWithAdminFieldResponseCounts.HasAdminFields) +
                                              FilteringHelper.Separator + HasAdminFields.ToString().ToLowerInvariant();

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
