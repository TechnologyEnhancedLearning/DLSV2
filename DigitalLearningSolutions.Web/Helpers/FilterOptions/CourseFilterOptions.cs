namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class CourseStatusFilterOptions
    {
        private const string Group = "Status";

        public static readonly FilterOptionViewModel IsInactive = new FilterOptionViewModel(
            "Inactive",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseStatistics.Active), "false"),
            FilterStatus.Warning
        );

        public static readonly FilterOptionViewModel IsActive = new FilterOptionViewModel(
            "Active",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseStatistics.Active), "true"),
            FilterStatus.Success
        );
    }

    public static class CourseVisibilityFilterOptions
    {
        private const string Group = "Visibility";

        public static readonly FilterOptionViewModel IsHiddenInLearningPortal = new FilterOptionViewModel(
            "Hidden in Learning Portal",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseStatistics.HideInLearnerPortal), "true"),
            FilterStatus.Warning
        );

        public static readonly FilterOptionViewModel IsNotHiddenInLearningPortal = new FilterOptionViewModel(
            "Visible in Learning Portal",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseStatistics.HideInLearnerPortal), "false"),
            FilterStatus.Success
        );
    }

    public static class CourseHasAdminFieldsFilterOptions
    {
        private const string Group = "HasAdminFields";

        public static readonly FilterOptionViewModel HasAdminFields = new FilterOptionViewModel(
            "Has admin fields",
            FilteringHelper.BuildFilterValueString(
                Group,
                nameof(CourseStatisticsWithAdminFieldResponseCounts.HasAdminFields),
                "true"
            ),
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel DoesNotHaveAdminFields = new FilterOptionViewModel(
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
