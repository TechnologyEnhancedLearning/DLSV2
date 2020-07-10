namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class LearningPortalControllerTests
    {
        private LearningPortalController controller;

        private IHeadlineFiguresService headlineFiguresService;

        [SetUp]
        public void SetUp()
        {
            headlineFiguresService = A.Fake<IHeadlineFiguresService>();
            controller = new LearningPortalController(headlineFiguresService);
        }

        [Test]
        public void Current_action_should_return_view_result()
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
            var result = controller.Current();

            // Then
            var expectedModel = new CurrentViewModel(headlineFigures);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }
    }
}
