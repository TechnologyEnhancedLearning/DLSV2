namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllAdminsViewModel : BaseJavaScriptFilterableViewModel
    {
        public readonly IEnumerable<SearchableAdminViewModel> Admins;

        public AllAdminsViewModel(IEnumerable<AdminUser> adminUsers,
            IEnumerable<string> categories,
            AdminUser loggedInAdminUser)
        
        {
            Admins = adminUsers.Select(au => new SearchableAdminViewModel(au, loggedInAdminUser, 1));

            Filters = new[]
            {
                new FilterModel("Role", "Role", AdministratorsViewModelFilterOptions.RoleOptions),
                new FilterModel(
                    "CategoryName",
                    "Category",
                    AdministratorsViewModelFilterOptions.GetCategoryOptions(categories)
                ),
                new FilterModel(
                    "AccountStatus",
                    "Account Status",
                    AdministratorsViewModelFilterOptions.AccountStatusOptions
                )
            }.SelectAppliedFilterViewModels();
        }
    }
}
