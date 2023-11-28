namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateCourses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;

    public static class DelegateCourseStatisticsViewModelFilterOptions
    {
        private static readonly IEnumerable<FilterOptionModel> CourseStatusOptions = new[]
        {
            CourseStatusFilterOptions.IsActive,
            CourseStatusFilterOptions.NotActive,
        };

        private static readonly IEnumerable<FilterOptionModel> CourseHasAdminFieldOptions = new[]
        {
            CourseHasAdminFieldsFilterOptions.HasAdminFields,
            CourseHasAdminFieldsFilterOptions.DoesNotHaveAdminFields,
        };

        private static readonly IEnumerable<FilterOptionModel> ActivityTypeOptions = new[]
        {
            ActivityTypeFilterOptions.IsCourse,
            ActivityTypeFilterOptions.IsSelfAssessment,
        };

        public static IEnumerable<FilterModel> GetFilterOptions(
            IEnumerable<string> categories,
            IEnumerable<string> topics
        )
        {
            var filterOptions = new List<FilterModel>();
            filterOptions.Add(new FilterModel("Course", "Type", ActivityTypeOptions));
            if (categories.Any())
            {
                filterOptions.Add(
                    new FilterModel(nameof(CourseStatistics.CategoryName), "Category",
                        GetCategoryOptions(categories)
                    ));
            }
            filterOptions.Add(new FilterModel(nameof(CourseStatistics.CourseTopic), "Topic", GetTopicOptions(topics)));
            filterOptions.Add(new FilterModel(nameof(CourseStatistics.Active), "Status", CourseStatusOptions));
            filterOptions.Add(new FilterModel(
                    nameof(CourseStatisticsWithAdminFieldResponseCounts.HasAdminFields),
                    "Admin fields",
                    CourseHasAdminFieldOptions
                ));

            return filterOptions;
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
