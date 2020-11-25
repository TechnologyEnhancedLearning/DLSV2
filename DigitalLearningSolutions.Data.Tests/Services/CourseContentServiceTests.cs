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
            // When
            const int candidateId = 9;
            const int customisationId = 259;
            var result = courseContentService.GetProgressId(candidateId, customisationId);

            // Then
            var expectedProgressId = 10;
            result.Should().Be(expectedProgressId);
        }

        [Test]
        public void Update_login_count_should_not_increment_login_if_no_new_session()
        {
            // When
            const int candidateId = 9;
            const int customisationId = 259;
            var progressId = courseContentService.GetProgressId(candidateId, customisationId);
            var expectedLoginCount = CourseContentTestHelper.GetLoginCount(progressId);
            courseContentService.UpdateLoginCountAndDuration(progressId);
            var result = CourseContentTestHelper.GetLoginCount(progressId);

            // Then
            result.Should().Be(expectedLoginCount);
        }

        [Test]
        public void Update_duration_should_not_increment_login_if_no_new_session()
        {
            // When
            const int candidateId = 9;
            const int customisationId = 259;
            var progressId = courseContentService.GetProgressId(candidateId, customisationId);
            var expectedDuration = CourseContentTestHelper.GetDuration(progressId);
            courseContentService.UpdateLoginCountAndDuration(progressId);
            var result = CourseContentTestHelper.GetDuration(progressId);

            // Then
            result.Should().Be(expectedDuration);
        }

        [Test]
        public void Update_login_count_should_increment_login_if_new_session()
        {
            // When
            const int candidateId = 9;
            const int customisationId = 259;
            const int duration = 5;
            const int active = 0;
            var loginTime = new DateTime(2010, 9, 23);
            var progressId = courseContentService.GetProgressId(candidateId, customisationId);
            var initialLoginCount = CourseContentTestHelper.GetLoginCount(progressId);
            CourseContentTestHelper.InsertSession(candidateId, customisationId, loginTime, duration, active);
            courseContentService.UpdateLoginCountAndDuration(progressId);
            var result = CourseContentTestHelper.GetLoginCount(progressId);

            // Then
            result.Should().Be(initialLoginCount + 1);
        }

        [Test]
        public void Update_duration_should_increment_login_if_new_session()
        {
            // When
            const int candidateId = 9;
            const int customisationId = 259;
            const int duration = 5;
            const int active = 0;
            var loginTime = new DateTime(2010, 9, 23);
            var progressId = courseContentService.GetProgressId(candidateId, customisationId);
            var initialDuration = CourseContentTestHelper.GetDuration(progressId);
            CourseContentTestHelper.InsertSession(candidateId, customisationId, loginTime, duration, active);
            courseContentService.UpdateLoginCountAndDuration(progressId);
            var result = CourseContentTestHelper.GetDuration(progressId);

            // Then
            result.Should().Be(initialDuration + duration);
        }
    }
}
