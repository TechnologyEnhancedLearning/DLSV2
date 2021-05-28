﻿namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using NUnit.Framework;
    using FluentAssertions;

    public class ConfigServiceTests
    {
        private ConfigService configService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            configService = new ConfigService(connection);
        }

        [Test]
        public void Get_config_value_returns_the_expected_value()
        {
            // When
            var result = configService.GetConfigValue(ConfigService.MailFromAddress);

            // Then
            result.Should().Be("noreply@itskills.nhs.uk");
        }
    }
}
