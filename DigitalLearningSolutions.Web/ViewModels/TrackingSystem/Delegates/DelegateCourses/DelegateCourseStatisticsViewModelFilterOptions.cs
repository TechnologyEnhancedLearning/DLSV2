namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateCourses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class DelegateCourseStatisticsViewModelFilterOptions
    {
        private static readonly IEnumerable<FilterOptionModel> CourseStatusOptions = new[]
        {
            CourseStatusFilterOptions.IsActive,
            CourseStatusFilterOptions.IsInactive,
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
                new FilterModel(nameof(CourseStatistics.Active), "Status", CourseStatusOptions),
                new FilterModel(
                    nameof(CourseStatisticsWithAdminFieldResponseCounts.HasAdminFields),
                    "Admin fields",
                    CourseHasAdminFieldOptions
                ),
            };

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
