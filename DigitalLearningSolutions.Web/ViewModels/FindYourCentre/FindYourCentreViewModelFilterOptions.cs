namespace DigitalLearningSolutions.Web.ViewModels.FindYourCentre
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public static class FindYourCentreViewModelFilterOptions
    {
        private static IEnumerable<FilterOptionModel> GetRegionOptions(
            IEnumerable<(int regionId, string regionName)> regions
        )
        {
            return regions.Select(
                region => new FilterOptionModel(
                    region.regionName,
                    nameof(CentreSummaryForFindYourCentre.RegionName) + FilteringHelper.Separator +
                    nameof(CentreSummaryForFindYourCentre.RegionName) + FilteringHelper.Separator + region.regionName,
                    FilterStatus.Default
                )
            );
        }

        public static IEnumerable<FilterModel> GetFindCentreFilterModels(
            IEnumerable<(int id, string name)> regions
        )
        {
            return new[]
            {
                new FilterModel(
                    nameof(CentreSummaryForFindYourCentre.RegionName),
                    "Region",
                    GetRegionOptions(regions)
                )
            };
        }
    }
}
