namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class RegionDataServiceTests
    {
        private IRegionDataService regionDataService = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            regionDataService = new RegionDataService(connection);
        }

        [Test]
        public void Get_job_groups_should_contain_a_job_group()
        {
            // When
            var result = regionDataService.GetRegionsAlphabetical().ToList();

            // Then
            result.Contains((2, "East Of England")).Should().BeTrue();
        }
    }
}
