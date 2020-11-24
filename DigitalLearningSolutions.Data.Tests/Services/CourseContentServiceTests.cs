namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Models.CourseContent;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    internal class CourseContentServiceTests
    {
        private CourseContentService courseContentService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<CourseContentService>>();
            courseContentService = new CourseContentService(connection, logger);
        }

        [Test]
        public void Get_course_content_should_return_course()
        {
            // When
            const int customisationId = 1604;
            var result = courseContentService.GetCourseContent(customisationId);

            // Then
            var expectedCourse = new CourseContent(
                1604,
                "Level 2 - Microsoft Word 2007",
                "Word Core 07 Testing",
                "3h 56m",
                "Northumbria Healthcare NHS Foundation Trust",
                "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            );
            result.Should().BeEquivalentTo(expectedCourse);
        }
    }
}
