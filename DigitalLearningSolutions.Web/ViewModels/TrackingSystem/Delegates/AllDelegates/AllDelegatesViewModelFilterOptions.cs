namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;

    public static class AllDelegatesViewModelFilterOptions
    {
        public static readonly IEnumerable<FilterOptionViewModel> PasswordStatusOptions = new[]
        {
            DelegatePasswordStatusFilterOptions.PasswordSet,
            DelegatePasswordStatusFilterOptions.PasswordNotSet
        };

        public static readonly IEnumerable<FilterOptionViewModel> AdminStatusOptions = new[]
        {
            DelegateAdminStatusFilterOptions.IsAdmin,
            DelegateAdminStatusFilterOptions.IsNotAdmin
        };

        public static readonly IEnumerable<FilterOptionViewModel> ActiveStatusOptions = new[]
        {
            DelegateActiveStatusFilterOptions.IsActive,
            DelegateActiveStatusFilterOptions.IsNotActive
        };

        public static readonly IEnumerable<FilterOptionViewModel> RegistrationTypeOptions = new[]
        {
            DelegateRegistrationTypeFilterOptions.SelfRegistered,
            DelegateRegistrationTypeFilterOptions.SelfRegisteredExternal,
            DelegateRegistrationTypeFilterOptions.RegisteredByCentre
        };

        public static List<FilterViewModel> GetAllDelegatesFilterViewModels(
            IEnumerable<(int id, string name)> jobGroups,
            IEnumerable<CentreRegistrationPrompt> promptsWithOptions
        )
        {
            var filters = new List<FilterViewModel>
            {
                new FilterViewModel("PasswordStatus", "Password Status", PasswordStatusOptions),
                new FilterViewModel("AdminStatus", "Admin Status", AdminStatusOptions),
                new FilterViewModel("ActiveStatus", "Active Status", ActiveStatusOptions),
                new FilterViewModel(
                    "JobGroupId",
                    "Job Group",
                    DelegatesViewModelFilters.GetJobGroupOptions(jobGroups)
                ),
                new FilterViewModel("RegistrationType", "Registration Type", RegistrationTypeOptions)
            };
            filters.AddRange(
                promptsWithOptions.Select(
                    customPrompt => new FilterViewModel(
                        $"CentreRegistrationPrompt{customPrompt.RegistrationField.Id}",
                        customPrompt.PromptText,
                        DelegatesViewModelFilters.GetPromptOptions(customPrompt)
                    )
                )
            );
            return filters;
        }
    }
}
