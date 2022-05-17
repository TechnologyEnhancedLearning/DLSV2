namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class FindYourCentreControllerTests
    {
        private ICentresService centresService = null!;
        private IRegionDataService regionDataService = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        private FindYourCentreController controller = null!;

        [SetUp]
        public void Setup()
        {
            regionDataService = A.Fake<IRegionDataService>();
            centresService = A.Fake<ICentresService>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();

            controller = new FindYourCentreController(
                centresService,
                regionDataService,
                searchSortFilterPaginateService
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
