namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class CentresServiceTests
    {
        private ICentresDataService centresDataService = null!;
        private ICentresService centresService = null!;

        [SetUp]
        public void Setup()
        {
            centresDataService = A.Fake<ICentresDataService>();
            centresService = new CentresService(centresDataService);

            A.CallTo(() => centresDataService.GetCentreRanks(A<DateTime>._, A<int>._)).Returns(
                new[]
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
                        CentreTestHelper.GetCentreRank(10),
                        CentreTestHelper.GetCentreRank(11),
                        CentreTestHelper.GetCentreRank(12)
                    }
            );
        }

        [Test]
        public void GetCentreRanks_returns_expected_list_when_centre_in_top_ten()
        {
            // When
            var result = centresService.GetTopCentreRanks(3, 14, -1);

            // Then
            result.Count().Should().Be(10);
        }

        [Test]
        public void GetCentreRanks_returns_expected_list_when_centre_is_not_in_top_ten()
        {
            // When
            var result = centresService.GetTopCentreRanks(12, 14, -1);

            // Then
            result.Count().Should().Be(11);
        }

        [Test]
        public void GetCentreRanks_returns_expected_list_when_centre_has_no_data()
        {
            // When
            var result = centresService.GetTopCentreRanks(20, 14, -1);

            // Then
            result.Count().Should().Be(10);
        }

        [Test]
        public void GetCentreRankForCentre_returns_expected_value()
        {
            // When
            var result = centresService.GetCentreRankForCentre(3);

            // Then
            result.Should().Be(3);
        }

        [Test]
        public void GetCentreRankForCentre_returns_negative_one_with_no_data_for_centre()
        {
            // When
            var result = centresService.GetCentreRankForCentre(20);

            // Then
            result.Should().Be(-1);
        }
    }
}
