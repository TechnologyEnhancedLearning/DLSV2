namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllDelegateItemsViewModel : BaseJavaScriptFilterableViewModel
    {
        public readonly IEnumerable<SearchableDelegateViewModel> Delegates;

        public AllDelegateItemsViewModel(
            IEnumerable<DelegateUserCard> delegateUserCards,
            IEnumerable<(int id, string name)> jobGroups,
            IEnumerable<CustomPrompt> customPrompts
        )
        {
            var closedCustomPrompts = customPrompts.Where(customPrompt => customPrompt.Options.Count > 0);
            Delegates = delegateUserCards.Select(
                delegateUser =>
                {
                    var customFields = CentreCustomPromptHelper.GetCustomFieldViewModels(delegateUser, customPrompts);
                    return new SearchableDelegateViewModel(delegateUser, customFields, closedCustomPrompts);
                }
            );

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
