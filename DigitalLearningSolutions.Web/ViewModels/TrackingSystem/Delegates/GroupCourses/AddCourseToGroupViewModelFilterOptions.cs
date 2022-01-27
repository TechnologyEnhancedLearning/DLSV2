namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AddCourseToGroupViewModelFilterOptions
    {
        public static FilterViewModel[] GetAllCategoriesFilters(IEnumerable<string> categories, IEnumerable<string> topics)
        {
            return new[]
            {
                new FilterViewModel(
                    nameof(CourseAssessmentDetails.CategoryName),
                    "Category",
                    AddCourseToGroupViewModelFilterOptions.GetCategoryOptions(categories)
                ),
                new FilterViewModel(
                    nameof(CourseAssessmentDetails.CourseTopic),
                    "Topic",
                    AddCourseToGroupViewModelFilterOptions.GetTopicOptions(topics)
                ),
            };
        }

        public static IEnumerable<FilterViewModel> GetSingleCategoryFilters(List<CourseAssessmentDetails> courseList)
        {
            var topics = courseList.Select(c => c.CourseTopic);
            return new[]
            {
                new FilterViewModel(
                    nameof(CourseAssessmentDetails.CourseTopic),
                    "Topic",
                    AddCourseToGroupViewModelFilterOptions.GetTopicOptions(topics)
                ),
            };
        }

        private static IEnumerable<FilterOptionViewModel> GetCategoryOptions(IEnumerable<string> categories)
        {
            var test = categories.Distinct();
            return test.Select(
                c => new FilterOptionViewModel(
                    c,
                    nameof(CourseAssessmentDetails.CategoryName) + FilteringHelper.Separator +
                    nameof(CourseAssessmentDetails.CategoryName) +
                    FilteringHelper.Separator + c,
                    FilterStatus.Default
                )
            );
        }

        private static IEnumerable<FilterOptionViewModel> GetTopicOptions(IEnumerable<string> topics)
        {
            return topics.Distinct().Select(
                c => new FilterOptionViewModel(
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
