namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class DelegatePasswordStatusFilterOptions
    {
        private const string Group = "PasswordStatus";

        public static readonly FilterOptionViewModel PasswordSet = new FilterOptionViewModel(
            "Password set",
            Group + FilteringHelper.Separator + nameof(DelegateUserCard.IsPasswordSet) + FilteringHelper.Separator +
            "true",
            FilterStatus.Success
        );

        public static readonly FilterOptionViewModel PasswordNotSet = new FilterOptionViewModel(
            "Password not set",
            Group + FilteringHelper.Separator + nameof(DelegateUserCard.IsPasswordSet) + FilteringHelper.Separator +
            "false",
            FilterStatus.Warning
        );
    }

    public static class DelegateAdminStatusFilterOptions
    {
        private const string Group = "AdminStatus";

        public static readonly FilterOptionViewModel IsAdmin = new FilterOptionViewModel(
            "Admin",
            Group + FilteringHelper.Separator + nameof(DelegateUserCard.IsAdmin) + FilteringHelper.Separator +
            "true",
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel IsNotAdmin = new FilterOptionViewModel(
            "Not admin",
            Group + FilteringHelper.Separator + nameof(DelegateUserCard.IsAdmin) + FilteringHelper.Separator +
            "false",
            FilterStatus.Default
        );
    }

    public static class DelegateActiveStatusFilterOptions
    {
        private const string Group = "ActiveStatus";

        public static readonly FilterOptionViewModel IsActive = new FilterOptionViewModel(
            "Active",
            Group + FilteringHelper.Separator + nameof(DelegateUserCard.Active) + FilteringHelper.Separator +
            "true",
            FilterStatus.Success
        );

        public static readonly FilterOptionViewModel IsNotActive = new FilterOptionViewModel(
            "Inactive",
            Group + FilteringHelper.Separator + nameof(DelegateUserCard.Active) + FilteringHelper.Separator +
            "false",
            FilterStatus.Warning
        );
    }

    public static class DelegateRegistrationTypeFilterOptions
    {
        private const string Group = "RegistrationType";

        public static readonly FilterOptionViewModel SelfRegistered = new FilterOptionViewModel(
            RegistrationType.SelfRegistered.DisplayText,
            Group + FilteringHelper.Separator + Group + FilteringHelper.Separator +
            nameof(RegistrationType.SelfRegistered),
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel SelfRegisteredExternal = new FilterOptionViewModel(
            RegistrationType.SelfRegisteredExternal.DisplayText,
            Group + FilteringHelper.Separator + Group + FilteringHelper.Separator +
            nameof(RegistrationType.SelfRegisteredExternal),
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel RegisteredByCentre = new FilterOptionViewModel(
            RegistrationType.RegisteredByCentre.DisplayText,
            Group + FilteringHelper.Separator + Group + FilteringHelper.Separator +
            nameof(RegistrationType.RegisteredByCentre),
            FilterStatus.Default
        );

        public static FilterOptionViewModel FromRegistrationType(RegistrationType registrationType)
        {
            return Equals(registrationType, RegistrationType.SelfRegisteredExternal)
                ? SelfRegisteredExternal
                : Equals(registrationType, RegistrationType.SelfRegistered)
                    ? SelfRegistered
                    : RegisteredByCentre;
        }
    }
}
