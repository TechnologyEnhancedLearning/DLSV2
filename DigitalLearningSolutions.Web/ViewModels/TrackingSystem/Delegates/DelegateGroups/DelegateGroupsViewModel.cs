﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class DelegateGroupsViewModel : BaseSearchablePageViewModel
    {
        public DelegateGroupsViewModel(
            List<Group> groups,
            IEnumerable<CustomPrompt> registrationPrompts,
            string searchString,
            string sortBy,
            string sortDirection,
            string? filterBy,
            int page
        ) : base(searchString, page, true, sortBy, sortDirection, filterBy)
        {
            var sortedItems = GenericSortingHelper.SortAllItems(
                groups.AsQueryable(),
                sortBy,
                sortDirection
            );
            var searchedItems = GenericSearchHelper.SearchItems(sortedItems, SearchString);
            var filteredItems = FilteringHelper.FilterItems(searchedItems.AsQueryable(), filterBy).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(filteredItems);
            var returnPage = string.IsNullOrEmpty(searchString) ? 1 : page;
            DelegateGroups = paginatedItems.Select(g => new SearchableDelegateGroupViewModel(g, returnPage));

            var admins = groups.Select(g => (g.AddedByAdminId, g.AddedByName)).Distinct();

            Filters = new[]
            {
                new FilterViewModel(
                    nameof(Group.AddedByAdminId),
                    "Added by",
                    DelegateGroupsViewModelFilterOptions.GetAddedByOptions(admins)
                ),
                new FilterViewModel(
                    nameof(Group.LinkedToField),
                    "Linked field",
                    DelegateGroupsViewModelFilterOptions.GetLinkedFieldOptions(registrationPrompts)
                )
            };
        }

        public IEnumerable<SearchableDelegateGroupViewModel> DelegateGroups { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DelegateGroupsSortByOptions.Name,
            DelegateGroupsSortByOptions.NumberOfDelegates,
            DelegateGroupsSortByOptions.NumberOfCourses
        };

        public override bool NoDataFound => !DelegateGroups.Any() && NoSearchOrFilter;
    }
}
