namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;

    public static class CourseStatisticsViewModelFilterOptions
    {
        private static readonly IEnumerable<FilterOptionModel> CourseStatusOptions = new[]
        {
            CourseStatusFilterOptions.IsActive,
            CourseStatusFilterOptions.NotActive
        };

        private static readonly IEnumerable<FilterOptionModel> CourseVisibilityOptions = new[]
        {
            CourseVisibilityFilterOptions.IsHiddenInLearningPortal,
            CourseVisibilityFilterOptions.IsNotHiddenInLearningPortal,
        };

        private static readonly IEnumerable<FilterOptionModel> CourseHasAdminFieldOptions = new[]
        {
            CourseHasAdminFieldsFilterOptions.HasAdminFields,
            CourseHasAdminFieldsFilterOptions.DoesNotHaveAdminFields,
        };

        public static IEnumerable<FilterModel> GetFilterOptions(
            IEnumerable<string> categories,
            IEnumerable<string> topics
        )
        {
            var filterOptions = new[]
            {
                new FilterModel(
                    nameof(CourseStatistics.CourseTopic),
                    "Topic",
                    GetTopicOptions(topics)
                ),
                new FilterModel(nameof(CourseStatistics.Active), "Active status", CourseStatusOptions,"course status"),
                new FilterModel(
                    nameof(CourseStatistics.HideInLearnerPortal),
                    "Visibility",
                    CourseVisibilityOptions,
                    "course status"
                ),
                new FilterModel(
                    nameof(CourseStatisticsWithAdminFieldResponseCounts.HasAdminFields),
                    "Admin field status",
                    CourseHasAdminFieldOptions,
                    "course status"
                ),
            };

            categories = categories.ToList();

            return categories.Any()
                ? filterOptions.Prepend(
                    new FilterModel(
                        nameof(CourseStatistics.CategoryName),
                        "Category",
                        GetCategoryOptions(categories)
                    )
                )
                : filterOptions;
        }

        private static IEnumerable<FilterOptionModel> GetCategoryOptions(IEnumerable<string> categories)
        {
            return categories.Select(
                category => new FilterOptionModel(
                    category,
                    nameof(CourseStatistics.CategoryName) + FilteringHelper.Separator +
                    nameof(CourseStatistics.CategoryName) +
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
                    nameof(CourseStatistics.CourseTopic) + FilteringHelper.Separator +
                    nameof(CourseStatistics.CourseTopic) +
                    FilteringHelper.Separator + topic,
                    FilterStatus.Default
                )
            );
        }
    }
}
