﻿namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class DelegatePasswordStatusFilterOptions
    {
        private const string Group = "PasswordStatus";

        public static readonly FilterOptionViewModel PasswordSet = new FilterOptionViewModel(
            "Password set",
            FilteringHelper.BuildFilterValueString(Group, nameof(DelegateUserCard.IsPasswordSet), "true"),
            FilterStatus.Success
        );

        public static readonly FilterOptionViewModel PasswordNotSet = new FilterOptionViewModel(
            "Password not set",
            FilteringHelper.BuildFilterValueString(Group, nameof(DelegateUserCard.IsPasswordSet), "false"),
            FilterStatus.Warning
        );
    }

    public static class DelegateAdminStatusFilterOptions
    {
        private const string Group = "AdminStatus";

        public static readonly FilterOptionViewModel IsAdmin = new FilterOptionViewModel(
            "Admin",
            FilteringHelper.BuildFilterValueString(Group, nameof(DelegateUserCard.IsAdmin), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel IsNotAdmin = new FilterOptionViewModel(
            "Not admin",
            FilteringHelper.BuildFilterValueString(Group, nameof(DelegateUserCard.IsAdmin), "false"),
            FilterStatus.Default
        );
    }

    public static class DelegateActiveStatusFilterOptions
    {
        private const string Group = "ActiveStatus";

        public static readonly FilterOptionViewModel IsActive = new FilterOptionViewModel(
            "Active",
            FilteringHelper.BuildFilterValueString(Group, nameof(DelegateUserCard.Active), "true"),
            FilterStatus.Success
        );

        public static readonly FilterOptionViewModel IsNotActive = new FilterOptionViewModel(
            "Inactive",
            FilteringHelper.BuildFilterValueString(Group, nameof(DelegateUserCard.Active), "false"),
            FilterStatus.Warning
        );
    }

    public static class DelegateRegistrationTypeFilterOptions
    {
        private const string Group = "RegistrationType";

        public static readonly FilterOptionViewModel SelfRegistered = new FilterOptionViewModel(
            RegistrationType.SelfRegistered.DisplayText,
            FilteringHelper.BuildFilterValueString(Group, Group, nameof(RegistrationType.SelfRegistered)),
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel SelfRegisteredExternal = new FilterOptionViewModel(
            RegistrationType.SelfRegisteredExternal.DisplayText,
            FilteringHelper.BuildFilterValueString(Group, Group, nameof(RegistrationType.SelfRegisteredExternal)),
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel RegisteredByCentre = new FilterOptionViewModel(
            RegistrationType.RegisteredByCentre.DisplayText,
            FilteringHelper.BuildFilterValueString(Group, Group, nameof(RegistrationType.RegisteredByCentre)),
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
