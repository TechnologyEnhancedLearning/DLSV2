namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllDelegatesViewModel : BaseSearchablePageViewModel
    {
        public AllDelegatesViewModel(
            int centreId,
            IEnumerable<DelegateUserCard> delegateUserCards,
            int page,
            CustomPromptHelper customPromptHelper
        ) : base(null, DefaultSortByOptions.Name.PropertyName, Ascending, page)
        {
            CentreId = centreId;

            var delegateUsers = delegateUserCards.ToList();
            MatchingSearchResults = delegateUsers.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(delegateUsers);

            Delegates = paginatedItems.Select(
                delegateUser =>
                {
                    var customFields = customPromptHelper.GetCustomFieldViewModelsForCentre(centreId, delegateUser);
                    var delegateInfoViewModel = new DelegateInfoViewModel(delegateUser, customFields);
                    return new SearchableDelegateViewModel(delegateInfoViewModel);
                }
            );
        }

        public int CentreId { get; set; }
        public IEnumerable<SearchableDelegateViewModel> Delegates { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name
        };
    }
}
