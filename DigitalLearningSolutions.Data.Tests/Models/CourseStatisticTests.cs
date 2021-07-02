namespace DigitalLearningSolutions.Data.Tests.Models
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using FluentAssertions;
    using NUnit.Framework;

    public class CourseStatisticTests
    {
        [Test]
        public void Pass_rate_should_be_calculated_from_attempts_and_rounded()
        {
            // When
            var courseStatistics = new CourseStatistics
            {
                AllAttempts = 1000,
                AttemptsPassed = 514
            };

            // Then
            courseStatistics.PassRate.Should().Be(51);
        }

        [Test]
        public void InProgressCount_should_be_calculated_from_total_delegates_and_total_completed()
        {
            // When
            var courseStatistics = new CourseStatistics
            {
                DelegateCount = 90,
                CompletedCount = 48
            };

            // Then
            courseStatistics.InProgressCount.Should().Be(42);
        }

        [Test]
        public void Course_name_should_be_application_name_if_customisation_name_is_null()
        {
            // When
            var courseStatistics = new CourseStatistics
            {
                ApplicationName = "Test application",
                CustomisationName = null,
            };

            // Then
            courseStatistics.CourseName.Should().BeEquivalentTo("Test application");
        }

        [Test]
        public void Course_name_should_include_customisation_name_if_it_is_not_null()
        {
            // When
            var courseStatistics = new CourseStatistics
            {
                ApplicationName = "Test application",
                CustomisationName = "customisation",
            };

            // Then
            courseStatistics.CourseName.Should().BeEquivalentTo("Test application - customisation");
        }

    }
}
