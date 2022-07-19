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
            IEnumerable<AdminEntity> admins,
            IEnumerable<Category> categories,
            AdminAccount loggedInAdminAccount
        )

        {
            Admins = admins.Select(admin => new SearchableAdminViewModel(admin, loggedInAdminAccount,
                new ReturnPageQuery(1, $"{admin.AdminAccount.Id}-card")));

            Filters = AdministratorsViewModelFilterOptions.GetAllAdministratorsFilterModels(categories)
                .SelectAppliedFilterViewModels();
        }
    }
}
