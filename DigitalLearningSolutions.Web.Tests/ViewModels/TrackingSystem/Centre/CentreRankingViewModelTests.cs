namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DbModels;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Ranking;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CentreRankingViewModelTests
    {
        private readonly CentreRank[] centreRankings =
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

        [Test]
        public void CentreRankingViewModelViewModel_populates_expected_values_from_centre_ranks_with_centre_in_top_ten()
        {
            // When
            var result = new CentreRankingViewModel(centreRankings, 3);

            // Then
            using (new AssertionScope())
            {
                result.TopTenCentres.Count().Should().Be(10);
                result.CurrentCentre!.Rank.Should().Be(3);
                result.IsCurrentCentreInTopTen.Should().BeTrue();
            }
        }

        [Test]
        public void
            CentreRankingViewModelViewModel_populates_expected_values_from_centre_ranks_with_centre_not_in_top_ten()
        {
            // Given
            var centreRankingsWithExtraCentre = centreRankings.Append(CentreTestHelper.GetCentreRank(20));

            // When
            var result = new CentreRankingViewModel(centreRankingsWithExtraCentre, 20);

            // Then
            using (new AssertionScope())
            {
                result.TopTenCentres.Count().Should().Be(10);
                result.CurrentCentre!.Rank.Should().Be(20);
                result.IsCurrentCentreInTopTen.Should().BeFalse();
            }
        }

        [Test]
        public void
            CentreRankingViewModelViewModel_populates_expected_values_from_centre_ranks_when_centre_has_no_data()
        {
            // When
            var result = new CentreRankingViewModel(centreRankings, 20);

            // Then
            using (new AssertionScope())
            {
                result.TopTenCentres.Count().Should().Be(10);
                result.CurrentCentre.Should().BeNull();
                result.IsCurrentCentreInTopTen.Should().BeFalse();
            }
        }

        [Test]
        public void CentreRankingViewModelViewModel_populates_expected_values_from_centre_ranks_with_less_data()
        {
            // Given
            var shortedCentreRankings = centreRankings.Take(5);

            // When
            var result = new CentreRankingViewModel(shortedCentreRankings, 20);

            // Then
            using (new AssertionScope())
            {
                result.TopTenCentres.Count().Should().Be(5);
                result.CurrentCentre.Should().BeNull();
                result.IsCurrentCentreInTopTen.Should().BeFalse();
            }
        }
    }
}
