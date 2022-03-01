namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using NUnit.Framework;
    using FluentAssertions;

    public class ConfigServiceTests
    {
        private ConfigDataDataService configDataDataService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            configDataDataService = new ConfigDataDataService(connection);
        }

        [Test]
        public void Get_config_value_returns_the_expected_value()
        {
            // When
            var result = configDataDataService.GetConfigValue(ConfigDataDataService.MailFromAddress);

            // Then
            result.Should().Be("noreply@itskills.nhs.uk");
        }
    }
}
