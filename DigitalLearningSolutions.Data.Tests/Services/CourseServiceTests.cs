namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using NUnit.Framework;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;

    public class Tests
    {
        private CourseService courseService;

        [SetUp]
        public void Setup()
        {
            var connection = new SqlConnection("Data Source=localhost;Initial Catalog=mbdbx101_test;Integrated Security=True;");
            courseService = new CourseService(connection);
        }

        [Test]
        public void Get_current_courses_should_return_applications()
        {
            // When
            var result = courseService.GetCurrentCourses().ToList();

            // Then
            var expectedFirstCourse = new Course
            {
                Name = "Entry Level - Win XP, Office 2003/07 OLD",
                Id = 1
            };
            result.Should().HaveCount(57);
            result.First().Should().BeEquivalentTo(expectedFirstCourse);
        }

        [Test]
        public void Get_completed_courses_should_return_applications()
        {
            // When
            var result = courseService.GetCompletedCourses().ToList();

            // Then
            var expectedFirstCourse = new Course
            {
                Name = "Combined Office Course",
                Id = 39
            };
            result.Should().HaveCount(37);
            result.First().Should().BeEquivalentTo(expectedFirstCourse);
        }

        [Test]
        public void Get_available_courses_should_return_applications()
        {
            // When
            var result = courseService.GetAvailableCourses().ToList();

            // Then
            var expectedFirstCourse = new Course
            {
                Name = "Mobile DoS",
                Id = 49
            };
            result.Should().HaveCount(45);
            result.First().Should().BeEquivalentTo(expectedFirstCourse);
        }
    }
}
