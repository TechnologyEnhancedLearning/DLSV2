namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Users
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using System.Collections.Generic;
    using System.Linq;

    public class AllUserViewModel : BaseJavaScriptFilterableViewModel
    {

        public readonly IEnumerable<SearchableUserAccountViewModel> UserAccounts;

        public AllUserViewModel(
                IEnumerable<UserAccountEntity> users
            )
        {
            UserAccounts = users.Select(user => new SearchableUserAccountViewModel(user,
                new ReturnPageQuery(1, $"{user.UserAccount.Id}-card")));
        }

    }
}
