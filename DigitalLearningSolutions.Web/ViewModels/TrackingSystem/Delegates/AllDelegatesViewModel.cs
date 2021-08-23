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
            IEnumerable<(int id, string name)> jobGroups,
            CentreCustomPromptHelper centreCustomPromptHelper,
            int page,
            string? searchString,
            string sortBy,
            string sortDirection,
            string? filterBy
        ) : base(searchString, page, true, sortBy, sortDirection, filterBy)
        {
            CentreId = centreId;

            var sortedItems = GenericSortingHelper.SortAllItems(
                delegateUserCards.AsQueryable(),
                sortBy,
                sortDirection
            );
            var searchedItems = GenericSearchHelper.SearchItems(sortedItems, SearchString).ToList();
            var filteredItems = FilteringHelper.FilterItems(searchedItems.AsQueryable(), filterBy).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(filteredItems);

            var closedCustomPrompts = centreCustomPromptHelper.GetClosedCustomPromptsForCentre(centreId);
            Delegates = paginatedItems.Select(
                delegateUser =>
                {
                    var customFields =
                        centreCustomPromptHelper.GetCustomFieldViewModelsForCentre(centreId, delegateUser);
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
                        $"CustomPrompt{customPrompt.CustomPromptNumber}",
                        customPrompt.CustomPromptText,
                        AllDelegatesViewModelFilterOptions.GetCustomPromptOptions(customPrompt)
                    )
                )
            );
            Filters = filters;
        }

        public int CentreId { get; set; }
        public IEnumerable<SearchableDelegateViewModel> Delegates { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DelegateSortByOptions.Name,
            DelegateSortByOptions.RegistrationDate
        };
    }
}
