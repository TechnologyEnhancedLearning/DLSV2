namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class CourseDelegateAccountStatusFilterOptions
    {
        private const string Group = "AccountStatus";

        public static readonly FilterOptionViewModel Active = new FilterOptionViewModel(
            "Active",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseDelegate.Active), "true"),
            FilterStatus.Success
        );

        public static readonly FilterOptionViewModel Inactive = new FilterOptionViewModel(
            "Inactive",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseDelegate.Active), "false"),
            FilterStatus.Warning
        );
    }

    public static class CourseDelegateProgressLockedFilterOptions
    {
        private const string Group = "ProgressLocked";

        public static readonly FilterOptionViewModel Locked = new FilterOptionViewModel(
            "Locked",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseDelegate.Locked), "true"),
            FilterStatus.Warning
        );

        public static readonly FilterOptionViewModel NotLocked = new FilterOptionViewModel(
            "Not locked",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseDelegate.Locked), "false"),
            FilterStatus.Default
        );
    }

    public static class CourseDelegateProgressRemovedFilterOptions
    {
        private const string Group = "ProgressRemoved";

        public static readonly FilterOptionViewModel Removed = new FilterOptionViewModel(
            "Removed",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseDelegate.Removed), "true"),
            FilterStatus.Warning
        );

        public static readonly FilterOptionViewModel NotRemoved = new FilterOptionViewModel(
            "Not removed",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseDelegate.Removed), "false"),
            FilterStatus.Default
        );
    }
}
