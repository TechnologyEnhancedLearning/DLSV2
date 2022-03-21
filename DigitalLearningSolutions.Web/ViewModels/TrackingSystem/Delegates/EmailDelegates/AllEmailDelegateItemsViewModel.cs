namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllEmailDelegateItemsViewModel : BaseJavaScriptFilterableViewModel
    {
        public readonly IEnumerable<EmailDelegatesItemViewModel> Delegates;

        public AllEmailDelegateItemsViewModel(
            IEnumerable<DelegateUserCard> delegateUserCards,
            IEnumerable<(int id, string name)> jobGroups,
            IEnumerable<CentreRegistrationPrompt> centreRegistrationPrompts,
            IEnumerable<int> selectedDelegateIds
        )
        {
            Delegates = delegateUserCards.Select(
                delegateUser =>
                {
                    var isDelegateSelected = selectedDelegateIds.Contains(delegateUser.Id);
                    var delegateRegistrationPrompts = PromptsService.GetDelegateRegistrationPrompts(delegateUser, centreRegistrationPrompts);
                    return new EmailDelegatesItemViewModel(delegateUser, isDelegateSelected, delegateRegistrationPrompts, centreRegistrationPrompts);
                }
            );

            Filters = EmailDelegatesViewModelFilterOptions
                .GetEmailDelegatesFilterModels(jobGroups, centreRegistrationPrompts)
                .SelectAppliedFilterViewModels();
        }
    }
}
