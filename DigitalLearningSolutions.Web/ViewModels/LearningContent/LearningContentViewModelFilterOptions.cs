namespace DigitalLearningSolutions.Web.ViewModels.LearningContent
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public static class LearningContentViewModelFilterOptions
    {
        public static IEnumerable<FilterModel> GetFilterOptions(
            IEnumerable<string> categories,
            IEnumerable<string> topics
        )
        {
            return new[]
            {
                new FilterModel(
                    nameof(ApplicationWithSections.CategoryName),
                    "Category",
                    GetCategoryOptions(categories)
                ),
                new FilterModel(
                    nameof(ApplicationWithSections.CourseTopic),
                    "Topic",
                    GetTopicOptions(topics)
                ),
            };
        }

        private static IEnumerable<FilterOptionModel> GetCategoryOptions(IEnumerable<string> categories)
        {
            return categories.Select(
                category => new FilterOptionModel(
                    category,
                    nameof(ApplicationWithSections.CategoryName) + FilteringHelper.Separator +
                    nameof(ApplicationWithSections.CategoryName) +
                    FilteringHelper.Separator + category,
                    FilterStatus.Default
                )
            );
        }

        private static IEnumerable<FilterOptionModel> GetTopicOptions(IEnumerable<string> topics)
        {
            return topics.Select(
                topic => new FilterOptionModel(
                    topic,
                    nameof(ApplicationWithSections.CourseTopic) + FilteringHelper.Separator +
                    nameof(ApplicationWithSections.CourseTopic) +
                    FilteringHelper.Separator + topic,
                    FilterStatus.Default
                )
            );
        }
    }
}
