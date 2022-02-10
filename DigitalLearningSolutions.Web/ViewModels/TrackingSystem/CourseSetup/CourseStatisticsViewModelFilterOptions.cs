namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class CourseStatisticsViewModelFilterOptions
    {
        private static readonly IEnumerable<FilterOptionViewModel> CourseStatusOptions = new[]
        {
            CourseStatusFilterOptions.IsActive,
            CourseStatusFilterOptions.IsInactive,
        };

        private static readonly IEnumerable<FilterOptionViewModel> CourseVisibilityOptions = new[]
        {
            CourseVisibilityFilterOptions.IsHiddenInLearningPortal,
            CourseVisibilityFilterOptions.IsNotHiddenInLearningPortal,
        };

        private static readonly IEnumerable<FilterOptionViewModel> CourseHasAdminFieldOptions = new[]
        {
            CourseHasAdminFieldsFilterOptions.HasAdminFields,
            CourseHasAdminFieldsFilterOptions.DoesNotHaveAdminFields,
        };

        public static IEnumerable<FilterViewModel> GetFilterOptions(
            IEnumerable<string> categories,
            IEnumerable<string> topics
        )
        {
            return new[]
            {
                new FilterViewModel(
                    nameof(CourseStatistics.CategoryName),
                    "Category",
                    GetCategoryOptions(categories)
                ),
                new FilterViewModel(
                    nameof(CourseStatistics.CourseTopic),
                    "Topic",
                    GetTopicOptions(topics)
                ),
                new FilterViewModel(nameof(CourseStatistics.Active), "Status", CourseStatusOptions),
                new FilterViewModel(
                    nameof(CourseStatistics.HideInLearnerPortal),
                    "Visibility",
                    CourseVisibilityOptions
                ),
                new FilterViewModel(
                    nameof(CourseStatisticsWithAdminFieldResponseCounts.HasAdminFields),
                    "Admin fields",
                    CourseHasAdminFieldOptions
                ),
            };
        }

        private static IEnumerable<FilterOptionViewModel> GetCategoryOptions(IEnumerable<string> categories)
        {
            return categories.Select(
                category => new FilterOptionViewModel(
                    category,
                    nameof(CourseStatistics.CategoryName) + FilteringHelper.Separator +
                    nameof(CourseStatistics.CategoryName) +
                    FilteringHelper.Separator + category,
                    FilterStatus.Default
                )
            );
        }

        private static IEnumerable<FilterOptionViewModel> GetTopicOptions(IEnumerable<string> topics)
        {
            return topics.Select(
                topic => new FilterOptionViewModel(
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
