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
            CourseStatusFilterOptions.IsInactive,
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
            return new[]
            {
                new FilterModel(
                    nameof(CourseStatistics.CategoryName),
                    "Category",
                    GetCategoryOptions(categories)
                ),
                new FilterModel(
                    nameof(CourseStatistics.CourseTopic),
                    "Topic",
                    GetTopicOptions(topics)
                ),
                new FilterModel(nameof(CourseStatistics.Active), "Status", CourseStatusOptions),
                new FilterModel(
                    nameof(CourseStatistics.HideInLearnerPortal),
                    "Visibility",
                    CourseVisibilityOptions
                ),
                new FilterModel(
                    nameof(CourseStatisticsWithAdminFieldResponseCounts.HasAdminFields),
                    "Admin fields",
                    CourseHasAdminFieldOptions
                ),
            };
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
