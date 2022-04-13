namespace DigitalLearningSolutions.Web.ViewModels.FindYourCentre
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using Microsoft.Extensions.Configuration;

    public class FindYourCentreViewModel : BaseSearchablePageViewModel<CentreSummaryForFindCentre>
    {
        public IEnumerable<CentreSummaryForFindCentre> CentreSummaries { get; set; }

        public FindYourCentreViewModel(SearchSortFilterPaginationResult<CentreSummaryForFindCentre> centreSummaries) :
            base(centreSummaries, false, null, "Search Centres")
        {
            CentreSummaries = centreSummaries.ItemsToDisplay;
        }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name,
        };
        public override bool NoDataFound => !CentreSummaries.Any() && NoSearchOrFilter;
    }
}
