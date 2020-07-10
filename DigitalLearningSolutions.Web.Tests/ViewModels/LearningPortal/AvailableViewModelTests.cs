namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningPortal
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using FluentAssertions;
    using NUnit.Framework;

    public class AvailableViewModelTests
    {
        private AvailableViewModel model;

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

            model = new AvailableViewModel(headlineFigures);
        }

        [TestCase(0, "Centres", "centres", 339)]
        [TestCase(1, "Learners", "learners", 329025)]
        [TestCase(2, "Learning Hours", "learning-hours", 649911)]
        [TestCase(3, "Courses Completed", "courses-completed", 162263)]
        public void Available_courses_should_map_to_view_models_in_the_correct_order(
            int index,
            string expectedLabel,
            string expectedCssClassname,
            int expectedValue)
        {
            var course = model.AvailableCourses.ElementAt(index);
            course.Label.Should().Be(expectedLabel);
            course.CssClassname.Should().Be(expectedCssClassname);
            course.Value.Should().Be(expectedValue);
        }
    }
}
