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
}
