namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using NUnit.Framework;

    public class CentresServiceTests
    {
        private ICentresDataService centresDataService = null!;
        private ICentresService centresService = null!;
        private IClockService clockService = null!;

        [SetUp]
        public void Setup()
        {
            centresDataService = A.Fake<ICentresDataService>();
            clockService = A.Fake<IClockService>();
            centresService = new CentresService(centresDataService, clockService);

            A.CallTo(() => clockService.UtcNow).Returns(new DateTime(2021, 1, 1));
            A.CallTo(() => centresDataService.GetCentreRanks(A<DateTime>._, A<int?>._, 10, A<int>._)).Returns(
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
                }
            );
        }

        [Test]
        public void GetCentreRanks_returns_expected_list()
        {
            // When
            var result = centresService.GetCentresForCentreRankingPage(3, 14, null);

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
        public void GetCentreRankForCentre_returns_null_with_no_data_for_centre()
        {
            // When
            var result = centresService.GetCentreRankForCentre(20);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetAllCentreSummariesForSuperAdmin_calls_dataService_and_returns_all_summary_details()
        {
            // Given
            var centres = Builder<CentreSummaryForSuperAdmin>.CreateListOfSize(10).Build();
            A.CallTo(() => centresDataService.GetAllCentreSummariesForSuperAdmin()).Returns(centres);

            // When
            var result = centresService.GetAllCentreSummariesForSuperAdmin();

            // Then
            result.Should().HaveCount(10);
        }

        [Test]
        public void GetAllCentreSummariesForMap_calls_dataService_and_returns_all_summary_details()
        {
            // Given
            var centres = Builder<CentreSummaryForMap>.CreateListOfSize(10).Build();
            A.CallTo(() => centresDataService.GetAllCentreSummariesForMap()).Returns(centres);

            // When
            var result = centresService.GetAllCentreSummariesForMap();

            // Then
            result.Should().BeEquivalentTo(centres);
        }

        [Test]
        public void GetAllCentreSummariesForFindCentre_calls_dataService_and_returns_all_summary_details()
        {
            // Given
            var expectedCentres = Builder<CentreSummaryForFindYourCentre>.CreateListOfSize(10).Build();
            A.CallTo(() => centresDataService.GetAllCentreSummariesForFindCentre()).Returns(expectedCentres);

            // When
            var result = centresService.GetAllCentreSummariesForFindCentre();

            // Then
            result.Should().HaveCount(10);
        }
    }
}
