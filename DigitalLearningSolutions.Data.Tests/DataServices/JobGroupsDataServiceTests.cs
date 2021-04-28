namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class JobGroupsServiceTests
    {
        private JobGroupsDataService jobGroupsDataService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            jobGroupsDataService = new JobGroupsDataService(connection);
        }

        [Test]
        public void Get_job_group_name_should_return_the_correct_value()
        {
            // When
            var result = jobGroupsDataService.GetJobGroupName(2);

            // Then
            result.Should().Be("Doctor / surgeon / consultant / dentist");
        }

        [Test]
        public void Get_job_group_name_should_return_null_when_the_job_group_does_not_exist()
        {
            // When
            var result = jobGroupsDataService.GetJobGroupName(11);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_job_groups_should_contain_a_job_group()
        {
            // When
            var result = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();

            // Then
            result.Contains((2, "Doctor / surgeon / consultant / dentist")).Should().BeTrue();
        }
    }
}
