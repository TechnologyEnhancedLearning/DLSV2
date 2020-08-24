namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningPortal
{
    using System.Linq;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class AvailablePageViewModelTests
    {
        private readonly IConfiguration config = A.Fake<IConfiguration>();
        private AvailablePageViewModel model;

        [SetUp]
        public void SetUp()
        {
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns("http://www.dls.nhs.uk");
            var courses = new[] {
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "First course"),
                AvailableCourseHelper.CreateDefaultAvailableCourse(courseName: "Second course")
            };

            model = new AvailablePageViewModel(courses, config, null);
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
    }
}
