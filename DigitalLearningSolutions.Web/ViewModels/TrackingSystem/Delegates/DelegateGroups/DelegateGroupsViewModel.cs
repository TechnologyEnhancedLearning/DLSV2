namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class DelegateGroupsViewModel : BaseSearchablePageViewModel
    {
        public DelegateGroupsViewModel(
            IEnumerable<Group> groups,
            IEnumerable<(int, string)> registrationPrompts,
            string sortBy,
            string sortDirection,
            string? filterBy,
            int page
        ) : base(null, page, true, sortBy, sortDirection, filterBy)
        {
            groups = groups.ToList();
            var sortedItems = GenericSortingHelper.SortAllItems(
                groups.AsQueryable(),
                sortBy,
                sortDirection
            );
            var filteredItems = FilteringHelper.FilterItems(sortedItems.AsQueryable(), filterBy).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(filteredItems);

            DelegateGroups = paginatedItems.Select(g => new SearchableDelegateGroupViewModel(g));

            var admins = groups.Select(g => (AddedByAdminID: g.AddedByAdminId, g.AddedByName)).Distinct();

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
    }
}
