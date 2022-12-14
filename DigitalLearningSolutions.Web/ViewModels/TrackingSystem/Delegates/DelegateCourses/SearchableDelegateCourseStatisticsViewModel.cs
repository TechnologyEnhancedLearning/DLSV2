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
            InProgressCount = courseStatistics.InProgressCount;
            CompletedCount = courseStatistics.CompletedCount;
            CourseName = courseStatistics.CourseName;
            CategoryName = courseStatistics.CategoryName;
            CourseTopic = courseStatistics.CourseTopic;
            LearningMinutes = courseStatistics.LearningMinutes;
            Tags = FilterableTagHelper.GetCurrentTagsForDelegateCourses(courseStatistics);
            Assessed = courseStatistics.IsAssessed;
            AdminFieldWithResponseCounts = courseStatistics.AdminFieldsWithResponses;
            Status = DeriveCourseStatus(courseStatistics);
        }

        public int CustomisationId { get; set; }
        public int InProgressCount { get; set; }
        public int CompletedCount { get; set; }
        public string CourseName { get; set; }
        public string CategoryName { get; set; }
        public string CourseTopic { get; set; }
        public string LearningMinutes { get; set; }
        public bool Assessed { get; set; }
        public string? Status { get; set; }

        public IEnumerable<CourseAdminFieldWithResponseCounts> AdminFieldWithResponseCounts { get; set; }

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
        private static string DeriveCourseStatus(Course courseStatistics)
        {
            if (courseStatistics.Archived)
            {
                return "inactive/archived";
            }
            else switch (courseStatistics.Active)
            {
                case true:
                    return "active";
                case false:
                    return "inactive";
                default:
            }
        }
    }
}
