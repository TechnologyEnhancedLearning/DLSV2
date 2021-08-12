namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;

    public class AllDelegateItemsViewModel
    {
        public readonly IEnumerable<SearchableDelegateViewModel> Delegates;

        public AllDelegateItemsViewModel(
            int centreId,
            IEnumerable<DelegateUserCard> delegateUserCards,
            CustomPromptHelper customPromptHelper
        )
        {
            Delegates = delegateUserCards.Select(
                delegateUser =>
                {
                    var customFields = customPromptHelper.GetCustomFieldViewModelsForCentre(centreId, delegateUser);
                    return new SearchableDelegateViewModel(delegateUser, customFields);
                }
            );
        }
    }
}
