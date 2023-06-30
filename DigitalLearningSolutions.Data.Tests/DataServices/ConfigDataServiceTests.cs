namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;
    using System;

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

        [Test]
        public void Get_config_last_updated_returns_a_valid_date()
        {
            // When
            var result = configDataService.GetConfigLastUpdated(ConfigDataService.TrackingSystemBaseUrl);

            // Then
            result.Should().BeBefore(DateTime.Now);
        }
    }
}
