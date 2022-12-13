namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public static class CourseStatusFilterOptions
    {
        private const string Group = "Status";

        public static readonly FilterOptionModel IsInactive = new FilterOptionModel(
            "Inactive/archived",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseStatistics.Active), "false"),
            FilterStatus.Warning
        );

        //public static readonly FilterOptionModel IsActive = new FilterOptionModel(
        //    "Active",
        //    FilteringHelper.BuildFilterValueString(Group, nameof(CourseStatistics.Active), "true"),
        //    FilterStatus.Success
        //);

        public const char FilterSeparator = '╡';
        
        public static FilterOptionModel IsActive
        {
            get
            {
                var activeFilterValue = FilteringHelper.BuildFilterValueString(
                    "Active",
                    nameof(CourseStatistics.Active),
                    "true"
                );

                var notArchivedFilterValue = FilteringHelper.BuildFilterValueString(
                    "Archived",
                    nameof(CourseStatistics.Archived),
                    "false"
                );

                var filterValue = activeFilterValue + FilterSeparator + notArchivedFilterValue;

                return new FilterOptionModel(
                    "Active",
                    filterValue,
                    FilterStatus.Success
                );
            }
        }

        //public static readonly FilterOptionModel IsArchived = new FilterOptionModel(
        //    "Archived",
        //    FilteringHelper.BuildFilterValueString(Group, nameof(CourseStatistics.Archived), "true"),
        //    FilterStatus.Default
        //);

        public static FilterOptionModel IsArchived
        {
            get
            {
                var filterValue = FilteringHelper.BuildFilterValueString(
                    "Archived",
                    nameof(CourseStatistics.Archived),
                    "true"
                );

                //TODO: Also need to OR with .Active=false
                //var filterValue = FilteringHelper.BuildFilterValueString(
                //    "Archived",
                //    nameof(CourseStatistics.Archived),
                //    "true"
                //);

                return new FilterOptionModel(
                    Group,
                    filterValue,
                    FilterStatus.Success
                );
            }
        }
    }

    public static class CourseVisibilityFilterOptions
    {
        private const string Group = "Visibility";

        public static readonly FilterOptionModel IsHiddenInLearningPortal = new FilterOptionModel(
            "Hidden in Learning Portal",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseStatistics.HideInLearnerPortal), "true"),
            FilterStatus.Warning
        );

        public static readonly FilterOptionModel IsNotHiddenInLearningPortal = new FilterOptionModel(
            "Visible in Learning Portal",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseStatistics.HideInLearnerPortal), "false"),
            FilterStatus.Success
        );
    }

    public static class CourseHasAdminFieldsFilterOptions
    {
        private const string Group = "HasAdminFields";

        public static readonly FilterOptionModel HasAdminFields = new FilterOptionModel(
            "Has admin fields",
            FilteringHelper.BuildFilterValueString(
                Group,
                nameof(CourseStatisticsWithAdminFieldResponseCounts.HasAdminFields),
                "true"
            ),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel DoesNotHaveAdminFields = new FilterOptionModel(
            "Doesn't have admin fields",
            FilteringHelper.BuildFilterValueString(
                Group,
                nameof(CourseStatisticsWithAdminFieldResponseCounts.HasAdminFields),
                "false"
            ),
            FilterStatus.Default
        );
    }
}
