namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CentreConfiguration
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.External.Maps;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CentreConfiguration;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class CentreConfigurationControllerTests
    {
        private ICentresDataService centresDataService = null!;
        private CentreConfigurationController controller = null!;
        private ILogger<CentreConfigurationController> logger = null!;
        private IMapsApiHelper mapsApiHelper = null!;

        [SetUp]
        public void Setup()
        {
            centresDataService = A.Fake<ICentresDataService>();
            mapsApiHelper = A.Fake<IMapsApiHelper>();
            logger = A.Fake<ILogger<CentreConfigurationController>>();

            controller =
                new CentreConfigurationController(centresDataService, mapsApiHelper, logger).WithDefaultContext()
                    .WithMockUser(true);

            A.CallTo(
                () => centresDataService.UpdateCentreWebsiteDetails(
                    A<int>._,
                    A<string>._,
                    A<bool>._,
                    A<double>._,
                    A<double>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._
                )
            ).DoesNothing();
        }

        [Test]
        public void EditCentreWebsiteDetails_should_show_validation_error_to_user_when_given_invalid_postcode()
        {
            // Given
            var model = new EditCentreWebsiteDetailsViewModel { CentrePostcode = "aaaaaaaaa" };
            A.CallTo(() => mapsApiHelper.GeocodePostcode(A<string>._))
                .Returns(new MapsResponse { Status = "ZERO_RESULTS" });

            // When
            var result = controller.EditCentreWebsiteDetails(model);

            // Then
            result.Should().BeViewResult().WithDefaultViewName()
                .ModelAs<EditCentreWebsiteDetailsViewModel>();
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [TestCase("OVER_DAILY_LIMIT")]
        [TestCase("OVER_QUERY_LIMIT")]
        [TestCase("REQUEST_DENIED")]
        [TestCase("INVALID_REQUEST")]
        [TestCase("UNKNOWN_ERROR")]
        public void EditCentreWebsiteDetails_should_redirect_to_error_page_when_API_issue_occurs(string statusCode)
        {
            // Given
            var model = new EditCentreWebsiteDetailsViewModel { CentrePostcode = "AA123" };
            A.CallTo(() => mapsApiHelper.GeocodePostcode(A<string>._))
                .Returns(new MapsResponse { Status = statusCode });

            // When
            var result = controller.EditCentreWebsiteDetails(model);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Error").WithControllerName("LearningSolutions");
        }

        [Test]
        public void EditCentreWebsiteDetails_should_show_save_coordinates_when_postcode_is_valid()
        {
            // Given
            var model = new EditCentreWebsiteDetailsViewModel { CentrePostcode = "AA123" };
            A.CallTo(() => mapsApiHelper.GeocodePostcode(A<string>._))
                .Returns(
                    new MapsResponse
                    {
                        Status = "OK",
                        Results = new[]
                        {
                            new Map
                            {
                                Geometry = new Geometry
                                    { Location = new Coordinates { Latitude = "50.123", Longitude = "-3.01" } }
                            }
                        }
                    }
                );

            // When
            var result = controller.EditCentreWebsiteDetails(model);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Index");
            A.CallTo(
                () => centresDataService.UpdateCentreWebsiteDetails(
                    A<int>._,
                    "AA123",
                    A<bool>._,
                    -3.01,
                    50.123,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<string>._
                )
            ).MustHaveHappened();
        }
    }
}
