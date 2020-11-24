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
            const int customisationId = 1;
            var result = courseContentService.GetCourseContent(customisationId);

            // Then
            var expectedCourse = new CourseContent(
                1,
                "Entry Level - Win XP, Office 2003/07 OLD",
                "Standard",
                "N/A",
                "North West Boroughs Healthcare NHS Foundation Trust",
                "xxxxxxxxxxxxxxxxxxxx"
            );
            result.Should().BeEquivalentTo(expectedCourse);
        }
    }
}
