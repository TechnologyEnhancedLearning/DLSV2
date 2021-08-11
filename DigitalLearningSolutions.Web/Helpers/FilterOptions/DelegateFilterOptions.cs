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
            FilterStatus.Warning
        );

        public static readonly FilterOptionViewModel PasswordNotSet = new FilterOptionViewModel(
            "Password not set",
            Group + FilteringHelper.Separator + nameof(DelegateUserCard.IsPasswordSet) + FilteringHelper.Separator +
            "false",
            FilterStatus.Default
        );
    }
}
