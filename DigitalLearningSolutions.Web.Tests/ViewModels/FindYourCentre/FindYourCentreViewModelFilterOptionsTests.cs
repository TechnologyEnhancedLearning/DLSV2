namespace DigitalLearningSolutions.Web.Tests.ViewModels.FindYourCentre
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.FindYourCentre;
    using FluentAssertions;
    using NUnit.Framework;

    public class FindYourCentreViewModelFilterOptionsTests
    {
        private readonly FilterModel expectedRegionFilters = new FilterModel(
            "RegionName",
            "Region",
            new[]
            {
                new FilterOptionModel(
                    "Region1",
                    "RegionName" + FilteringHelper.Separator + "RegionName" + FilteringHelper.Separator + "Region1",
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "Region2",
                    "RegionName" + FilteringHelper.Separator + "RegionName" + FilteringHelper.Separator + "Region2",
                    FilterStatus.Default
                ),
            }
        );

        private readonly IEnumerable<(int, string)> filterableRegions = new List<(int, string)>
        {
            (1, "Region1"),
            (2, "Region2"),
        };

        [Test]
        public void GetFilterOptions_correctly_sets_up_filters()
        {
            // When
            var result =
                FindYourCentreViewModelFilterOptions.GetFindCentreFilterModels(filterableRegions);

            // Then
            result.Should().BeEquivalentTo(
                new List<FilterModel>
                    { expectedRegionFilters }
            );
        }
    }
}
