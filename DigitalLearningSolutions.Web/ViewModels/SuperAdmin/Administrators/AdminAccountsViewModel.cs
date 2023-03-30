namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Administrators
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using System.Collections.Generic;
    using System.Linq;

    public class AdminAccountsViewModel : BaseSearchablePageViewModel<AdminEntity>
    {
        public AdminAccountsViewModel(
            SearchSortFilterPaginationResult<AdminEntity> result,
            AdminAccount loggedInSuperAdminAccount
        ) : base(
            result,
            true,
            null,
            "Search admin account"
        )
        {
            Admins = result.ItemsToDisplay.Select(
                admin => new SearchableAdminAccountsViewModel(
                    admin,
                    loggedInSuperAdminAccount,
                    result.GetReturnPageQuery($"{admin.UserAccount.Id}-card")
                )
            );
        }

        public SuperAdminUserAccountsPage CurrentPage => SuperAdminUserAccountsPage.AdminAccounts;

        public IEnumerable<SearchableAdminAccountsViewModel> Admins { get; set; }        

        public int? AdminID { get; set; }
        public string UserStatus { get; set; }
        public string Role { get; set; }
        public int? CentreID { get; set; }
        public string Search { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name,
        };

        public override bool NoDataFound => !Admins.Any() && NoSearchOrFilter;
    }
}
