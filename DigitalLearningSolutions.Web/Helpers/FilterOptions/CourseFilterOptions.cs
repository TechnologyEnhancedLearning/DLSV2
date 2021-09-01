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
            Group + FilteringHelper.Separator + nameof(CourseStatistics.Active) + FilteringHelper.Separator + "false",
            FilterStatus.Warning
        );

        public static readonly FilterOptionViewModel IsActive = new FilterOptionViewModel(
            "Active",
            Group + FilteringHelper.Separator + nameof(CourseStatistics.Active) + FilteringHelper.Separator + "true",
            FilterStatus.Success
        );
    }

    public static class CourseVisibilityFilterOptions
    {
        private const string Group = "Visibility";

        public static readonly FilterOptionViewModel IsHiddenInLearningPortal = new FilterOptionViewModel(
            "Hidden in Learning Portal",
            Group + FilteringHelper.Separator + nameof(CourseStatistics.HideInLearnerPortal) +
            FilteringHelper.Separator + "true",
            FilterStatus.Warning
        );

        public static readonly FilterOptionViewModel IsNotHiddenInLearningPortal = new FilterOptionViewModel(
            "Visible in Learning Portal",
            Group + FilteringHelper.Separator + nameof(CourseStatistics.HideInLearnerPortal) +
            FilteringHelper.Separator + "false",
            FilterStatus.Success
        );
    }
}
