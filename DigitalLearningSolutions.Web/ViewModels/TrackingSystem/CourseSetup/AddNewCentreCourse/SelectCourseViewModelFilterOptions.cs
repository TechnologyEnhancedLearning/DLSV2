namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class SelectCourseViewModelFilterOptions
    {
        public static CategoryTopicFilterViewModel[] GetAllCategoriesFilters(
            IEnumerable<string> categories,
            IEnumerable<string> topics,
            string? categoryFilterString = null,
            string? topicFilterString = null
        )
        {
            return new[]
            {
                new CategoryTopicFilterViewModel(
                    nameof(ApplicationDetails.CategoryName),
                    "Category",
                    GetCategoryOptions(categories),
                    "categoryFilterString",
                    categoryFilterString,
                    topicFilterString
                ),
                new CategoryTopicFilterViewModel(
                    nameof(ApplicationDetails.CourseTopic),
                    "Topic",
                    GetTopicOptions(topics),
                    "topicFilterString",
                    categoryFilterString,
                    topicFilterString
                ),
            };
        }

        public static IEnumerable<CategoryTopicFilterViewModel> GetSingleCategoryFilters(
            List<ApplicationDetails> courseList,
            string? categoryFilterString,
            string? topicFilterString
        )
        {
            var topics = courseList.Select(c => c.CourseTopic);
            return new[]
            {
                new CategoryTopicFilterViewModel(
                    nameof(ApplicationDetails.CourseTopic),
                    "Topic",
                    GetTopicOptions(topics),
                    "topicFilterString",
                    categoryFilterString,
                    topicFilterString
                ),
            };
        }

        private static IEnumerable<FilterOptionModel> GetCategoryOptions(IEnumerable<string> categories)
        {
            return categories.Distinct().Select(
                c => new FilterOptionModel(
                    c,
                    nameof(ApplicationDetails.CategoryName) + FilteringHelper.Separator +
                    nameof(ApplicationDetails.CategoryName) +
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
                    nameof(ApplicationDetails.CourseTopic) + FilteringHelper.Separator +
                    nameof(ApplicationDetails.CourseTopic) +
                    FilteringHelper.Separator + c,
                    FilterStatus.Default
                )
            );
        }
    }
}
