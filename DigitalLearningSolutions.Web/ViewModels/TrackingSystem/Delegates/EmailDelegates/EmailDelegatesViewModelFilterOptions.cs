namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;

    public static class EmailDelegatesViewModelFilterOptions
    {
        public static List<FilterViewModel> GetEmailDelegatesFilterViewModels(
            IEnumerable<(int id, string name)> jobGroups,
            IEnumerable<CustomPrompt> promptsWithOptions
        )
        {
            var filters = new List<FilterViewModel>
            {
                new FilterViewModel(
                    "JobGroupId",
                    "Job Group",
                    DelegatesViewModelFilters.GetJobGroupOptions(jobGroups)
                )
            };
            filters.AddRange(
                promptsWithOptions.Select(
                    customPrompt => new FilterViewModel(
                        $"CustomPrompt{customPrompt.RegistrationField.Id}",
                        customPrompt.CustomPromptText,
                        DelegatesViewModelFilters.GetCustomPromptOptions(customPrompt)
                    )
                )
            );
            return filters;
        }
    }
}
