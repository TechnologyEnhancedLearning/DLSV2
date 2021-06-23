namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class CentreAdministratorsViewModel : BaseSearchablePageViewModel
    {
        public CentreAdministratorsViewModel(
            int centreId,
            IEnumerable<AdminUser> adminUsers,
            string? searchString,
            string sortBy,
            string sortDirection,
            int page
        ) : base(searchString, sortBy, sortDirection, page)
        {
            CentreId = centreId;
            var sortedItems = GenericSortingHelper.SortAllItems(
                adminUsers.AsQueryable(),
                sortBy,
                sortDirection
            );
            var searchedItems = GenericSearchHelper.SearchItems(sortedItems, SearchString).ToList();
            MatchingSearchResults = searchedItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(searchedItems);
            Admins = paginatedItems.Select(adminUser => new SearchableAdminViewModel(adminUser));
        }

        public int CentreId { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name
        };

        public IEnumerable<SearchableAdminViewModel> Admins { get; }
    }
}
