namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class CentreAdministratorsViewModel : BaseSearchablePageViewModel
    {
        private static readonly IEnumerable<FilterOptionViewModel> RoleOptions = new[]
        {
            AdminFilterOptions.CentreAdministrator,
            AdminFilterOptions.Supervisor,
            AdminFilterOptions.Trainer,
            AdminFilterOptions.ContentCreatorLicense,
            AdminFilterOptions.CmsAdministrator,
            AdminFilterOptions.CmsManager
        };

        private static readonly IEnumerable<FilterOptionViewModel> AccountStatusOptions = new[]
        {
            AdminFilterOptions.IsLocked,
            AdminFilterOptions.IsNotLocked
        };

        public CentreAdministratorsViewModel(
            int centreId,
            IEnumerable<AdminUser> adminUsers,
            IEnumerable<string> categories,
            string? searchString,
            string sortBy,
            string sortDirection,
            string? filterString,
            int page
        ) : base(searchString, sortBy, sortDirection, filterString, page)
        {
            CentreId = centreId;
            var sortedItems = GenericSortingHelper.SortAllItems(
                adminUsers.AsQueryable(),
                sortBy,
                sortDirection
            );
            var searchedItems = GenericSearchHelper.SearchItems(sortedItems, SearchString);
            var filteredItems = FilteringHelper.FilterItems(searchedItems.AsQueryable(), filterString).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(filteredItems);
            Admins = paginatedItems.Select(adminUser => new SearchableAdminViewModel(adminUser));
            IEnumerable<FilterOptionViewModel> categoryOptions =
                categories.Select(
                    c => new FilterOptionViewModel(c, $"{nameof(AdminUser.CategoryName)}|{c}", FilterStatus.Default)
                );

            Filters = new[]
            {
                new FilterViewModel("Role", "Role", RoleOptions),
                new FilterViewModel(nameof(AdminUser.CategoryName), "Category", categoryOptions),
                new FilterViewModel("AccountStatus", "Account Status", AccountStatusOptions)
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
