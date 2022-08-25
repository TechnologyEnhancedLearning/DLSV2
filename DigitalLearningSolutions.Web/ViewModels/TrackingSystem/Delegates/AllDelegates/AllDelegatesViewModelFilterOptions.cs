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
        public static readonly IEnumerable<FilterOptionModel> RegistrationStatusOptions = new[]
        {
            DelegateRegistrationCompletionStatusFilterOptions.RegistrationComplete,
            DelegateRegistrationCompletionStatusFilterOptions.RegistrationIncomplete,
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

        public static List<FilterModel> GetAllDelegatesFilterViewModels(
            IEnumerable<(int id, string name)> jobGroups,
            IEnumerable<CentreRegistrationPrompt> promptsWithOptions
        )
        {
            var filters = new List<FilterModel>
            {
                new FilterModel(
                    DelegateRegistrationCompletionStatusFilterOptions.Group,
                    "Registration Status",
                    RegistrationStatusOptions
                ),
                new FilterModel(DelegateAdminStatusFilterOptions.Group, "Admin Status", AdminStatusOptions),
                new FilterModel(DelegateActiveStatusFilterOptions.Group, "Active Status", ActiveStatusOptions),
                new FilterModel(
                    "JobGroupId",
                    "Job Group",
                    DelegatesViewModelFilters.GetJobGroupOptions(jobGroups)
                ),
                new FilterModel(
                    DelegateRegistrationTypeFilterOptions.Group,
                    "Registration Type",
                    RegistrationTypeOptions
                ),
            };
            filters.AddRange(
                promptsWithOptions.Select(
                    customPrompt => new FilterModel(
                        $"CentreRegistrationPrompt{customPrompt.RegistrationField.Id}",
                        customPrompt.PromptText,
                        FilteringHelper.GetPromptFilterOptions(customPrompt)
                    )
                )
            );
            return filters;
        }
    }
}
