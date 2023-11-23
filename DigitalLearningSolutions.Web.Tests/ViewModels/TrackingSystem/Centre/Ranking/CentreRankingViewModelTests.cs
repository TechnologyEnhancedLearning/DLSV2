namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Ranking
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DbModels;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Ranking;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CentreRankingViewModelTests
    {
        private readonly CentreRanking[] centreRankings =
        {
            CentreTestHelper.GetCentreRank(1),
            CentreTestHelper.GetCentreRank(2),
            CentreTestHelper.GetCentreRank(3),
            CentreTestHelper.GetCentreRank(4),
            CentreTestHelper.GetCentreRank(5),
            CentreTestHelper.GetCentreRank(6),
            CentreTestHelper.GetCentreRank(7),
            CentreTestHelper.GetCentreRank(8),
            CentreTestHelper.GetCentreRank(9),
            CentreTestHelper.GetCentreRank(10)
        };

        private readonly List<(int, string)> regions = new List<(int, string)>
        {
            (1, "North"),
            (2, "South"),
            (3, "East"),
            (4, "West")
        };

        [Test]
        public void CentreRankingViewModel_populates_expected_values_from_centre_ranks_with_centre_in_top_ten()
        {
            // When
            var result = new CentreRankingViewModel(centreRankings, 3, regions);

            // Then
            using (new AssertionScope())
            {
                result.Centres.Count().Should().Be(10);
                result.CentreHasNoActivity.Should().BeFalse();
            }
        }

        [Test]
        public void
            CentreRankingViewModel_populates_expected_values_from_centre_ranks_with_centre_not_in_top_ten()
        {
            // Given
            var centreRankingsWithExtraCentre = centreRankings.Append(CentreTestHelper.GetCentreRank(20));

            // When
            var result = new CentreRankingViewModel(centreRankingsWithExtraCentre, 20, regions);

            // Then
            using (new AssertionScope())
            {
                result.Centres.Count().Should().Be(11);
                result.CentreHasNoActivity.Should().BeFalse();
            }
        }

        [Test]
        public void
            CentreRankingViewModel_populates_expected_values_from_centre_ranks_when_centre_has_no_data()
        {
            // When
            var result = new CentreRankingViewModel(centreRankings, 20, regions);

            // Then
            using (new AssertionScope())
            {
                result.Centres.Count().Should().Be(10);
                result.CentreHasNoActivity.Should().BeTrue();
            }
        }

        [Test]
        public void CentreRankingViewModel_populates_expected_values_from_centre_ranks_with_less_data()
        {
            // Given
            var shortedCentreRankings = centreRankings.Take(5);

            // When
            var result = new CentreRankingViewModel(shortedCentreRankings, 20, regions);

            // Then
            using (new AssertionScope())
            {
                result.Centres.Count().Should().Be(5);
                result.CentreHasNoActivity.Should().BeTrue();
            }
        }

        [Test]
        public void CentreRankingViewModel_populates_region_options_correctly()
        {
            // When
            var result = new CentreRankingViewModel(centreRankings, 3, regions, 4);

            // Then
            using (new AssertionScope())
            {
                result.RegionOptions.Should().NotBeNullOrEmpty();
                result.RegionOptions.Count().Should().Be(4);

                result.RegionOptions.ElementAt(0).Value.Should().Be("1");
                result.RegionOptions.ElementAt(0).Text.Should().Be("North");
                result.RegionOptions.ElementAt(0).Selected.Should().BeFalse();

                result.RegionOptions.ElementAt(3).Value.Should().Be("4");
                result.RegionOptions.ElementAt(3).Text.Should().Be("West");
                result.RegionOptions.ElementAt(3).Selected.Should().BeTrue();
            }
        }

        [Test]
        public void GeneratePeriodSelectListWithSelectedItem_returns_expected_list()
        {
            // When
            var result = new CentreRankingViewModel(centreRankings, 3, regions, 4, Period.Fortnight).PeriodOptions;

            // Then
            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Count().Should().Be(5);

                result.ElementAt(0).Value.Should().Be("0");
                result.ElementAt(0).Text.Should().Be("Week");
                result.ElementAt(0).Selected.Should().BeFalse();

                result.ElementAt(1).Value.Should().Be("1");
                result.ElementAt(1).Text.Should().Be("Fortnight");
                result.ElementAt(1).Selected.Should().BeTrue();
            }
        }
    }
}
