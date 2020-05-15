namespace DigitalLearningSolutions.Web.Tests.ViewModels.Home
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ViewModels.Home;
    using FluentAssertions;
    using NUnit.Framework;

    public class IndexViewModelTests
    {
        private IndexViewModel model;

        [SetUp]
        public void SetUp()
        {
            var headlineFigures = new HeadlineFigures
            {
                ActiveCentres = 339,
                Delegates = 329025,
                LearningTime = 649911,
                Completions = 162263
            };

            model = new IndexViewModel(headlineFigures);
        }

        [TestCase(0, "Centres", "centres", 339)]
        [TestCase(1, "Learners", "learners", 329025)]
        [TestCase(2, "Learning Hours", "learning-hours", 649911)]
        [TestCase(3, "Courses Completed", "courses-completed", 162263)]
        public void Headline_figures_should_map_to_view_models_in_the_correct_order(
            int index,
            string expectedLabel,
            string expectedCssClassname,
            int expectedValue)
        {
            var headlineFigure = model.HeadlineFigures.ElementAt(index);
            headlineFigure.Label.Should().Be(expectedLabel);
            headlineFigure.CssClassname.Should().Be(expectedCssClassname);
            headlineFigure.Value.Should().Be(expectedValue);
        }
    }
}
