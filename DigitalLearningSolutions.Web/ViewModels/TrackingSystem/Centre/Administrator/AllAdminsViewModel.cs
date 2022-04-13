namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllAdminsViewModel : BaseJavaScriptFilterableViewModel
    {
        public readonly IEnumerable<SearchableAdminViewModel> Admins;

        public AllAdminsViewModel(
            IEnumerable<AdminUser> adminUsers,
            IEnumerable<Category> categories,
            AdminUser loggedInAdminUser
        )

        {
            Admins = adminUsers.Select(au => new SearchableAdminViewModel(au, loggedInAdminUser,
                new ReturnPageQuery(1, $"{au.Id}-card")));

            Filters = AdministratorsViewModelFilterOptions.GetAllAdministratorsFilterModels(categories)
                .SelectAppliedFilterViewModels();
        }
    }
}
