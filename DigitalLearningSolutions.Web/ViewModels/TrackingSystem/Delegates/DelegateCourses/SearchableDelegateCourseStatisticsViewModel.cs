namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateCourses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchableDelegateCourseStatisticsViewModel : BaseFilterableViewModel
    {
        public SearchableDelegateCourseStatisticsViewModel(CourseStatisticsWithAdminFieldResponseCounts courseStatistics)
        {
            CustomisationId = courseStatistics.CustomisationId;
            DelegateCount = courseStatistics.DelegateCount;
            InProgressCount = courseStatistics.InProgressCount;
            CourseName = courseStatistics.CourseName;
            CategoryName = courseStatistics.CategoryName;
            CourseTopic = courseStatistics.CourseTopic;
            LearningMinutes = courseStatistics.LearningMinutes;
            Tags = FilterableTagHelper.GetCurrentTagsForDelegateCourses(courseStatistics);
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
    }
}
