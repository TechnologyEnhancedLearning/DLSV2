namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.ViewModels.Home;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class HomeControllerTests
    {
        private HomeController controller;

        private IHeadlineFiguresService headlineFiguresService;

        [SetUp]
        public void SetUp()
        {
            headlineFiguresService = A.Fake<IHeadlineFiguresService>();
            controller = new HomeController(headlineFiguresService);
        }

        [Test]
        public void Index_action_should_return_view_result()
        {
            // Given
            var headlineFigures = new HeadlineFigures
            {
                ActiveCentres = 339,
                Delegates = 329025,
                LearningTime = 649911,
                Completions = 162263
            };
            A.CallTo(() => headlineFiguresService.GetHeadlineFigures()).Returns(headlineFigures);

            // When
            var result = controller.Index();

            // Then
            var expectedModel = new IndexViewModel(headlineFigures);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }
    }
}
