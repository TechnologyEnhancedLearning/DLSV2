namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningPortal
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available;
    using FluentAssertions;
    using NUnit.Framework;

    public class AvailableViewModelTests
    {
        private AvailableViewModel model;

        [SetUp]
        public void SetUp()
        {
            var availableCourses = new[] {
                new Course { Id = 1, Name = "Course 1" },
                new Course { Id = 2, Name = "Course 2" }
            };

            model = new AvailableViewModel(availableCourses, null);
        }

        [TestCase(0, "Course 1", 1)]
        [TestCase(1, "Course 2", 2)]
        public void Available_courses_should_map_to_view_models_in_the_correct_order(
            int index,
            string expectedName,
            int expectedId)
        {
            var course = model.AvailableCourses.ElementAt(index);
            course.Name.Should().Be(expectedName);
            course.Id.Should().Be(expectedId);
        }
    }
}
