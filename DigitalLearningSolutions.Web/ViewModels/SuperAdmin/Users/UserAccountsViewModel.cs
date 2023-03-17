namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Users
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.Linq;

    public class UserAccountsViewModel : BaseSearchablePageViewModel<UserAccountEntity>
    {
        public UserAccountsViewModel(
            SearchSortFilterPaginationResult<UserAccountEntity> result
        ) : base(
            result,
            true,
            null,
            "Search user account"
        )
        {
            UserAccounts = result.ItemsToDisplay.Select(
                user => new SearchableUserAccountViewModel(
                    user,
                    result.GetReturnPageQuery($"{user.UserAccount.Id}-card")
                )
            );
        }

        public SuperAdminUserAccountsPage CurrentPage => SuperAdminUserAccountsPage.UserAccounts;

        public IEnumerable<SearchableUserAccountViewModel> UserAccounts { get; set; }

        public int ResultCount { get; set; }

        public int JobGroupId { get; set; }

        public string UserStatus { get; set; }

        public string EmailStatus { get; set; }

        public int? UserId { get; set; }

        public string Search { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name,
        };

        public override bool NoDataFound => !UserAccounts.Any() && NoSearchOrFilter;


    }
}
