namespace DigitalLearningSolutions.Web.ViewModels.FindYourCentre
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class FindYourCentreViewModel : BaseSearchablePageViewModel<CentreSummaryForFindYourCentre>
    {
        public FindYourCentreViewModel(
            SearchSortFilterPaginationResult<CentreSummaryForFindYourCentre> centreSummaries,
            IEnumerable<FilterModel> availableFilters
        ) :
            base(centreSummaries, true, availableFilters, "Search Centres")
        {
            CentreSummaries = centreSummaries.ItemsToDisplay;
        }

        public IEnumerable<CentreSummaryForFindYourCentre> CentreSummaries { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = Array.Empty<(string, string)>();

        public override bool NoDataFound => !CentreSummaries.Any() && NoSearchOrFilter;
    }
}
