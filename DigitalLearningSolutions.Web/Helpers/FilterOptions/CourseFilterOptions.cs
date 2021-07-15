namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class CourseActiveStatusFilterOptions
    {
        private const string Group = "ActiveStatus";

        public static readonly FilterOptionViewModel IsInactive = new FilterOptionViewModel(
            "Inactive",
            Group + FilteringHelper.Separator + nameof(CourseStatistics.Active) + FilteringHelper.Separator + "false",
            FilterStatus.Warning
        );

        public static readonly FilterOptionViewModel IsActive = new FilterOptionViewModel(
            "Active",
            Group + FilteringHelper.Separator + nameof(CourseStatistics.Active) + FilteringHelper.Separator + "true",
            FilterStatus.Success
        );
    }

    public static class CourseHiddenInLearningPortalStatusFilterOptions
    {
        private const string Group = "HiddenInLearningPortalStatus";

        public static readonly FilterOptionViewModel IsHidden = new FilterOptionViewModel(
            "Hidden in Learning Portal",
            Group + FilteringHelper.Separator + nameof(CourseStatistics.HideInLearnerPortal) +
            FilteringHelper.Separator + "true",
            FilterStatus.Warning
        );

        public static readonly FilterOptionViewModel IsNotHidden = new FilterOptionViewModel(
            "Visible in Learning Portal",
            Group + FilteringHelper.Separator + nameof(CourseStatistics.HideInLearnerPortal) +
            FilteringHelper.Separator + "true",
            FilterStatus.Success
        );
    }
}
