namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class DelegateRegistrationCompletionStatusFilterOptions
    {
        public const string Group = "RegistrationStatus";

        public static readonly FilterOptionModel RegistrationComplete = new FilterOptionModel(
            "Registration complete",
            FilteringHelper.BuildFilterValueString(Group, nameof(DelegateUserCard.IsRegistrationComplete), "true"),
            FilterStatus.Success
        );

        public static readonly FilterOptionModel RegistrationIncomplete = new FilterOptionModel(
            "Registration incomplete",
            FilteringHelper.BuildFilterValueString(Group, nameof(DelegateUserCard.IsRegistrationComplete), "false"),
            FilterStatus.Warning
        );
    }

    public static class DelegateAdminStatusFilterOptions
    {
        public const string Group = "AdminStatus";

        public static readonly FilterOptionModel IsAdmin = new FilterOptionModel(
            "Admin",
            FilteringHelper.BuildFilterValueString(Group, nameof(DelegateUserCard.IsAdmin), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel IsNotAdmin = new FilterOptionModel(
            "Not admin",
            FilteringHelper.BuildFilterValueString(Group, nameof(DelegateUserCard.IsAdmin), "false"),
            FilterStatus.Default
        );
    }

    public static class DelegateActiveStatusFilterOptions
    {
        public const string Group = "ActiveStatus";

        public static readonly FilterOptionModel IsActive = new FilterOptionModel(
            "Active",
            FilteringHelper.BuildFilterValueString(Group, nameof(DelegateUserCard.Active), "true"),
            FilterStatus.Success
        );

        public static readonly FilterOptionModel IsNotActive = new FilterOptionModel(
            "Inactive",
            FilteringHelper.BuildFilterValueString(Group, nameof(DelegateUserCard.Active), "false"),
            FilterStatus.Warning
        );
    }

    public static class DelegateRegistrationTypeFilterOptions
    {
        public const string Group = "RegistrationType";

        public static readonly FilterOptionModel SelfRegistered = new FilterOptionModel(
            RegistrationType.SelfRegistered.DisplayText,
            FilteringHelper.BuildFilterValueString(Group, Group, nameof(RegistrationType.SelfRegistered)),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel SelfRegisteredExternal = new FilterOptionModel(
            RegistrationType.SelfRegisteredExternal.DisplayText,
            FilteringHelper.BuildFilterValueString(Group, Group, nameof(RegistrationType.SelfRegisteredExternal)),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel RegisteredByCentre = new FilterOptionModel(
            RegistrationType.RegisteredByCentre.DisplayText,
            FilteringHelper.BuildFilterValueString(Group, Group, nameof(RegistrationType.RegisteredByCentre)),
            FilterStatus.Default
        );

        public static FilterOptionModel FromRegistrationType(RegistrationType registrationType)
        {
            return Equals(registrationType, RegistrationType.SelfRegisteredExternal)
                ? SelfRegisteredExternal
                : Equals(registrationType, RegistrationType.SelfRegistered)
                    ? SelfRegistered
                    : RegisteredByCentre;
        }
    }
}
