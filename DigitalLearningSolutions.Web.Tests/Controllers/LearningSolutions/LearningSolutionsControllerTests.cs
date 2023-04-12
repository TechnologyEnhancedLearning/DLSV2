namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningSolutions
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.Controllers.LearningSolutions;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningSolutions;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    internal class LearningSolutionsControllerTests
    {
        private const int CandidateId = 11;
        private const int CentreId = 2;
        private ICentresDataService centresDataService = null!;
        private ICentresService centresService = null!;
        private IConfigDataService configDataService = null!;
        private LearningSolutionsController controller = null!;

        [SetUp]
        public void SetUp()
        {
            centresDataService = A.Fake<ICentresDataService>();
            configDataService = A.Fake<IConfigDataService>();
            centresService= A.Fake<ICentresService>();
            var logger = A.Fake<ILogger<LearningSolutionsController>>();

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim("learnCandidateID", CandidateId.ToString()),
                        new Claim("UserCentreID", CentreId.ToString()),
                    },
                    "mock"
                )
            );
            controller = new LearningSolutionsController(
                configDataService,
                logger,
                centresDataService,
                centresService
            )
            {
                ControllerContext = new ControllerContext
                { HttpContext = new DefaultHttpContext { User = user } },
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
            A.CallTo(() => centresDataService.GetBannerText(CentreId)).Returns(bannerText);

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
        public void StatusCode_should_render_gone_view_when_code_is_410()
        {
            // When
            var result = controller.StatusCode(410);

            // Then
            result.Should().BeViewResult().WithViewName("Error/Gone");
            controller.Response.StatusCode.Should().Be(410);
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
            A.CallTo(() => centresDataService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var result = controller.StatusCode(404);

            // Then
            var expectedModel = new ErrorViewModel(bannerText);
            result.Should().BeViewResult()
                .ModelAs<ErrorViewModel>().HelpText().Should().Be(expectedModel.HelpText());
        }

        [Test]
        public void StatusCode_should_set_banner_text_when_code_is_410()
        {
            // Given
            const string bannerText = "Banner text";
            A.CallTo(() => centresDataService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var result = controller.StatusCode(410);

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
            A.CallTo(() => centresDataService.GetBannerText(CentreId)).Returns(bannerText);

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
            A.CallTo(() => centresDataService.GetBannerText(CentreId)).Returns(bannerText);

            // When
            var result = controller.StatusCode(500);

            // Then
            var expectedModel = new ErrorViewModel(bannerText);
            result.Should().BeViewResult()
                .ModelAs<ErrorViewModel>().HelpText().Should().Be(expectedModel.HelpText());
        }

        [Test]
        public void AccessDenied_returns_AccessDenied_view()
        {
            // Given
            controller.WithMockUser(true, delegateId: null);

            // When
            var result = controller.AccessDenied();

            // Then
            result.Should().BeViewResult().WithViewName("Error/AccessDenied");
        }

        [Test]
        public void AccessDenied_redirects_to_Learning_Portal_when_user_is_delegate_only()
        {
            // Given
            controller.WithMockUser(true, adminId: null);

            // When
            var result = controller.AccessDenied();

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("LearningPortal")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void PleaseLogout_returns_default_view()
        {
            // When
            var result = controller.PleaseLogout();

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }
    }
}
