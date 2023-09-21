namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Delegates
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.SuperAdmin;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using System.Collections.Generic;
    using System.Linq;
    public class DelegatesViewModel : BaseSearchablePageViewModel<SuperAdminDelegateAccount>
    {
        public DelegatesViewModel(
            SearchSortFilterPaginationResult<SuperAdminDelegateAccount> result
        ) : base(
            result,
            true,
            null,
            "Search delegates account"
        )
        {
            Delegates = result.ItemsToDisplay.Select(
                delegates => new SearchableDelegatesViewModel(
                    delegates,
                    result.GetReturnPageQuery($"{delegates.Id}-card")
                )
            );
        }

        public SuperAdminUserAccountsPage CurrentPage => SuperAdminUserAccountsPage.Delegates;

        public IEnumerable<SearchableDelegatesViewModel> Delegates { get; set; }

        public int? DelegateID { get; set; }
        public string AccountStatus { get; set; }
        public string LHLinkStatus { get; set; }
        public int? CentreID { get; set; }
        public string Search { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name,
        };

        public override bool NoDataFound => !Delegates.Any() && NoSearchOrFilter;
    }
}
