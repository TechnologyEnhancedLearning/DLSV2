namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningMenu
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Web.Controllers.LearningMenuController;
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
        private IConfiguration config;
        private const int CandidateId = 11;
        private const int CentreId = 2;
        private const int CustomisationId = 12;

        [SetUp]
        public void SetUp()
        {
            var logger = A.Fake<ILogger<LearningMenuController>>();
            config = A.Fake<IConfiguration>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("learnCandidateID", CandidateId.ToString()),
                new Claim("UserCentreID", CentreId.ToString())
            }, "mock"));
            controller = new LearningMenuController(logger, config)
            {
                ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext { User = user } }
            };
        }

        [Test]
        public void Index_should_render_view()
        {
            // When
            var result = controller.Index(CustomisationId);

            // Then
            var expectedModel = new InitialMenuViewModel(CustomisationId);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
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
