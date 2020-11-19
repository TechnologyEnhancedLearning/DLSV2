namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningMenu
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.LearningMenuController;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class LearningMenuControllerTests
    {
        private LearningMenuController controller;
        private ICourseContentService courseContentService;
        private IConfiguration config;
        private const int CandidateId = 11;
        private const int CentreId = 2;
        private const int CustomisationId = 12;

        [SetUp]
        public void SetUp()
        {
            var logger = A.Fake<ILogger<LearningMenuController>>();
            config = A.Fake<IConfiguration>();
            courseContentService = A.Fake<ICourseContentService>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("learnCandidateID", CandidateId.ToString()),
                new Claim("UserCentreID", CentreId.ToString())
            }, "mock"));
            controller = new LearningMenuController(logger, config, courseContentService)
            {
                ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext { User = user } }
            };
        }

        [Test]
        public void Index_should_render_view()
        {
            // Given
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(CustomisationId);
            A.CallTo(() => courseContentService.GetCourseContent(CustomisationId)).Returns(expectedCourseContent);

            // When
            var result = controller.Index(CustomisationId);

            // Then
            var expectedModel = new InitialMenuViewModel(expectedCourseContent);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Index_should_return_404_if_unknown_course()
        {
            // Given
            A.CallTo(() => courseContentService.GetCourseContent(CustomisationId)).Returns(null);

            // When
            var result = controller.Index(CustomisationId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void ContentViewer_should_render_view()
        {
            // When
            var result = controller.ContentViewer();

            // Then
            var expectedModel = new ContentViewerViewModel(config);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }
    }
}
