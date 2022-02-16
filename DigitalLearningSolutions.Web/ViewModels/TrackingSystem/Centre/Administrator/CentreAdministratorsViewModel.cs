namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class CentreAdministratorsViewModel : BaseSearchablePageViewModel
    {
        public CentreAdministratorsViewModel(
            int centreId,
            IEnumerable<AdminUser> adminUsers,
            IEnumerable<string> categories,
            string? searchString,
            string? filterBy,
            int page,
            AdminUser loggedInAdminUser,
            int? itemsPerPage
        ) : base(
            searchString,
            page,
            true,
            filterBy: filterBy,
            itemsPerPage: itemsPerPage ?? DefaultItemsPerPage,
            searchLabel: "Search administrators"
        )
        {
            CentreId = centreId;
            var sortedItems = GenericSortingHelper.SortAllItems(
                adminUsers.AsQueryable(),
                GenericSortingHelper.DefaultSortOption,
                GenericSortingHelper.Ascending
            );
            var searchedItems = GenericSearchHelper.SearchItems(sortedItems, SearchString);
            var filteredItems = FilteringHelper.FilterItems(searchedItems.AsQueryable(), filterBy).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(filteredItems);
            var returnPage = string.IsNullOrWhiteSpace(searchString) ? page : 1;
            Admins = paginatedItems.Select(
                adminUser => new SearchableAdminViewModel(adminUser, loggedInAdminUser, returnPage)
            );

            Filters = new[]
            {
                new FilterViewModel("Role", "Role", AdministratorsViewModelFilterOptions.RoleOptions),
                new FilterViewModel(
                    "CategoryName",
                    "Category",
                    AdministratorsViewModelFilterOptions.GetCategoryOptions(categories)
                ),
                new FilterViewModel(
                    "AccountStatus",
                    "Account Status",
                    AdministratorsViewModelFilterOptions.AccountStatusOptions
                ),
            };
        }

        public int CentreId { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name,
        };

        public IEnumerable<SearchableAdminViewModel> Admins { get; }

        public override bool NoDataFound => !Admins.Any() && NoSearchOrFilter;
    }
}
