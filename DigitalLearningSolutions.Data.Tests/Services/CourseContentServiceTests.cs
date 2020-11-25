namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
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
        private CourseContentTestHelper courseContentTestHelper;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<CourseContentService>>();
            courseContentService = new CourseContentService(connection, logger);
            courseContentTestHelper = new CourseContentTestHelper(connection);
        }

        [Test]
        public void Get_course_content_should_return_course()
        {
            // When
            const int customisationId = 1;
            var result = courseContentService.GetCourseContent(customisationId);

            // Then
            var expectedCourse = new CourseContent
            {
                Id = 1,
                CustomisationName = "Standard",
                ApplicationName = "Entry Level - Win XP, Office 2003/07 OLD"
            };
            result.Should().BeEquivalentTo(expectedCourse);
        }

        [Test]
        public void Get_progress_id_should_return_progress_id()
        {
            // Given
            const int candidateId = 9;
            const int customisationId = 259;

            // When
            var result = courseContentService.GetProgressId(candidateId, customisationId);

            // Then
            const int expectedProgressId = 10;
            result.Should().Be(expectedProgressId);
        }

        [Test]
        public void Update_login_count_should_not_increment_login_count_if_no_new_session()
        {
            // Given
            const int progressId = 10;
            var expectedLoginCount = courseContentTestHelper.GetLoginCount(progressId);

            // When
            courseContentService.UpdateLoginCountAndDuration(progressId);
            var result = courseContentTestHelper.GetLoginCount(progressId);

            // Then
            result.Should().Be(expectedLoginCount);
        }

        [Test]
        public void Update_duration_should_not_increment_duration_if_no_new_session()
        {
            // Given
            const int progressId = 10;
            var expectedDuration = courseContentTestHelper.GetDuration(progressId);

            // When
            courseContentService.UpdateLoginCountAndDuration(progressId);
            var result = courseContentTestHelper.GetDuration(progressId);

            // Then
            result.Should().Be(expectedDuration);
        }

        [Test]
        public void Update_login_count_should_not_increment_login_count_if_session_time_not_in_range()
        {
            // Given
            const int candidateId = 9;
            const int customisationId = 259;
            const int progressId = 10;
            const int duration = 5;
            var loginTime = new DateTime(2011, 9, 23);
            var expectedLoginCount = courseContentTestHelper.GetLoginCount(progressId);

            // When
            courseContentTestHelper.InsertSession(candidateId, customisationId, loginTime, duration);
            courseContentService.UpdateLoginCountAndDuration(progressId);
            var result = courseContentTestHelper.GetLoginCount(progressId);

            // Then
            result.Should().Be(expectedLoginCount);
        }

        [Test]
        public void Update_duration_should_not_increment_duration_if_session_time_not_in_range()
        {
            // Given
            const int candidateId = 9;
            const int customisationId = 259;
            const int progressId = 10;
            const int duration = 5;
            var loginTime = new DateTime(2011, 9, 23);
            var expectedDuration = courseContentTestHelper.GetDuration(progressId);

            // When
            courseContentTestHelper.InsertSession(candidateId, customisationId, loginTime, duration);
            courseContentService.UpdateLoginCountAndDuration(progressId);
            var result = courseContentTestHelper.GetDuration(progressId);
            
            // Then
            result.Should().Be(expectedDuration);
        }

        [Test]
        public void Update_login_count_should_increment_login_count_if_new_session()
        {
            // Given
            const int candidateId = 9;
            const int customisationId = 259;
            const int progressId = 10;
            const int duration = 5;
            var loginTime = new DateTime(2010, 9, 23);
            var initialLoginCount = courseContentTestHelper.GetLoginCount(progressId);

            // When
            courseContentTestHelper.InsertSession(candidateId, customisationId, loginTime, duration);
            courseContentService.UpdateLoginCountAndDuration(progressId);
            var result = courseContentTestHelper.GetLoginCount(progressId);

            // Then
            result.Should().Be(initialLoginCount + 1);
        }

        [Test]
        public void Update_duration_should_increment_duration_if_new_session()
        {
            // Given
            const int candidateId = 9;
            const int customisationId = 259;
            const int progressId = 10;
            const int duration = 5;
            var loginTime = new DateTime(2010, 9, 23);
            var initialDuration = courseContentTestHelper.GetDuration(progressId);

            // When
            courseContentTestHelper.InsertSession(candidateId, customisationId, loginTime, duration);
            courseContentService.UpdateLoginCountAndDuration(progressId);
            var result = courseContentTestHelper.GetDuration(progressId);

            // Then
            result.Should().Be(initialDuration + duration);
        }
    }
}
