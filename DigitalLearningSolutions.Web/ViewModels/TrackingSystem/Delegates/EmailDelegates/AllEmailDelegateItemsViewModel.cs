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
            IEnumerable<CentreRegistrationPrompt> customPrompts,
            IEnumerable<int> selectedDelegateIds
        )
        {
            var promptsWithOptions = customPrompts.Where(customPrompt => customPrompt.Options.Count > 0);
            Delegates = delegateUserCards.Select(
                delegateUser =>
                {
                    var isDelegateSelected = selectedDelegateIds.Contains(delegateUser.Id);
                    var customFields = CentreRegistrationPromptHelper.GetDelegateRegistrationPrompts(delegateUser, customPrompts);
                    return new EmailDelegatesItemViewModel(delegateUser, isDelegateSelected, customFields, promptsWithOptions);
                }
            );

            Filters = EmailDelegatesViewModelFilterOptions
                .GetEmailDelegatesFilterViewModels(jobGroups, promptsWithOptions)
                .SelectAppliedFilterViewModels();
        }
    }
}
