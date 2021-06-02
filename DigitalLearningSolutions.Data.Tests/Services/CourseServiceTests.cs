namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Transactions;
    using DigitalLearningSolutions.Data.Mappers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using NUnit.Framework;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;

    public class CourseServiceTests
    {
        private CourseService courseService;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MapperHelper.SetUpFluentMapper();
        }

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<CourseService>>();
            courseService = new CourseService(connection, logger);
        }

        [Test]
        public void Get_current_courses_should_return_courses_for_candidate()
        {
            // When
            const int candidateId = 1;
            var result = courseService.GetCurrentCourses(candidateId).ToList();

            // Then
            var expectedFirstCourse = new CurrentCourse
            {
                Name = "Office 2013 Essentials for the Workplace - Erin Test 01",
                Id = 15853,
                LastAccessed = new DateTime(2019, 1, 22, 8, 20, 39, 133),
                StartedDate = new DateTime(2016, 7, 6, 11, 12, 15, 393),
                DiagnosticScore = 0,
                IsAssessed = true,
                HasDiagnostic = true,
                HasLearning = true,
                Passes = 2,
                Sections = 6,
                CompleteByDate = new DateTime(2018, 12, 31, 0, 0, 0, 0),
                GroupCustomisationId = 0,
                SupervisorAdminId = 0,
                ProgressID = 173218,
                EnrollmentMethodID = 1,
                PLLocked = false
            };
            result.Should().HaveCount(4);
            result.First().Should().BeEquivalentTo(expectedFirstCourse);
        }

        [Test]
        public void Get_completed_courses_should_return_courses_for_candidate()
        {
            // When
            const int candidateId = 1;
            var result = courseService.GetCompletedCourses(candidateId).ToList();

            // Then
            var expectedFirstCourse = new CompletedCourse
            {
                Name = "Staying Safe Online Test PLA Issue - test",
                Id = 25140,
                StartedDate = new DateTime(2018, 5, 29, 9, 11, 45, 943),
                Completed = new DateTime(2018, 5, 29, 14, 28, 5, 557),
                LastAccessed = new DateTime(2018, 5, 29, 14, 28, 5, 020),
                Evaluated = new DateTime(2019, 4, 5, 7, 10, 28, 507),
                DiagnosticScore = 0,
                IsAssessed = true,
                HasDiagnostic = true,
                HasLearning = true,
                Passes = 1,
                Sections = 2,
                ProgressID = 251571,
            };
            result.Should().HaveCount(15);
            result.First().Should().BeEquivalentTo(expectedFirstCourse);
        }

        [Test]
        public void Get_available_courses_should_return_courses_for_candidate()
        {
            // When
            const int candidateId = 254480;
            const int centreId = 101;
            var result = courseService.GetAvailableCourses(candidateId, centreId).ToList();

            // Then
            var expectedFirstCourse = new AvailableCourse
            {
                Name = "5 Jan Test - New",
                Id = 18438,
                Brand = "Local content",
                Topic = "Microsoft Office",
                Category = "Digital Workplace",
                DelegateStatus = 0,
                HasLearning = true,
                HasDiagnostic = true,
                IsAssessed = true
            };
            result.Should().HaveCountGreaterOrEqualTo(1);
            result.First().Should().BeEquivalentTo(expectedFirstCourse);
        }

        [TestCase(4, "Office 2010")]
        [TestCase(2, null)]
        public void Get_available_courses_should_validate_category(
            int index,
            string? expectedValidatedCategory
        )
        {
            // When
            const int candidateId = 254480;
            const int centreId = 101;
            var result = courseService.GetAvailableCourses(candidateId, centreId).ToList();

            // Then
            result[index].Category.Should().Be(expectedValidatedCategory);
        }

        [TestCase(4, "Word")]
        [TestCase(2, null)]
        public void Get_available_courses_should_validate_topic(
            int index,
            string? expectedValidatedTopic
        )
        {
            // When
            const int candidateId = 254480;
            const int centreId = 101;
            var result = courseService.GetAvailableCourses(candidateId, centreId).ToList();

            // Then
            result[index].Topic.Should().Be(expectedValidatedTopic);
        }

        [Test]
        public void Get_available_courses_should_return_no_courses_if_no_centre()
        {
            // When
            const int candidateId = 1;
            var result = courseService.GetAvailableCourses(candidateId, null).ToList();

            // Then
            result.Should().BeEmpty();
        }

        [Test]
        public void Set_complete_by_date_should_update_db()
        {
            // Given
            const int candidateId = 1;
            const int progressId = 94323;
            var newCompleteByDate = new DateTime(2020, 7, 29);

            using (new TransactionScope())
            {
                // When
                courseService.SetCompleteByDate(progressId, candidateId, newCompleteByDate);
                var modifiedCourse = courseService.GetCurrentCourses(candidateId).ToList().First(c => c.ProgressID == progressId);

                // Then
                modifiedCourse.CompleteByDate.Should().Be(newCompleteByDate);
            }
        }

        [Test]
        public void Remove_current_course_should_prevent_a_course_from_being_returned()
        {
            using (new TransactionScope())
            {
                // Given
                const int progressId = 94323;
                const int candidateId = 1;

                // When
                courseService.RemoveCurrentCourse(progressId, candidateId);
                var courseReturned = courseService.GetCurrentCourses(candidateId).ToList().Any(c => c.ProgressID == progressId);

                // Then
                courseReturned.Should().BeFalse();
            }
        }

        [Test]
        public void GetNumberOfActiveCoursesAtCentre_returns_expected_count()
        {
            // When
            var count = courseService.GetNumberOfActiveCoursesAtCentre(2);

            // Then
            count.Should().Be(38);
        }
    }
}
