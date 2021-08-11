namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class AllDelegatesViewModelFilterOptions
    {
        public static readonly IEnumerable<FilterOptionViewModel> PasswordStatusOptions = new[]
        {
            DelegatePasswordStatusFilterOptions.PasswordSet,
            DelegatePasswordStatusFilterOptions.PasswordNotSet
        };
    }
}
