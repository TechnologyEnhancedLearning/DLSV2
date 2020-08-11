namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using System;
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.LearningPortalController;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public partial class LearningPortalControllerTests
    {
        private LearningPortalController controller;
        private ICentresService centresService;
        private IConfigService configService;
        private ICourseService courseService;
        private ISelfAssessmentService selfAssessmentService;
        private IUnlockService unlockService;
        private IConfiguration config;
        private const string BaseUrl = "https://www.dls.nhs.uk";
        private const int CandidateId = 254480;
        private const int CentreId = 2;

        [SetUp]
        public void SetUp()
        {
            centresService = A.Fake<ICentresService>();
            configService = A.Fake<IConfigService>();
            courseService = A.Fake<ICourseService>();
            selfAssessmentService = A.Fake<ISelfAssessmentService>();
            unlockService = A.Fake<IUnlockService>();
            var logger = A.Fake<ILogger<LearningPortalController>>();
            config = A.Fake<IConfiguration>();
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(BaseUrl);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("learnCandidateID", CandidateId.ToString()),
                new Claim("UserCentreID", CentreId.ToString())
            }, "mock"));
            controller = new LearningPortalController(
                centresService,
                configService,
                courseService,
                selfAssessmentService,
                unlockService,
                logger,
                config
            )
            {
                ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext { User = user } }
            };
        }

        [Test]
        public void Error_should_render_the_error_view()
        {
            // When
            var result = controller.Error();

            // Then
            result.Should().BeViewResult().WithViewName("Error/UnknownError");
            controller.Response.StatusCode.Should().Be(500);
        }

        [Test]
        public void Error_should_pass_the_banner_text()
        {
            // Given
            const string bannerText = "Banner text";
            A.CallTo(() => centresService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var result = controller.Error();

            // Then
            var expectedModel = new ErrorViewModel(bannerText);
            result.Should().BeViewResult()
                .ModelAs<ErrorViewModel>().HelpText().Should().Be(expectedModel.HelpText());
        }

        [Test]
        public void StatusCode_should_render_not_found_view_when_code_is_404()
        {
            // When
            var result = controller.StatusCode(404);

            // Then
            result.Should().BeViewResult().WithViewName("Error/PageNotFound");
            controller.Response.StatusCode.Should().Be(404);
        }

        [Test]
        public void StatusCode_should_render_forbidden_view_when_code_is_403()
        {
            // When
            var result = controller.StatusCode(403);

            // Then
            result.Should().BeViewResult().WithViewName("Error/Forbidden");
            controller.Response.StatusCode.Should().Be(403);
        }

        [Test]
        public void StatusCode_should_render_unknown_error_view_when_code_is_500()
        {
            // When
            var result = controller.StatusCode(500);

            // Then
            result.Should().BeViewResult().WithViewName("Error/UnknownError");
            controller.Response.StatusCode.Should().Be(500);
        }

        [Test]
        public void StatusCode_should_set_banner_text_when_code_is_404()
        {
            // Given
            const string bannerText = "Banner text";
            A.CallTo(() => centresService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var result = controller.StatusCode(404);

            // Then
            var expectedModel = new ErrorViewModel(bannerText);
            result.Should().BeViewResult()
                .ModelAs<ErrorViewModel>().HelpText().Should().Be(expectedModel.HelpText());
        }

        [Test]
        public void StatusCode_should_set_banner_text_when_code_is_403()
        {
            // Given
            const string bannerText = "Banner text";
            A.CallTo(() => centresService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var result = controller.StatusCode(403);

            // Then
            var expectedModel = new ErrorViewModel(bannerText);
            result.Should().BeViewResult()
                .ModelAs<ErrorViewModel>().HelpText().Should().Be(expectedModel.HelpText());
        }

        [Test]
        public void StatusCode_should_set_banner_text_when_code_is_500()
        {
            // Given
            const string bannerText = "Banner text";
            A.CallTo(() => centresService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var result = controller.StatusCode(500);

            // Then
            var expectedModel = new ErrorViewModel(bannerText);
            result.Should().BeViewResult()
                .ModelAs<ErrorViewModel>().HelpText().Should().Be(expectedModel.HelpText());
        }
    }
}
