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
            IReadOnlyCollection<string> categories,
            IReadOnlyCollection<string> topics
        )
        {
            var filterModels = new List<FilterModel>();

            if (categories.Count > 1)
            {
                filterModels.Add(
                    new FilterModel(
                        nameof(ApplicationWithSections.CategoryName),
                        "Category",
                        GetCategoryOptions(categories)
                    )
                );
            }

            if (topics.Count > 1)
            {
                filterModels.Add(
                    new FilterModel(
                        nameof(ApplicationWithSections.CourseTopic),
                        "Topic",
                        GetTopicOptions(topics)
                    )
                );
            }

            return filterModels;
        }

        private static IEnumerable<FilterOptionModel> GetCategoryOptions(IEnumerable<string> categories)
        {
            return categories.Select(
                category => new FilterOptionModel(
                    category,
                    FilteringHelper.BuildFilterValueString(
                        nameof(ApplicationWithSections.CategoryName),
                        nameof(ApplicationWithSections.CategoryName),
                        category
                    ),
                    FilterStatus.Default
                )
            );
        }

        private static IEnumerable<FilterOptionModel> GetTopicOptions(IEnumerable<string> topics)
        {
            return topics.Select(
                topic => new FilterOptionModel(
                    topic,
                    FilteringHelper.BuildFilterValueString(
                        nameof(ApplicationWithSections.CourseTopic),
                        nameof(ApplicationWithSections.CourseTopic),
                        topic
                    ),
                    FilterStatus.Default
                )
            );
        }
    }
}
