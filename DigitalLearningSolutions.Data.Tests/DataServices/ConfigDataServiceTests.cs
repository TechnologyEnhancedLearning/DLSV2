namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class ConfigDataServiceTests
    {
        private ConfigDataService configDataService = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            configDataService = new ConfigDataService(connection);
        }

        [Test]
        public void Get_config_value_returns_the_expected_value()
        {
            // When
            var result = configDataService.GetConfigValue(ConfigDataService.TrackingSystemBaseUrl);

            // Then
            result.Should().Be("https://www.dls.nhs.uk/tracking/");
        }
    }
}
