namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Mappers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class CourseDataServiceTests
    {
        private CourseDataService courseDataService = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MapperHelper.SetUpFluentMapper();
        }

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<CourseDataService>>();
            courseDataService = new CourseDataService(connection, logger);
        }

        [Test]
        public void Get_current_courses_should_return_courses_for_candidate()
        {
            // When
            const int candidateId = 1;
            var result = courseDataService.GetCurrentCourses(candidateId).ToList();

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
            var result = courseDataService.GetCompletedCourses(candidateId).ToList();

            // Then
            var expectedFirstCourse = new CompletedCourse
            {
                Name = "Staying Safe Online Test PLA Issue - test",
                Id = 25140,
                StartedDate = new DateTime(2018, 5, 29, 9, 11, 45, 943),
                Completed = new DateTime(2018, 5, 29, 14, 28, 5, 557),
                LastAccessed = new DateTime(2018, 5, 29, 14, 28, 5, 020),
                Evaluated = new DateTime(2019, 4, 5, 7, 10, 28, 507),
                ArchivedDate = null,
                DiagnosticScore = 0,
                IsAssessed = true,
                HasDiagnostic = true,
                HasLearning = true,
                Passes = 1,
                Sections = 2,
                ProgressID = 251571
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
            var result = courseDataService.GetAvailableCourses(candidateId, centreId).ToList();

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
            var result = courseDataService.GetAvailableCourses(candidateId, centreId).ToList();

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
            var result = courseDataService.GetAvailableCourses(candidateId, centreId).ToList();

            // Then
            result[index].Topic.Should().Be(expectedValidatedTopic);
        }

        [Test]
        public void Get_available_courses_should_return_no_courses_if_no_centre()
        {
            // When
            const int candidateId = 1;
            var result = courseDataService.GetAvailableCourses(candidateId, null).ToList();

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
                courseDataService.SetCompleteByDate(progressId, candidateId, newCompleteByDate);
                var modifiedCourse = courseDataService.GetCurrentCourses(candidateId).ToList()
                    .First(c => c.ProgressID == progressId);

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
                courseDataService.RemoveCurrentCourse(progressId, candidateId);
                var courseReturned = courseDataService.GetCurrentCourses(candidateId).ToList()
                    .Any(c => c.ProgressID == progressId);

                // Then
                courseReturned.Should().BeFalse();
            }
        }

        [Test]
        public void GetNumberOfActiveCoursesAtCentre_returns_expected_count()
        {
            // When
            var count = courseDataService.GetNumberOfActiveCoursesAtCentreForCategory(2, 0);

            // Then
            count.Should().Be(38);
        }

        [Test]
        public void GetNumberOfActiveCoursesAtCentre_with_filtered_category_returns_expected_count()
        {
            // When
            var count = courseDataService.GetNumberOfActiveCoursesAtCentreForCategory(2, 2);

            // Then
            count.Should().Be(3);
        }

        [Test]
        public void GetCourseStatisticsAtCentreForCategoryID_should_return_course_statistics_correctly()
        {
            // Given
            const int centreId = 101;
            const int categoryId = 0;

            // When
            var result = courseDataService.GetCourseStatisticsAtCentreForCategoryId(centreId, categoryId).ToList();

            // Then
            var expectedFirstCourse = new CourseStatistics
            {
                CustomisationId = 100,
                CentreId = 101,
                Active = false,
                AllCentres = false,
                ApplicationName = "Entry Level - Win XP, Office 2003/07 OLD",
                CustomisationName = "Standard",
                DelegateCount = 25,
                AllAttempts = 49,
                AttemptsPassed = 34,
                CompletedCount = 5,
                HideInLearnerPortal = false,
                CategoryName = "Office 2007",
                LearningMinutes = "N/A"
            };

            result.Should().HaveCount(260);
            result.First().Should().BeEquivalentTo(expectedFirstCourse);
        }

        [Test]
        public void GetCourseDetailsByIdAtCentreForCategoryId_should_return_course_details_correctly()
        {
            // Given
            const int customisationId = 100;
            const int centreId = 101;
            const int categoryId = 0;
            var fixedCreationDateTime = DateTime.UtcNow;
            var expectedLastAccess = new DateTime(2014, 03, 31, 13, 00, 23, 457);
            var expectedCourseDetails = CourseDetailsTestHelper.GetDefaultCourseDetails(
                createdTime: fixedCreationDateTime,
                lastAccessed: expectedLastAccess
            );

            // When
            var result =
                courseDataService.GetCourseDetails(customisationId, centreId, categoryId)!;
            // Overwrite the created time as it is populated by a default constraint and not consistent over different databases
            result.CreatedTime = fixedCreationDateTime;

            // Then
            result.Should().BeEquivalentTo(expectedCourseDetails);
        }
    }
}
