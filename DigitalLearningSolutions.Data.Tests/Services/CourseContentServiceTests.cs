namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Transactions;
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
        public void Update_progress_should_not_increment_login_count_if_no_new_session()
        {
            // Given
            const int progressId = 10;
            var expectedLoginCount = courseContentTestHelper.GetLoginCount(progressId);

            using (new TransactionScope())
            {
                // When
                courseContentService.UpdateProgress(progressId);
                var result = courseContentTestHelper.GetLoginCount(progressId);

                // Then
                result.Should().Be(expectedLoginCount);
            }
        }

        [Test]
        public void Update_progress_should_not_increment_duration_if_no_new_session()
        {
            // Given
            const int progressId = 10;
            var expectedDuration = courseContentTestHelper.GetDuration(progressId);

            using (new TransactionScope())
            {
                // When
                courseContentService.UpdateProgress(progressId);
                var result = courseContentTestHelper.GetDuration(progressId);

                // Then
                result.Should().Be(expectedDuration);
            }
        }

        [Test]
        public void Update_progress_should_not_increment_login_count_if_session_time_is_before_first_submitted_time()
        {
            // Given
            const int candidateId = 9;
            const int customisationId = 259;
            const int progressId = 10;
            const int duration = 5;
            var loginTime = new DateTime(2010, 8, 23);
            var expectedLoginCount = courseContentTestHelper.GetLoginCount(progressId);

            using (new TransactionScope())
            {
                // When
                courseContentTestHelper.InsertSession(candidateId, customisationId, loginTime, duration);
                courseContentService.UpdateProgress(progressId);
                var result = courseContentTestHelper.GetLoginCount(progressId);

                // Then
                result.Should().Be(expectedLoginCount);
            }
        }

        [Test]
        public void Update_progress_should_not_increment_duration_if_session_time_is_before_first_submitted_time()
        {
            // Given
            const int candidateId = 9;
            const int customisationId = 259;
            const int progressId = 10;
            const int duration = 5;
            var loginTime = new DateTime(2010, 8, 23);
            var expectedDuration = courseContentTestHelper.GetDuration(progressId);

            using (new TransactionScope())
            {
                // When
                courseContentTestHelper.InsertSession(candidateId, customisationId, loginTime, duration);
                courseContentService.UpdateProgress(progressId);
                var result = courseContentTestHelper.GetDuration(progressId);

                // Then
                result.Should().Be(expectedDuration);
            }
        }

        [Test]
        public void Update_progress_should_increment_login_count_if_new_session()
        {
            // Given
            const int candidateId = 9;
            const int customisationId = 259;
            const int progressId = 10;
            const int duration = 5;
            var loginTime = new DateTime(2010, 9, 23);
            var initialLoginCount = courseContentTestHelper.GetLoginCount(progressId);

            using (new TransactionScope())
            {
                // When
                courseContentTestHelper.InsertSession(candidateId, customisationId, loginTime, duration);
                courseContentService.UpdateProgress(progressId);
                var result = courseContentTestHelper.GetLoginCount(progressId);

                // Then
                result.Should().Be(initialLoginCount + 1);
            }
        }

        [Test]
        public void Update_progress_should_increment_duration_if_new_session()
        {
            // Given
            const int candidateId = 9;
            const int customisationId = 259;
            const int progressId = 10;
            const int duration = 5;
            var loginTime = new DateTime(2010, 9, 23);
            var initialDuration = courseContentTestHelper.GetDuration(progressId);

            using (new TransactionScope())
            {
                // When
                courseContentTestHelper.InsertSession(candidateId, customisationId, loginTime, duration);
                courseContentService.UpdateProgress(progressId);
                var result = courseContentTestHelper.GetDuration(progressId);

                // Then
                result.Should().Be(initialDuration + duration);
            }
        }

        [Test]
        public void Update_progress_should_update_submitted_time()
        {
            // Given
            const int progressId = 10;
            var initialSubmittedTime = courseContentTestHelper.GetSubmittedTime(progressId);

            using (new TransactionScope())
            {
                // When
                courseContentService.UpdateProgress(progressId);
                var result = courseContentTestHelper.GetSubmittedTime(progressId);

                // Then
                courseContentTestHelper.IsApproximatelyNow(result).Should().BeTrue();
            }
        }
    }
}
