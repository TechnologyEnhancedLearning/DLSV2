﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllDelegatesViewModel : BaseSearchablePageViewModel
    {
        public AllDelegatesViewModel(
            IEnumerable<DelegateUserCard> delegateUserCards,
            IEnumerable<(int id, string name)> jobGroups,
            IEnumerable<CustomPrompt> customPrompts,
            int page,
            string? searchString,
            string sortBy,
            string sortDirection,
            string? filterBy,
            int? itemsPerPage
        ) : base(
            searchString,
            page,
            true,
            sortBy,
            sortDirection,
            filterBy,
            itemsPerPage ?? DefaultItemsPerPage,
            searchLabel: "Search delegates"
        )
        {
            var sortedItems = GenericSortingHelper.SortAllItems(
                delegateUserCards.AsQueryable(),
                sortBy,
                sortDirection
            );
            var searchedItems = GenericSearchHelper.SearchItems(sortedItems, SearchString);
            var filteredItems = FilteringHelper.FilterItems(searchedItems.AsQueryable(), filterBy).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(filteredItems);

            var promptsWithOptions = customPrompts.Where(customPrompt => customPrompt.Options.Count > 0);
            Delegates = paginatedItems.Select(
                delegateUser =>
                {
                    var customFields = CentreCustomPromptHelper.GetCustomFieldViewModels(delegateUser, customPrompts);
                    return new SearchableDelegateViewModel(delegateUser, customFields, promptsWithOptions, page);
                }
            );

            Filters = AllDelegatesViewModelFilterOptions.GetAllDelegatesFilterViewModels(
                jobGroups,
                promptsWithOptions
            );
        }

        public IEnumerable<SearchableDelegateViewModel> Delegates { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DelegateSortByOptions.Name,
            DelegateSortByOptions.RegistrationDate
        };

        public override bool NoDataFound => !Delegates.Any() && NoSearchOrFilter;
    }
}
