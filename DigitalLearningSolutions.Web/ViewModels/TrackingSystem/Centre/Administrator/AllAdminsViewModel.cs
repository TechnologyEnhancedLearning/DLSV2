namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
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
            Admins = adminUsers.Select(au => new SearchableAdminViewModel(au, loggedInAdminUser));

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
            }.SelectAppliedFilterViewModels();
        }
    }
}
