namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CentreConfiguration
{
    using System.Globalization;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.External.Maps;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CentreConfiguration;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class CentreConfigurationControllerTests
    {
        private readonly ICentresDataService centresDataService = A.Fake<ICentresDataService>();
        private readonly IMapsApiHelper mapsApiHelper = A.Fake<IMapsApiHelper>();
        private readonly ILogger<CentreConfigurationController> logger =
            A.Fake<ILogger<CentreConfigurationController>>();
        private readonly IImageResizeService imageResizeService = A.Fake<IImageResizeService>();
        private CentreConfigurationController controller = null!;

        [SetUp]
        public void Setup()
        {
            controller =
                new CentreConfigurationController(centresDataService, mapsApiHelper, logger, imageResizeService)
                    .WithDefaultContext()
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
        public void EditCentreDetailsPostSave_with_signature_image_fails_validation()
        {
            // Given
            var model = new EditCentreDetailsViewModel
            {
                NotifyEmail = "email@test.com",
                BannerText = "Banner text",
                CentreSignatureFile = A.Fake<FormFile>()
            };

            // When
            var result = controller.EditCentreDetails(model, "save");

            // Then
            A.CallTo(() => centresDataService.UpdateCentreDetails(A<int>._, A<string>._, A<string>._, A<byte[]?>._, A<byte[]?>._))
                .MustNotHaveHappened();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model);
            controller.ModelState[nameof(EditCentreDetailsViewModel.CentreSignatureFile)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
        }

        [Test]
        public void EditCentreDetailsPostSave_with_logo_image_fails_validation()
        {
            // Given
            var model = new EditCentreDetailsViewModel
            {
                NotifyEmail = "email@test.com",
                BannerText = "Banner text",
                CentreLogoFile = A.Fake<FormFile>()
            };

            // When
            var result = controller.EditCentreDetails(model, "save");

            // Then
            A.CallTo(() => centresDataService.UpdateCentreDetails(A<int>._, A<string>._, A<string>._, A<byte[]?>._, A<byte[]?>._))
                .MustNotHaveHappened();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model);
            controller.ModelState[nameof(EditCentreDetailsViewModel.CentreLogoFile)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
        }

        [Test]
        public void EditCentreDetailsPost_returns_error_with_unexpected_action()
        {
            // Given
            const string action = "unexpectedString";
            var model = new EditCentreDetailsViewModel();

            // When
            var result = controller.EditCentreDetails(model, action);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions").WithActionName("Error");
        }

        [Test]
        public void EditCentreWebsiteDetails_should_show_save_coordinates_when_postcode_is_valid()
        {
            // Given
            var model = new EditCentreWebsiteDetailsViewModel { CentrePostcode = "AA123" };
            const double latitude = 50.123;
            const double longitude = -3.01;
            A.CallTo(() => mapsApiHelper.GeocodePostcode(A<string>._))
                .Returns(CreateSuccessfulMapsResponse(latitude, longitude));

            // When
            var result = controller.EditCentreWebsiteDetails(model);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Index");
            A.CallTo(
                () => centresDataService.UpdateCentreWebsiteDetails(
                    A<int>._,
                    "AA123",
                    A<bool>._,
                    latitude,
                    longitude,
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

        private MapsResponse CreateSuccessfulMapsResponse(double latitude, double longitude)
        {
            return new MapsResponse
            {
                Status = "OK",
                Results = new[]
                {
                    new Map
                    {
                        Geometry = new Geometry
                        {
                            Location = new Coordinates
                            {
                                Latitude = latitude.ToString(CultureInfo.CreateSpecificCulture("en-GB")),
                                Longitude = longitude.ToString(CultureInfo.CreateSpecificCulture("en-GB"))
                            }
                        }
                    }
                }
            };
        }
    }
}
