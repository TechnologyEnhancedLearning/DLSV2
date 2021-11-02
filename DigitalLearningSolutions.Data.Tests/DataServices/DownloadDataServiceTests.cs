namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class DownloadDataServiceTests
    {
        private IDownloadDataService downloadDataService = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            downloadDataService = new DownloadDataService(connection);
        }

        [Test]
        public void GetAllDownloads_gets_expected_downloads()
        {
            // When
            var result = downloadDataService.GetAllDownloads().ToList();

            // Then
            result.Count.Should().Be(96);
        }
    }
}
