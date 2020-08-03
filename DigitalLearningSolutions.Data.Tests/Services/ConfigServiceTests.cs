namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using NUnit.Framework;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using FluentMigrator.Runner;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.DependencyInjection;

    [Parallelizable(ParallelScope.Fixtures)]
    public class ConfigServiceTests
    {
        private ConfigService configService;

        [SetUp]
        public void Setup()
        {
            var connectionString = ServiceTestHelper.GetSqlConnectionString();
            var serviceCollection = new ServiceCollection().RegisterMigrationRunner(connectionString);
            serviceCollection.BuildServiceProvider().GetRequiredService<IMigrationRunner>().MigrateUp();

            var connection = new SqlConnection(connectionString);
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
