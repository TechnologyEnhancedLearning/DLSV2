namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningPortal
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using FluentAssertions;
    using NUnit.Framework;

    public class CurrentViewModelTests
    {
        private CurrentViewModel model;

        [SetUp]
        public void SetUp()
        {
            var currentCourses = new[] {
                new CurrentCourse { CustomisationID = 1, CourseName = "Course 1" },
                new CurrentCourse { CustomisationID = 2, CourseName = "Course 2" }
            };

            model = new CurrentViewModel(currentCourses, 1);
        }

        [TestCase(0, "Course 1", 1)]
        [TestCase(1, "Course 2", 2)]
        public void Current_courses_should_map_to_view_models_in_the_correct_order(
            int index,
            string expectedName,
            int expectedId)
        {
            var course = model.CurrentCourses.ElementAt(index);
            course.Name.Should().Be(expectedName);
            course.Id.Should().Be(expectedId);
        }
    }
}
