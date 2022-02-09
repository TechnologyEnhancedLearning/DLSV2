namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class AdminRoleFilterOptions
    {
        private const string Group = "Role";

        public static readonly FilterOptionViewModel CentreAdministrator = new FilterOptionViewModel(
            "Centre administrator",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsCentreAdmin), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel Supervisor = new FilterOptionViewModel(
            "Supervisor",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsSupervisor), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel Trainer = new FilterOptionViewModel(
            "Trainer",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsTrainer), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel ContentCreatorLicense =
            new FilterOptionViewModel(
                "Content Creator license",
                FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsContentCreator), "true"),
                FilterStatus.Default
            );

        public static readonly FilterOptionViewModel CmsAdministrator =
            new FilterOptionViewModel(
                "CMS administrator",
                FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsCmsAdministrator), "true"),
                FilterStatus.Default
            );

        public static readonly FilterOptionViewModel CmsManager = new FilterOptionViewModel(
            "CMS manager",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsCmsManager), "true"),
            FilterStatus.Default
        );
    }

    public static class AdminAccountStatusFilterOptions
    {
        private const string Group = "AccountStatus";

        public static readonly FilterOptionViewModel IsLocked = new FilterOptionViewModel(
            "Locked",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsLocked), "true"),
            FilterStatus.Warning
        );

        public static readonly FilterOptionViewModel IsNotLocked = new FilterOptionViewModel(
            "Not locked",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsLocked), "false"),
            FilterStatus.Default
        );
    }
}
