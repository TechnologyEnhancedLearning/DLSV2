namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;

    public static class AllDelegatesViewModelFilterOptions
    {
        public static readonly IEnumerable<FilterOptionModel> PasswordStatusOptions = new[]
        {
            DelegatePasswordStatusFilterOptions.PasswordSet,
            DelegatePasswordStatusFilterOptions.PasswordNotSet,
        };

        public static readonly IEnumerable<FilterOptionModel> AdminStatusOptions = new[]
        {
            DelegateAdminStatusFilterOptions.IsAdmin,
            DelegateAdminStatusFilterOptions.IsNotAdmin,
        };

        public static readonly IEnumerable<FilterOptionModel> ActiveStatusOptions = new[]
        {
            DelegateActiveStatusFilterOptions.IsActive,
            DelegateActiveStatusFilterOptions.IsNotActive,
        };

        public static readonly IEnumerable<FilterOptionModel> RegistrationTypeOptions = new[]
        {
            DelegateRegistrationTypeFilterOptions.SelfRegistered,
            DelegateRegistrationTypeFilterOptions.SelfRegisteredExternal,
            DelegateRegistrationTypeFilterOptions.RegisteredByCentre,
        };

        public static readonly IEnumerable<FilterOptionModel> AccountStatusOptions = new[]
        {
            AccountStatusFilterOptions.ClaimedAccount,
            AccountStatusFilterOptions.UnclaimedAccount
        };

        public static readonly IEnumerable<FilterOptionModel> EmailStatusOptions = new[]
        {
            EmailStatusFilterOptions.VerifiedAccount,
            EmailStatusFilterOptions.UnverifiedAccount
        };

        public static List<FilterModel> GetAllDelegatesFilterViewModels(
            IEnumerable<(int id, string name)> jobGroups,
            IEnumerable<CentreRegistrationPrompt> promptsWithOptions,
            IEnumerable<(int id, string name)> groups
        )
        {
            var filters = new List<FilterModel>
            {
                new FilterModel("PasswordStatus", "Password status", PasswordStatusOptions,"status"),
                new FilterModel("AdminStatus", "Admin status", AdminStatusOptions,"status"),
                new FilterModel("ActiveStatus", "Active status", ActiveStatusOptions,"status"),
                new FilterModel("JobGroupId","Job Group",DelegatesViewModelFilters.GetJobGroupOptions(jobGroups)),
                new FilterModel("RegistrationType", "Registration type", RegistrationTypeOptions,"status"),
                new FilterModel("AccountStatus", "Account status", AccountStatusOptions,"status"),
                new FilterModel("EmailStatus", "Email status", EmailStatusOptions,"status"),
            };
            filters.AddRange(
                promptsWithOptions.Select(
                    customPrompt => new FilterModel(
                        $"CentreRegistrationPrompt{customPrompt.RegistrationField.Id}",
                        "Prompt: " + customPrompt.PromptText,
                        FilteringHelper.GetPromptFilterOptions(customPrompt), "prompts/groups"
                    )
                )
            );
            filters.Add(new FilterModel("GroupId", "Groups", DelegatesViewModelFilters.GetGroupOptions(groups), "prompts/groups"));
            return filters;
        }
    }
}
