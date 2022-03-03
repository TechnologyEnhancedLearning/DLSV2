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
            IEnumerable<CentreRegistrationPrompt> centreRegistrationPrompts
        )
        {
            var promptsWithOptions = centreRegistrationPrompts.Where(customPrompt => customPrompt.Options.Count > 0);
            Delegates = delegateUserCards.Select(
                delegateUser =>
                {
                    var delegateRegistrationPrompts = PromptsService.GetDelegateRegistrationPrompts(delegateUser, centreRegistrationPrompts);
                    return new SearchableDelegateViewModel(delegateUser, delegateRegistrationPrompts, promptsWithOptions, 1);
                }
            );

            Filters = AllDelegatesViewModelFilterOptions.GetAllDelegatesFilterViewModels(jobGroups, promptsWithOptions)
                .SelectAppliedFilterViewModels();
        }
    }
}
