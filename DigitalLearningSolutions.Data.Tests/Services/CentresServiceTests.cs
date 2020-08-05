namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using FluentMigrator.Runner;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [Parallelizable(ParallelScope.Fixtures)]
    public class CentresServiceTests
    {
        private CentresService centresService;

        [SetUp]
        public void Setup()
        {
            var connectionString = ServiceTestHelper.GetSqlConnectionString();
            var serviceCollection = new ServiceCollection().RegisterMigrationRunner(connectionString);
            serviceCollection.BuildServiceProvider().GetRequiredService<IMigrationRunner>().MigrateUp();

            var connection = new SqlConnection(connectionString);
            centresService = new CentresService(connection);
        }

        [Test]
        public void Get_banner_text_should_return_the_correct_value()
        {
            // When
            var result = centresService.GetBannerText(2);

            // Then
            result.Should().Be("xxxxxxxxxxxxxxxxxxxx");
        }

        [Test]
        public void Get_banner_text_should_return_null_when_the_centre_does_not_exist()
        {
            // When
            var result = centresService.GetBannerText(1);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_banner_text_should_return_null_when_the_banner_text_is_null()
        {
            // When
            var result = centresService.GetBannerText(3);

            // Then
            result.Should().BeNull();
        }
    }
}
