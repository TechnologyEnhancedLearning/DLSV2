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
        public void Get_course_content_of_partially_complete_course_should_return_course()
        {
            // When
            const int candidateId = 22044;
            const int customisationId = 4169;
            var result = courseContentService.GetCourseContent(candidateId, customisationId);

            // Then
            var expectedCourse = new CourseContent(
                4169,
                "Level 2 - Microsoft Excel 2010",
                "MOS Excel 2010 CORE",
                "5h 49m",
                "Northumbria Healthcare NHS Foundation Trust",
                "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            );
            expectedCourse.Sections.AddRange(
                new[]
                {
                    new CourseSection("Viewing workbooks", true, 12.5),
                    new CourseSection("Manipulating worksheets", true, 20),
                    new CourseSection("Manipulating information", true, 25),
                    new CourseSection("Using formulas", true, 100 / 3.0),
                    new CourseSection("Using functions", true, 400 / 7.0),
                    new CourseSection("Managing formulas and functions", true, 0),
                    new CourseSection("Working with data", true, 0),
                    new CourseSection("Formatting cells and worksheets", true, 0),
                    new CourseSection("Formatting numbers", true, 0),
                    new CourseSection("Working with charts", true, 0),
                    new CourseSection("Working with illustrations", true, 0),
                    new CourseSection("Collaborating with others", true, 0),
                    new CourseSection("Preparing to print", true, 0)
                }
            );
            result.Should().BeEquivalentTo(expectedCourse);
        }

        [Test]
        public void Get_course_content_of_unstarted_course_should_return_course()
        {
            // When
            const int candidateId = 22044000;
            const int customisationId = 4169;
            var result = courseContentService.GetCourseContent(candidateId, customisationId);

            // Then
            var expectedCourse = new CourseContent(
                4169,
                "Level 2 - Microsoft Excel 2010",
                "MOS Excel 2010 CORE",
                "5h 49m",
                "Northumbria Healthcare NHS Foundation Trust",
                "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            );
            expectedCourse.Sections.AddRange(
                new[]
                {
                    new CourseSection("Viewing workbooks", true, 0),
                    new CourseSection("Manipulating worksheets", true, 0),
                    new CourseSection("Manipulating information", true, 0),
                    new CourseSection("Using formulas", true, 0),
                    new CourseSection("Using functions", true, 0),
                    new CourseSection("Managing formulas and functions", true, 0),
                    new CourseSection("Working with data", true, 0),
                    new CourseSection("Formatting cells and worksheets", true, 0),
                    new CourseSection("Formatting numbers", true, 0),
                    new CourseSection("Working with charts", true, 0),
                    new CourseSection("Working with illustrations", true, 0),
                    new CourseSection("Collaborating with others", true, 0),
                    new CourseSection("Preparing to print", true, 0)
                }
            );
            result.Should().BeEquivalentTo(expectedCourse);
        }

        [Test]
        public void Get_course_content_of_course_without_learning_should_return_course()
        {
            // When
            const int customisationId = 2921;
            const int candidateId = 22044;
            var result = courseContentService.GetCourseContent(candidateId, customisationId);

            // Then
            var expectedCourse = new CourseContent(
                2921,
                "Level 2 - Microsoft Outlook 2007",
                "MOST OUTLOOK CORE 2007",
                "46m",
                "Northumbria Healthcare NHS Foundation Trust",
                "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            );
            expectedCourse.Sections.AddRange(
                new[]
                {
                    new CourseSection("Introducing Outlook", false, 0),
                    new CourseSection("Writing and sending emails", false, 0),
                    new CourseSection("Managing emails", false, 0),
                    new CourseSection("Using the Calendar", true, 300/ 11.0),
                    new CourseSection("Working with Contacts", false, 0),
                    new CourseSection("Using Tasks", false, 0),
                    new CourseSection("Using Notes and  the  Journal", true, 0),
                }
            );
            result.Should().BeEquivalentTo(expectedCourse);
        }

        [Test]
        public void Get_course_content_of_unstarted_course_without_learning_should_return_course()
        {
            // When
            const int customisationId = 2921;
            const int candidateId = 22044000;
            var result = courseContentService.GetCourseContent(candidateId, customisationId);

            // Then
            var expectedCourse = new CourseContent(
                2921,
                "Level 2 - Microsoft Outlook 2007",
                "MOST OUTLOOK CORE 2007",
                "46m",
                "Northumbria Healthcare NHS Foundation Trust",
                "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            );
            expectedCourse.Sections.AddRange(
                new[]
                {
                    new CourseSection("Introducing Outlook", false, 0),
                    new CourseSection("Writing and sending emails", false, 0),
                    new CourseSection("Managing emails", false, 0),
                    new CourseSection("Using the Calendar", true, 0),
                    new CourseSection("Working with Contacts", false, 0),
                    new CourseSection("Using Tasks", false, 0),
                    new CourseSection("Using Notes and  the  Journal", true, 0),
                }
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
        public void Does_progress_exist_returns_true_for_existing_progress()
        {
            // Given
            const int candidateId = 9;
            const int customisationId = 259;

            // When
            var result = courseContentService.DoesProgressExist(candidateId, customisationId);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void Does_progress_exist_returns_false_for_non_existing_progress()
        {
            // Given
            const int candidateId = 123;
            const int customisationId = 123;

            // When
            var result = courseContentService.DoesProgressExist(candidateId, customisationId);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void Insert_new_progress_should_insert_progress()
        {
            // Given
            const int candidateId = 187251;
            const int customisationId = 17468;
            const int centreId = 549;
            var initialDoesProgressExist = courseContentService.DoesProgressExist(candidateId, customisationId);

            using (new TransactionScope())
            {
                // When
                courseContentService.InsertNewProgress(candidateId, customisationId, centreId);
                var result = courseContentService.DoesProgressExist(candidateId, customisationId);

                //Then
                initialDoesProgressExist.Should().BeFalse();
                result.Should().BeTrue();
            }
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
