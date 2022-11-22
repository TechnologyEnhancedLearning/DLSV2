namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class AdminRoleFilterOptions
    {
        private const string Group = "Role";

        public static readonly FilterOptionModel CentreManager = new FilterOptionModel(
            "Centre manager",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsCentreManager), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel CentreAdministrator = new FilterOptionModel(
            "Centre administrator",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsCentreAdmin), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel Supervisor = new FilterOptionModel(
            "Supervisor",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsSupervisor), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel NominatedSupervisor = new FilterOptionModel(
            "Nominated supervisor",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsNominatedSupervisor), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel Trainer = new FilterOptionModel(
            "Trainer",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsTrainer), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel ContentCreatorLicense =
            new FilterOptionModel(
                "Content creator license",
                FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsContentCreator), "true"),
                FilterStatus.Default
            );

        public static readonly FilterOptionModel CmsAdministrator =
            new FilterOptionModel(
                "CMS administrator",
                FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsCmsAdministrator), "true"),
                FilterStatus.Default
            );

        public static readonly FilterOptionModel CmsManager = new FilterOptionModel(
            "CMS manager",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsCmsManager), "true"),
            FilterStatus.Default
        );
    }

    public static class AdminAccountStatusFilterOptions
    {
        private const string Group = "AccountStatus";

        public static readonly FilterOptionModel IsLocked = new FilterOptionModel(
            "Locked",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsLocked), "true"),
            FilterStatus.Warning
        );

        public static readonly FilterOptionModel IsNotLocked = new FilterOptionModel(
            "Not locked",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminUser.IsLocked), "false"),
            FilterStatus.Default
        );
    }
}
