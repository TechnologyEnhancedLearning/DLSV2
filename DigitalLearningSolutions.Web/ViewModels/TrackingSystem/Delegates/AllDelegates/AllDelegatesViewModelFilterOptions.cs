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
            IEnumerable<CentreRegistrationPrompt> promptsWithOptions
        )
        {
            var filters = new List<FilterModel>
            {
                new FilterModel("PasswordStatus", "Password Status", PasswordStatusOptions,"status"),
                new FilterModel("AdminStatus", "Admin Status", AdminStatusOptions,"status"),
                new FilterModel("ActiveStatus", "Active Status", ActiveStatusOptions,"status"),
                new FilterModel(
                    "JobGroupId",
                    "Job Group",
                    DelegatesViewModelFilters.GetJobGroupOptions(jobGroups)
                ),
                new FilterModel("RegistrationType", "Registration Type", RegistrationTypeOptions,"status"),
                new FilterModel("AccountStatus", "Account Status", AccountStatusOptions,"status"),
                new FilterModel("EmailStatus", "Email Status", EmailStatusOptions,"status"),
            };
            filters.AddRange(
                promptsWithOptions.Select(
                    customPrompt => new FilterModel(
                        $"CentreRegistrationPrompt{customPrompt.RegistrationField.Id}",
                        customPrompt.PromptText,
                        FilteringHelper.GetPromptFilterOptions(customPrompt),"prompts"
                    )
                )
            );
            return filters;
        }
    }
}
