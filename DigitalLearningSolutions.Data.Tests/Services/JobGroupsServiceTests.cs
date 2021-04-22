namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class JobGroupsServiceTests
    {
        private JobGroupsService jobGroupsService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<JobGroupsService>>();
            jobGroupsService = new JobGroupsService(connection, logger);
        }

        [Test]
        public void Get_job_group_name_should_return_the_correct_value()
        {
            // When
            var result = jobGroupsService.GetJobGroupName(2);

            // Then
            result.Should().Be("Doctor / surgeon / consultant / dentist");
        }

        [Test]
        public void Get_job_group_name_should_return_null_when_the_job_group_does_not_exist()
        {
            // When
            var result = jobGroupsService.GetJobGroupName(11);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_job_groups_should_contain_a_job_group()
        {
            // When
            var result = jobGroupsService.GetJobGroups().ToList();

            // Then
            result.Contains((2, "Doctor / surgeon / consultant / dentist")).Should().BeTrue();
        }
    }
}
