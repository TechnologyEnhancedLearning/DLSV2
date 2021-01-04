namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using DigitalLearningSolutions.Data.Models.CourseCompletion;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    internal class CourseCompletionServiceTests
    {
        private CourseCompletionService courseCompletionService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            courseCompletionService = new CourseCompletionService(connection);
        }

        [Test]
        public void Get_course_completion_of_assessed_course_should_return_course_completion()
        {
            // Given
            const int candidateId = 252646;
            const int customisationId = 21452;

            // When
            var result = courseCompletionService.GetCourseCompletion(candidateId, customisationId);

            // Then
            result.Should().BeEquivalentTo(new CourseCompletion(
                customisationId,
                "TNA v0.1",
                "RiO Mersey Care & NWBH",
                false,
                DateTime.Parse("2017-07-26 07:56:15.273"),
                null,
                1,
                true,
                80,
                85,
                100,
                22,
                1,
                13,
                9,
                13
            ));
        }

        [Test]
        public void Get_course_completion_of_course_should_return_when_all_tutorials_statuses_are_0()
        {
            // CustomisationTutorials.Status = 0 entries where CustomisationID = 26696

            // Given
            const int candidateId = 210962;
            const int customisationId = 26696;

            // When
            var result = courseCompletionService.GetCourseCompletion(candidateId, customisationId);

            // Then
            result.Should().BeEquivalentTo(new CourseCompletion(
                customisationId,
                "Recruitment and Selection",
                "RandS",
                false,
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

        [Test]
        public void Get_course_completion_of_evaluated_course_should_return_course_completion()
        {
            // Given
            const int candidateId = 118938;
            const int customisationId = 9117;

            // When
            var result = courseCompletionService.GetCourseCompletion(candidateId, customisationId);

            // Then
            result.Should().BeEquivalentTo(new CourseCompletion(
                customisationId,
                "Entry Level - Win 7, Office 2010",
                "Basic Introductory Course",
                false,
                DateTime.Parse("2014-08-26 15:23:58.620"),
                DateTime.Parse("2014-08-26 15:26:03.260"),
                0,
                true,
                85,
                85,
                100,
                53,
                1,
                4,
                10,
                10
            ));
        }

        [Test]
        public void Get_course_completion_should_return_course_completion_for_course_without_tutorials()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 100;

            // When
            var result = courseCompletionService.GetCourseCompletion(candidateId, customisationId);

            // Then
            result.Should().BeEquivalentTo(new CourseCompletion(
                customisationId,
                "Entry Level - Win XP, Office 2003/07 OLD",
                "Standard",
                false,
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
            // Given
            const int candidateId = 100;
            const int customisationId = 100;

            // When
            var result = courseCompletionService.GetCourseCompletion(candidateId, customisationId);

            // Then
            result.Should().BeEquivalentTo(new CourseCompletion(
                customisationId,
                "Entry Level - Win XP, Office 2003/07 OLD",
                "Standard",
                false,
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
