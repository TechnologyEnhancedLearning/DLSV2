namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class ResourceDataServiceTests
    {
        private IResourceDataService resourceDataService = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            resourceDataService = new ResourceDataService(connection);
        }

        [Test]
        public void GetAllResources_gets_expected_downloads()
        {
            // When
            var result = resourceDataService.GetAllResources().ToList();

            // Then
            result.Count.Should().Be(96);
        }
    }
}
