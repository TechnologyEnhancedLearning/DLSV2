namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SelectCourseViewModelFilterOptions
    {
        public static CategoryTopicFilterViewModel[] GetAllCategoriesFilters(
            IEnumerable<string> categories,
            IEnumerable<string> topics,
            string? categoryFilterBy = null,
            string? topicFilterBy = null
        )
        {
            return new[]
            {
                new CategoryTopicFilterViewModel(
                    nameof(ApplicationDetails.CategoryName),
                    "Category",
                    GetCategoryOptions(categories),
                    "categoryFilterBy",
                    categoryFilterBy,
                    topicFilterBy
                ),
                new CategoryTopicFilterViewModel(
                    nameof(ApplicationDetails.CourseTopic),
                    "Topic",
                    GetTopicOptions(topics),
                    "topicFilterBy",
                    categoryFilterBy,
                    topicFilterBy
                ),
            };
        }

        public static IEnumerable<CategoryTopicFilterViewModel> GetSingleCategoryFilters(
            List<ApplicationDetails> courseList,
            string? categoryFilterBy,
            string? topicFilterBy
        )
        {
            var topics = courseList.Select(c => c.CourseTopic);
            return new[]
            {
                new CategoryTopicFilterViewModel(
                    nameof(ApplicationDetails.CourseTopic),
                    "Topic",
                    GetTopicOptions(topics),
                    "topicFilterBy",
                    categoryFilterBy,
                    topicFilterBy
                ),
            };
        }

        private static IEnumerable<FilterOptionViewModel> GetCategoryOptions(IEnumerable<string> categories)
        {
            return categories.Distinct().Select(
                c => new FilterOptionViewModel(
                    c,
                    nameof(ApplicationDetails.CategoryName) + FilteringHelper.Separator +
                    nameof(ApplicationDetails.CategoryName) +
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
                    nameof(ApplicationDetails.CourseTopic) + FilteringHelper.Separator +
                    nameof(ApplicationDetails.CourseTopic) +
                    FilteringHelper.Separator + c,
                    FilterStatus.Default
                )
            );
        }
    }
}
