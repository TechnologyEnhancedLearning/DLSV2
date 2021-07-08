namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class AdminRoleFilterOptions
    {
        private const string Category = "Role";

        public static readonly FilterOptionViewModel CentreAdministrator = new FilterOptionViewModel(
            "Centre administrator",
            Category + FilteringHelper.Separator + nameof(AdminUser.IsCentreAdmin) + FilteringHelper.Separator + "true",
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel Supervisor = new FilterOptionViewModel(
            "Supervisor",
            Category + FilteringHelper.Separator + nameof(AdminUser.IsSupervisor) + FilteringHelper.Separator + "true",
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel Trainer = new FilterOptionViewModel(
            "Trainer",
            Category + FilteringHelper.Separator + nameof(AdminUser.IsTrainer) + FilteringHelper.Separator + "true",
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel ContentCreatorLicense =
            new FilterOptionViewModel(
                "Content Creator license",
                Category + FilteringHelper.Separator + nameof(AdminUser.IsContentCreator) + FilteringHelper.Separator +
                "true",
                FilterStatus.Default
            );

        public static readonly FilterOptionViewModel CmsAdministrator =
            new FilterOptionViewModel(
                "CMS administrator",
                Category + FilteringHelper.Separator + nameof(AdminUser.IsCmsAdministrator) +
                FilteringHelper.Separator + "true",
                FilterStatus.Default
            );

        public static readonly FilterOptionViewModel CmsManager = new FilterOptionViewModel(
            "CMS manager",
            Category + FilteringHelper.Separator + nameof(AdminUser.IsCmsManager) + FilteringHelper.Separator + "true",
            FilterStatus.Default
        );
    }

    public static class AdminAccountStatusFilterOptions
    {
        private const string Category = "AccountStatus";

        public static readonly FilterOptionViewModel IsLocked = new FilterOptionViewModel(
            "Locked",
            Category + FilteringHelper.Separator + nameof(AdminUser.IsLocked) + FilteringHelper.Separator + "true",
            FilterStatus.Warning
        );

        public static readonly FilterOptionViewModel IsNotLocked = new FilterOptionViewModel(
            "Not locked",
            Category + FilteringHelper.Separator + nameof(AdminUser.IsLocked) + FilteringHelper.Separator + "false",
            FilterStatus.Default
        );
    }
}
