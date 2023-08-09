namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using System.Collections.Generic;
    using System.Linq;

    public class CentresViewModel : BaseSearchablePageViewModel<CentreEntity>
    {
        public CentresViewModel(SearchSortFilterPaginationResult<CentreEntity> result) : base(
            result,
            true,
            null,
            "Search centre"
        )
        {
            Centres = result.ItemsToDisplay.Select(
                centre => new SearchableCentreViewModel(
                    centre,
                    result.GetReturnPageQuery($"{centre.Centre.CentreId}-card")
                )
            );
        }

        public string Search { get; set; }
        public int Region { get; set; }
        public int CentreType { get; set; }
        public int ContractType { get; set; }
        public string CentreStatus { get; set; }
        public IEnumerable<SearchableCentreViewModel> Centres { get; set; }
        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name,
        };

        public override bool NoDataFound => !Centres.Any() && NoSearchOrFilter;
    }
}
