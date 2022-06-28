namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class CentreAdministratorsViewModel : BaseSearchablePageViewModel<AdminEntity>
    {
        public CentreAdministratorsViewModel(
            int centreId,
            SearchSortFilterPaginationResult<AdminEntity> result,
            IEnumerable<FilterModel> availableFilters,
            AdminAccount loggedInAdminAccount
        ) : base(
            result,
            true,
            availableFilters,
            "Search administrators"
        )
        {
            CentreId = centreId;
            Admins = result.ItemsToDisplay.Select(
                admin => new SearchableAdminViewModel(
                    admin,
                    loggedInAdminAccount,
                    result.GetReturnPageQuery($"{admin.AdminAccount.Id}-card")
                )
            );
        }

        public int CentreId { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name,
        };

        public IEnumerable<SearchableAdminViewModel> Admins { get; }

        public override bool NoDataFound => !Admins.Any() && NoSearchOrFilter;
    }
}
