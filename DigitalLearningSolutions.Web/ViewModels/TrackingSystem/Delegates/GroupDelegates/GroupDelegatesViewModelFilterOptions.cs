namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;

    public static class GroupDelegatesViewModelFilterOptions
    {
        public static List<FilterModel> GetAddGroupDelegateFilterViewModels(
            IEnumerable<(int id, string name)> jobGroups,
            IEnumerable<CentreRegistrationPrompt> promptsWithOptions
        )
        {
            var filters = new List<FilterModel>
            {
                new FilterModel(
                    "JobGroupId",
                    "Job Group",
                    DelegatesViewModelFilters.GetJobGroupOptions(jobGroups)
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
