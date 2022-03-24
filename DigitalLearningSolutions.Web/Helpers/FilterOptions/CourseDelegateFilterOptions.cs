namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class CourseDelegateAccountStatusFilterOptions
    {
        private const string Group = "AccountStatus";

        public static readonly FilterOptionModel Active = new FilterOptionModel(
            "Active",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseDelegate.Active), "true"),
            FilterStatus.Success
        );

        public static readonly FilterOptionModel Inactive = new FilterOptionModel(
            "Inactive",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseDelegate.Active), "false"),
            FilterStatus.Warning
        );
    }

    public static class CourseDelegateProgressLockedFilterOptions
    {
        private const string Group = "ProgressLocked";

        public static readonly FilterOptionModel Locked = new FilterOptionModel(
            "Locked",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseDelegate.Locked), "true"),
            FilterStatus.Warning
        );

        public static readonly FilterOptionModel NotLocked = new FilterOptionModel(
            "Not locked",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseDelegate.Locked), "false"),
            FilterStatus.Default
        );
    }

    public static class CourseDelegateProgressRemovedFilterOptions
    {
        private const string Group = "ProgressRemoved";

        public static readonly FilterOptionModel Removed = new FilterOptionModel(
            "Removed",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseDelegate.Removed), "true"),
            FilterStatus.Warning
        );

        public static readonly FilterOptionModel NotRemoved = new FilterOptionModel(
            "Not removed",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseDelegate.Removed), "false"),
            FilterStatus.Default
        );
    }
}
