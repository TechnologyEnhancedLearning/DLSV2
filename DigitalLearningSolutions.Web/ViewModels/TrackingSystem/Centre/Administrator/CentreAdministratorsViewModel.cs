namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class CentreAdministratorsViewModel : BaseSearchablePageViewModel
    {
        private static readonly IEnumerable<FilterOptionViewModel> RoleOptions = new[]
        {
            AdminRoleFilterOptions.CentreAdministrator,
            AdminRoleFilterOptions.Supervisor,
            AdminRoleFilterOptions.Trainer,
            AdminRoleFilterOptions.ContentCreatorLicense,
            AdminRoleFilterOptions.CmsAdministrator,
            AdminRoleFilterOptions.CmsManager
        };

        private static readonly IEnumerable<FilterOptionViewModel> AccountStatusOptions = new[]
        {
            AdminAccountStatusFilterOptions.IsLocked,
            AdminAccountStatusFilterOptions.IsNotLocked
        };

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
            IEnumerable<FilterOptionViewModel> categoryOptions =
                categories.Select(
                    c => new FilterOptionViewModel(
                        c,
                        nameof(AdminUser.CategoryName) + FilteringHelper.Separator + nameof(AdminUser.CategoryName) +
                        FilteringHelper.Separator + c,
                        FilterStatus.Default
                    )
                );

            Filters = new[]
            {
                new FilterViewModel("Role", "Role", RoleOptions),
                new FilterViewModel("CategoryName", "Category", categoryOptions),
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
