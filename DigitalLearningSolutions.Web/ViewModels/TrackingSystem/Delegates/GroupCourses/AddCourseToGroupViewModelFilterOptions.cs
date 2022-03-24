namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class AddCourseToGroupViewModelFilterOptions
    {
        public static FilterModel[] GetAllCategoriesFilters(
            IEnumerable<string> categories,
            IEnumerable<string> topics
        )
        {
            return new[]
            {
                new FilterModel(
                    nameof(CourseAssessmentDetails.CategoryName),
                    "Category",
                    GetCategoryOptions(categories)
                ),
                new FilterModel(
                    nameof(CourseAssessmentDetails.CourseTopic),
                    "Topic",
                    GetTopicOptions(topics)
                ),
            };
        }

        public static IEnumerable<FilterModel> GetSingleCategoryFilters(IEnumerable<CourseAssessmentDetails> courseList)
        {
            var topics = courseList.Select(c => c.CourseTopic);
            return new[]
            {
                new FilterModel(
                    nameof(CourseAssessmentDetails.CourseTopic),
                    "Topic",
                    GetTopicOptions(topics)
                ),
            };
        }

        private static IEnumerable<FilterOptionModel> GetCategoryOptions(IEnumerable<string> categories)
        {
            return categories.Distinct().Select(
                c => new FilterOptionModel(
                    c,
                    nameof(CourseAssessmentDetails.CategoryName) + FilteringHelper.Separator +
                    nameof(CourseAssessmentDetails.CategoryName) +
                    FilteringHelper.Separator + c,
                    FilterStatus.Default
                )
            );
        }

        private static IEnumerable<FilterOptionModel> GetTopicOptions(IEnumerable<string> topics)
        {
            return topics.Distinct().Select(
                c => new FilterOptionModel(
                    c,
                    nameof(CourseAssessmentDetails.CourseTopic) + FilteringHelper.Separator +
                    nameof(CourseAssessmentDetails.CourseTopic) +
                    FilteringHelper.Separator + c,
                    FilterStatus.Default
                )
            );
        }
    }
}
