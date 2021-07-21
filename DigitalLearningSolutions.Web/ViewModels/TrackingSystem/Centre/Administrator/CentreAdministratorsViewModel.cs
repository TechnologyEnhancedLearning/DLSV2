﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class CentreAdministratorsViewModel : BaseSearchablePageViewModel
    {
        public CentreAdministratorsViewModel(
            int centreId,
            IEnumerable<AdminUser> adminUsers,
            IEnumerable<string> categories,
            string? searchString,
            string? filterString,
            int page
        ) : base(searchString, page, false, true, filterString: filterString)
        {
            CentreId = centreId;
            var sortedItems = GenericSortingHelper.SortAllItems(
                adminUsers.AsQueryable(),
                DefaultSortByOptions.Name.PropertyName,
                Ascending
            );
            var searchedItems = GenericSearchHelper.SearchItems(sortedItems, SearchString);
            var filteredItems = FilteringHelper.FilterItems(searchedItems.AsQueryable(), filterString).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(filteredItems);
            Admins = paginatedItems.Select(adminUser => new SearchableAdminViewModel(adminUser));

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
                )
            };
        }

        public int CentreId { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name
        };

        public IEnumerable<SearchableAdminViewModel> Admins { get; }
    }
}
