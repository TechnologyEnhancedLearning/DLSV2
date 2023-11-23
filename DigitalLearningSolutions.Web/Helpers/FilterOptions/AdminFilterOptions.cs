namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;

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
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminEntity.IsCentreAdmin), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel Supervisor = new FilterOptionModel(
            "Supervisor",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminEntity.IsSupervisor), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel NominatedSupervisor = new FilterOptionModel(
            "Nominated supervisor",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminEntity.IsNominatedSupervisor), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel Trainer = new FilterOptionModel(
            "Trainer",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminEntity.IsTrainer), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel ContentCreatorLicense =
            new FilterOptionModel(
                "Content Creator license",
                FilteringHelper.BuildFilterValueString(Group, nameof(AdminEntity.IsContentCreator), "true"),
                FilterStatus.Default
            );

        public static readonly FilterOptionModel CmsAdministrator =
            new FilterOptionModel(
                "CMS administrator",
                FilteringHelper.BuildFilterValueString(Group, nameof(AdminEntity.IsCmsAdministrator), "true"),
                FilterStatus.Default
            );

        public static readonly FilterOptionModel CmsManager = new FilterOptionModel(
            "CMS manager",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminEntity.IsCmsManager), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel SuperAdmin = new FilterOptionModel(
            "Super admin",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminEntity.IsSuperAdmin), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel ReportsViewer = new FilterOptionModel(
            "Report viewer",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminEntity.IsReportsViewer), "true"),
            FilterStatus.Default
        );
    }

    public static class AdminAccountStatusFilterOptions
    {
        private const string Group = "AccountStatus";

        public static readonly FilterOptionModel IsLocked = new FilterOptionModel(
            "Locked",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminEntity.IsLocked), "true"),
            FilterStatus.Warning
        );

        public static readonly FilterOptionModel IsNotLocked = new FilterOptionModel(
            "Not locked",
            FilteringHelper.BuildFilterValueString(Group, nameof(AdminEntity.IsLocked), "false"),
            FilterStatus.Default
        );
    }
}
