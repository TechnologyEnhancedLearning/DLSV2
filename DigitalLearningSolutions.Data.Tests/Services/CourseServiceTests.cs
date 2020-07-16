namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Linq;
    using Castle.Core.Internal;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web;
    using NUnit.Framework;
    using FluentAssertions;
    using FluentMigrator.Runner;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.DependencyInjection;

    public class Tests
    {
        private CourseService courseService;

        [SetUp]
        public void Setup()
        {
            const string defaultConnectionString = "Data Source=localhost;Initial Catalog=mbdbx101_test;Integrated Security=True;";
            var jenkinsConnectionString = GetJenkinsSqlConnectionString();
            var connectionString = jenkinsConnectionString.IsNullOrEmpty() ? defaultConnectionString : jenkinsConnectionString;

            var serviceCollection = new ServiceCollection().RegisterMigrationRunner(connectionString);
            serviceCollection.BuildServiceProvider().GetRequiredService<IMigrationRunner>().MigrateUp();

            var connection = new SqlConnection(connectionString);
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

        private static string GetJenkinsSqlConnectionString()
        {
            var jenkinsSqlServerPassword = Environment.GetEnvironmentVariable("SqlTestCredentials_PSW");
            var jenkinsSqlServerUsername = Environment.GetEnvironmentVariable("SqlTestCredentials_USR");
            return jenkinsSqlServerUsername.IsNullOrEmpty() || jenkinsSqlServerPassword.IsNullOrEmpty()
                ? ""
                : $"Server=HEE-DLS-SQL\\HEETEST; Database=mbdbx101; User Id={jenkinsSqlServerUsername}; Password={jenkinsSqlServerPassword};";
        }
    }
}
