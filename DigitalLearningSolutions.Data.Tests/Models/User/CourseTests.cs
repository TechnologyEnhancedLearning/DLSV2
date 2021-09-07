namespace DigitalLearningSolutions.Data.Tests.Models.User
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using FluentAssertions;
    using NUnit.Framework;

    public class CourseTests
    {
        [Test]
        public void Course_name_should_be_application_name_if_customisation_name_is_null()
        {
            // When
            var courseStatistics = new Course
            {
                ApplicationName = "Test application",
                CustomisationName = string.Empty
            };

            // Then
            courseStatistics.CourseName.Should().BeEquivalentTo("Test application");
        }

        [Test]
        public void Course_name_should_include_customisation_name_if_it_is_not_null()
        {
            // When
            var courseStatistics = new Course
            {
                ApplicationName = "Test application",
                CustomisationName = "customisation"
            };

            // Then
            courseStatistics.CourseName.Should().BeEquivalentTo("Test application - customisation");
        }
    }
}
