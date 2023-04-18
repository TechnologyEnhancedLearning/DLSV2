namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Administrators
{
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using System.Collections.Generic;
    using System.Linq;

    public class AllAdminAccountsViewModel : BaseJavaScriptFilterableViewModel
    {
        public readonly IEnumerable<SearchableAdminAccountsViewModel> Admins;

        public AllAdminAccountsViewModel(
            IEnumerable<AdminEntity> admins,
            IEnumerable<Category> categories,
            AdminAccount loggedInSuperAdminAccount
        )

        {
            Admins = admins.Select(admin => new SearchableAdminAccountsViewModel(admin, loggedInSuperAdminAccount,
                new ReturnPageQuery(1, $"{admin.AdminAccount.Id}-card")));
        }
    }
}
