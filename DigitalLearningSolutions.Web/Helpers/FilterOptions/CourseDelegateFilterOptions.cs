namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class CourseDelegateAccountStatusFilterOptions
    {
        private const string Group = "AccountStatus";

        public static readonly FilterOptionViewModel Active = new FilterOptionViewModel(
            "Active",
            Group + FilteringHelper.Separator + nameof(CourseDelegate.Active) + FilteringHelper.Separator + "true",
            FilterStatus.Success
        );

        public static readonly FilterOptionViewModel Inactive = new FilterOptionViewModel(
            "Inactive",
            Group + FilteringHelper.Separator + nameof(CourseDelegate.Active) + FilteringHelper.Separator + "false",
            FilterStatus.Warning
        );
    }

    public static class CourseDelegateProgressLockedFilterOptions
    {
        private const string Group = "ProgressLocked";

        public static readonly FilterOptionViewModel Locked = new FilterOptionViewModel(
            "Locked",
            Group + FilteringHelper.Separator + nameof(CourseDelegate.Locked) + FilteringHelper.Separator + "true",
            FilterStatus.Warning
        );

        public static readonly FilterOptionViewModel NotLocked = new FilterOptionViewModel(
            "Not locked",
            Group + FilteringHelper.Separator + nameof(CourseDelegate.Locked) + FilteringHelper.Separator + "false",
            FilterStatus.Default
        );
    }

    public static class CourseDelegateProgressRemovedFilterOptions
    {
        private const string Group = "ProgressRemoved";

        public static readonly FilterOptionViewModel Removed = new FilterOptionViewModel(
            "Removed",
            Group + FilteringHelper.Separator + nameof(CourseDelegate.Removed) + FilteringHelper.Separator + "true",
            FilterStatus.Warning
        );

        public static readonly FilterOptionViewModel NotRemoved = new FilterOptionViewModel(
            "Not removed",
            Group + FilteringHelper.Separator + nameof(CourseDelegate.Removed) + FilteringHelper.Separator + "false",
            FilterStatus.Default
        );
    }
}
