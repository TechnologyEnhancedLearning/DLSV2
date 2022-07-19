namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.CourseCompletion;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    internal class CourseCompletionServiceTests
    {
        private CourseCompletionService courseCompletionService;
        private CourseContentTestHelper courseContentTestHelper;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            courseCompletionService = new CourseCompletionService(connection);
            courseContentTestHelper = new CourseContentTestHelper(connection);
        }

        [Test]
        public void Get_course_completion_of_assessed_course_should_return_course_completion()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 252646;
                const int customisationId = 21452;
                courseContentTestHelper.UpdateIncludeCertification(customisationId, true);

                // When
                var result = courseCompletionService.GetCourseCompletion(candidateId, customisationId);

                // Then
                result.Should().BeEquivalentTo(new CourseCompletion(
                    customisationId,
                    "TNA v0.1",
                    "RiO Mersey Care & NWBH",
                    DateTime.Parse("2017-07-26 07:56:15.273"),
                    null,
                    1,
                    true,
                    80,
                    85,
                    100,
                    22,
                    1,
                    425 / 31.0,
                    9,
                    13
                ));
            }
        }

        [Test]
        public void Get_course_completion_of_course_should_return_when_all_tutorials_statuses_are_0()
        {
            // CustomisationTutorials.Status = 0 for all CustomisationTutorial records where CustomisationID = 26696

            using (new TransactionScope())
            {
                // Given
                const int candidateId = 210962;
                const int customisationId = 26696;
                courseContentTestHelper.UpdateIncludeCertification(customisationId, true);

                // When
                var result = courseCompletionService.GetCourseCompletion(candidateId, customisationId);

                // Then
                result.Should().BeEquivalentTo(new CourseCompletion(
                    customisationId,
                    "Recruitment and Selection",
                    "RandS",
                    null,
                    null,
                    2,
                    false,
                    0,
                    85,
                    0,
                    4,
                    1,
                    0,
                    0,
                    9
                ));
            }
        }

        [Test]
        public void Get_course_completion_should_not_use_archived_tutorials_in_percentageTutorialsCompleted()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 11;
                const int customisationId = 15937;
                courseContentTestHelper.UpdateIncludeCertification(customisationId, true);

                const int tutorialsComplete = 36;
                const int tutorialsAvailable = 196;

                // When
                var result = courseCompletionService.GetCourseCompletion(candidateId, customisationId);

                // Then
                result.PercentageTutorialsCompleted.Should().Be(100.0 * tutorialsComplete / tutorialsAvailable);
            }
        }

        [Test]
        public void Get_course_completion_of_evaluated_course_should_return_course_completion()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 118938;
                const int customisationId = 9117;
                courseContentTestHelper.UpdateIncludeCertification(customisationId, true);

                // When
                var result = courseCompletionService.GetCourseCompletion(candidateId, customisationId);

                // Then
                result.Should().BeEquivalentTo(new CourseCompletion(
                    customisationId,
                    "Entry Level - Win 7, Office 2010",
                    "Basic Introductory Course",
                    DateTime.Parse("2014-08-26 15:23:58.620"),
                    DateTime.Parse("2014-08-26 15:26:03.260"),
                    0,
                    true,
                    85,
                    85,
                    100,
                    53,
                    1,
                    50 / 11.0,
                    10,
                    10
                ));
            }
        }

        [Test]
        public void Get_course_completion_should_return_course_completion_for_course_without_tutorials()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 1;
                const int customisationId = 100;
                courseContentTestHelper.UpdateIncludeCertification(customisationId, true);

                // When
                var result = courseCompletionService.GetCourseCompletion(candidateId, customisationId);

                // Then
                result.Should().BeEquivalentTo(new CourseCompletion(
                    customisationId,
                    "Entry Level - Win XP, Office 2003/07 OLD",
                    "Standard",
                    null,
                    null,
                    0,
                    true,
                    85,
                    85,
                    100,
                    29,
                    0,
                    0,
                    6,
                    10
                ));
            }
        }

        [Test]
        public void Get_course_completion_should_return_null_when_course_does_not_have_certification()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 100;
            // Note helper method to add certification has not been run in this test

            // When
            var result = courseCompletionService.GetCourseCompletion(candidateId, customisationId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_course_completion_should_return_null_when_course_does_not_exist()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1000000;

            // When
            var result = courseCompletionService.GetCourseCompletion(candidateId, customisationId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_course_completion_should_return_default_completion_when_candidate_not_enrolled()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 100;
                const int customisationId = 100;
                courseContentTestHelper.UpdateIncludeCertification(customisationId, true);

                // When
                var result = courseCompletionService.GetCourseCompletion(candidateId, customisationId);

                // Then
                result.Should().BeEquivalentTo(new CourseCompletion(
                    customisationId,
                    "Entry Level - Win XP, Office 2003/07 OLD",
                    "Standard",
                    null,
                    null,
                    0,
                    true,
                    85,
                    85,
                    100,
                    null,
                    0,
                    0,
                    0,
                    10
                ));
            }
        }
    }
}
