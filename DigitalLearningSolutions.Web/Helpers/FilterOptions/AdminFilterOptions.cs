namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class AdminRoleFilterOptions
    {
        private const string Group = "Role";

        public static readonly FilterOptionViewModel CentreAdministrator = new FilterOptionViewModel(
            "Centre administrator",
            Group + FilteringHelper.Separator + nameof(AdminUser.IsCentreAdmin) + FilteringHelper.Separator + "true",
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel Supervisor = new FilterOptionViewModel(
            "Supervisor",
            Group + FilteringHelper.Separator + nameof(AdminUser.IsSupervisor) + FilteringHelper.Separator + "true",
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel Trainer = new FilterOptionViewModel(
            "Trainer",
            Group + FilteringHelper.Separator + nameof(AdminUser.IsTrainer) + FilteringHelper.Separator + "true",
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel ContentCreatorLicense =
            new FilterOptionViewModel(
                "Content Creator license",
                Group + FilteringHelper.Separator + nameof(AdminUser.IsContentCreator) + FilteringHelper.Separator +
                "true",
                FilterStatus.Default
            );

        public static readonly FilterOptionViewModel CmsAdministrator =
            new FilterOptionViewModel(
                "CMS administrator",
                Group + FilteringHelper.Separator + nameof(AdminUser.IsCmsAdministrator) +
                FilteringHelper.Separator + "true",
                FilterStatus.Default
            );

        public static readonly FilterOptionViewModel CmsManager = new FilterOptionViewModel(
            "CMS manager",
            Group + FilteringHelper.Separator + nameof(AdminUser.IsCmsManager) + FilteringHelper.Separator + "true",
            FilterStatus.Default
        );
    }

    public static class AdminAccountStatusFilterOptions
    {
        private const string Group = "AccountStatus";

        public static readonly FilterOptionViewModel IsLocked = new FilterOptionViewModel(
            "Locked",
            Group + FilteringHelper.Separator + nameof(AdminUser.IsLocked) + FilteringHelper.Separator + "true",
            FilterStatus.Warning
        );

        public static readonly FilterOptionViewModel IsNotLocked = new FilterOptionViewModel(
            "Not locked",
            Group + FilteringHelper.Separator + nameof(AdminUser.IsLocked) + FilteringHelper.Separator + "false",
            FilterStatus.Default
        );
    }
}
