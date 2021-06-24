namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningPortal
{
    using System.Linq;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available;
    using FluentAssertions;
    using NUnit.Framework;

    public class AvailablePageViewModelTests
    {
        private AvailablePageViewModel model = null!;

        [SetUp]
        public void SetUp()
        {
            var courses = new[] {
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "First course"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "Second course")
            };

            model = new AvailablePageViewModel(
                courses,
                null,
                "Name",
                "Ascending",
                null,
                1
            );
        }

        [TestCase(0, "First course")]
        [TestCase(1, "Second course")]
        public void Available_courses_should_map_to_view_models_in_the_correct_order(
            int index,
            string expectedName)
        {
            var course = model.AvailableCourses.ElementAt(index);
            course.Name.Should().Be(expectedName);
        }

        [Test]
        public void Available_courses_should_default_to_returning_the_first_ten_courses()
        {
            var courses = new[] {
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "a course 1"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "b course 2"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "c course 3"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "d course 4"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "e course 5"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "f course 6"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "g course 7"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "h course 8"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "i course 9"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "j course 10"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "k course 11"),
            };

            model = new AvailablePageViewModel(
                courses,
                null,
                "Name",
                "Ascending",
                null,
                1
            );

            model.AvailableCourses.Count().Should().Be(10);
            model.AvailableCourses.FirstOrDefault(course => course.Name == "k course 11").Should().BeNull();
        }

        [Test]
        public void Available_courses_should_correctly_return_the_second_page_of_courses()
        {
            var courses = new[] {
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "a course 1"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "b course 2"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "c course 3"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "d course 4"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "e course 5"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "f course 6"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "g course 7"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "h course 8"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "i course 9"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "j course 10"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "k course 11"),
            };

            model = new AvailablePageViewModel(
                courses,
                null,
                "Name",
                "Ascending",
                null,
                2
            );

            model.AvailableCourses.Count().Should().Be(1);
            model.AvailableCourses.First().Name.Should().Be("k course 11");
        }
    }
}
