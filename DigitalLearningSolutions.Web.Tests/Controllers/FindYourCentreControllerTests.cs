namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;
    using NUnit.Framework;

    public class FindYourCentreControllerTests
    {
        private ICentresService centresService = null!;
        private IRegionDataService regionDataService = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        private FindYourCentreController controller = null!;
        private IConfiguration configuration = null!;
        private IFeatureManager featureManager = null!;

        [SetUp]
        public void Setup()
        {
            regionDataService = A.Fake<IRegionDataService>();
            centresService = A.Fake<ICentresService>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();
            configuration = A.Fake<IConfiguration>();
            featureManager = A.Fake<IFeatureManager>();

            controller = new FindYourCentreController(
                centresService,
                regionDataService,
                searchSortFilterPaginateService,
                configuration,
                featureManager
            );
        }

        [Test]
        public void CentreData_returns_expected_json_result()
        {
            // Given
            var centres = Builder<CentreSummaryForMap>.CreateListOfSize(10).Build();
            A.CallTo(() => centresService.GetAllCentreSummariesForMap()).Returns(centres);

            // When
            var result = controller.CentreData();

            // Then
            using (new AssertionScope())
            {
                result.Should().BeJsonResult();
                result.As<JsonResult>().Value.Should().BeEquivalentTo(centres);
            }
        }
    }
}
