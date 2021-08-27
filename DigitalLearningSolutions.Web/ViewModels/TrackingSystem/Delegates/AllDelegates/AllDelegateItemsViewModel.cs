namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllDelegateItemsViewModel : BaseJavaScriptFilterableViewModel
    {
        public readonly IEnumerable<SearchableDelegateViewModel> Delegates;

        public AllDelegateItemsViewModel(
            IEnumerable<SearchableDelegateViewModel> delegates,
            IEnumerable<(int id, string name)> jobGroups,
            IEnumerable<CustomPrompt> closedCustomPrompts
        )
        {
            Delegates = delegates;

            var filters = new List<FilterViewModel>
            {
                new FilterViewModel(
                    "PasswordStatus",
                    "Password Status",
                    AllDelegatesViewModelFilterOptions.PasswordStatusOptions
                ),
                new FilterViewModel(
                    "AdminStatus",
                    "Admin Status",
                    AllDelegatesViewModelFilterOptions.AdminStatusOptions
                ),
                new FilterViewModel(
                    "ActiveStatus",
                    "Active Status",
                    AllDelegatesViewModelFilterOptions.ActiveStatusOptions
                ),
                new FilterViewModel(
                    "JobGroupId",
                    "Job Group",
                    AllDelegatesViewModelFilterOptions.GetJobGroupOptions(jobGroups)
                ),
                new FilterViewModel(
                    "RegistrationType",
                    "Registration Type",
                    AllDelegatesViewModelFilterOptions.RegistrationTypeOptions
                )
            };
            filters.AddRange(
                closedCustomPrompts.Select(
                    customPrompt => new FilterViewModel(
                        customPrompt.CustomPromptText,
                        customPrompt.CustomPromptText,
                        AllDelegatesViewModelFilterOptions.GetCustomPromptOptions(customPrompt)
                    )
                )
            );

            Filters = filters.SelectAppliedFilterViewModels();
        }
    }
}
