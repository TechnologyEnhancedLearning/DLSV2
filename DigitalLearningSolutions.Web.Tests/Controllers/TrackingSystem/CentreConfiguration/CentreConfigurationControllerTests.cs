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

        [TearDown]
        public void Cleanup()
        {
            Fake.ClearRecordedCalls(centresDataService);
            Fake.ClearRecordedCalls(mapsApiHelper);
            Fake.ClearRecordedCalls(logger);
            Fake.ClearRecordedCalls(imageResizeService);
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
        public void EditCentreDetailsPostSave_without_previewing_signature_image_fails_validation()
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
            A.CallTo(
                    () => centresDataService.UpdateCentreDetails(
                        A<int>._,
                        A<string>._,
                        A<string>._,
                        A<byte[]?>._,
                        A<byte[]?>._
                    )
                )
                .MustNotHaveHappened();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model);
            controller.ModelState[nameof(EditCentreDetailsViewModel.CentreSignatureFile)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
        }

        [Test]
        public void EditCentreDetailsPostSave_without_previewing_logo_image_fails_validation()
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
            A.CallTo(
                    () => centresDataService.UpdateCentreDetails(
                        A<int>._,
                        A<string>._,
                        A<string>._,
                        A<byte[]?>._,
                        A<byte[]?>._
                    )
                )
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
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        [Test]
        public void EditCentreDetailsPost_updates_centre_and_redirects_with_successful_save()
        {
            // Given
            const string action = "save";
            var model = new EditCentreDetailsViewModel
            {
                BannerText = "Test banner text"
            };

            // When
            var result = controller.EditCentreDetails(model, action);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Index");
            A.CallTo(() => centresDataService.UpdateCentreDetails(2, null, model.BannerText, null, null))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void EditCentreDetailsPost_previewSignature_calls_imageResizeService()
        {
            // Given
            const string action = "previewSignature";
            var model = new EditCentreDetailsViewModel
            {
                BannerText = "Test banner text",
                CentreSignature = new byte[100],
                CentreSignatureFile = A.Fake<IFormFile>()
            };
            var newImage = new byte [200];
            A.CallTo(() => imageResizeService.ResizeCentreImage(A<IFormFile>._)).Returns(newImage);

            // When
            var result = controller.EditCentreDetails(model, action);

            // Then
            result.Should().BeViewResult();
            A.CallTo(() => imageResizeService.ResizeCentreImage(A<IFormFile>._)).MustHaveHappenedOnceExactly();
            var returnModel = (result as ViewResult)!.Model as EditCentreDetailsViewModel;
            returnModel!.CentreSignature.Should().BeEquivalentTo(newImage);
        }

        [Test]
        public void EditCentreDetailsPost_previewLogo_calls_imageResizeService()
        {
            // Given
            const string action = "previewLogo";
            var model = new EditCentreDetailsViewModel
            {
                BannerText = "Test banner text",
                CentreLogo = new byte[100],
                CentreLogoFile = A.Fake<IFormFile>()
            };
            var newImage = new byte [200];
            A.CallTo(() => imageResizeService.ResizeCentreImage(A<IFormFile>._)).Returns(newImage);

            // When
            var result = controller.EditCentreDetails(model, action);

            // Then
            result.Should().BeViewResult();
            A.CallTo(() => imageResizeService.ResizeCentreImage(A<IFormFile>._)).MustHaveHappenedOnceExactly();
            var returnModel = (result as ViewResult)!.Model as EditCentreDetailsViewModel;
            returnModel!.CentreLogo.Should().BeEquivalentTo(newImage);
        }

        [Test]
        public void EditCentreDetailsPost_removeSignature_removes_signature()
        {
            // Given
            const string action = "removeSignature";
            var model = new EditCentreDetailsViewModel
            {
                BannerText = "Test banner text",
                CentreSignature = new byte[100]
            };

            // When
            var result = controller.EditCentreDetails(model, action);

            // Then
            result.Should().BeViewResult();
            var returnModel = (result as ViewResult)!.Model as EditCentreDetailsViewModel;
            returnModel!.CentreSignature.Should().BeNull();
        }

        [Test]
        public void EditCentreDetailsPost_removeLogo_removes_logo()
        {
            // Given
            const string action = "removeLogo";
            var model = new EditCentreDetailsViewModel
            {
                BannerText = "Test banner text",
                CentreLogo = new byte[100]
            };

            // When
            var result = controller.EditCentreDetails(model, action);

            // Then
            result.Should().BeViewResult();
            var returnModel = (result as ViewResult)!.Model as EditCentreDetailsViewModel;
            returnModel!.CentreLogo.Should().BeNull();
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
