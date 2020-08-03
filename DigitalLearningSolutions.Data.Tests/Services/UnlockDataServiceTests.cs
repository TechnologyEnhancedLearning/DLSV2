namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Web.Helpers;
    using NUnit.Framework;
    using FluentAssertions;
    using FluentMigrator.Runner;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.DependencyInjection;

    [Parallelizable(ParallelScope.Fixtures)]
    public class UnlockDataServiceTests
    {
        private UnlockDataService unlockDataService;

        [SetUp]
        public void Setup()
        {
            var connectionString = ServiceTestHelper.GetSqlConnectionString();
            var serviceCollection = new ServiceCollection().RegisterMigrationRunner(connectionString);
            serviceCollection.BuildServiceProvider().GetRequiredService<IMigrationRunner>().MigrateUp();

            var connection = new SqlConnection(connectionString);
            unlockDataService = new UnlockDataService(connection);
        }

        [Test]
        public void Get_unlock_data_returns_correct_results()
        {
            // When
            const int progressId = 173218;
            var result = unlockDataService.GetUnlockData(progressId);

            // Then
            var expectedUnlockData = new UnlockData
            {
                DelegateEmail = "hcta@egviomklw.",
                DelegateName = "xxxxx xxxxxxxxx",
                ContactForename = "xxxxx",
                ContactEmail = "e@1htrnkisv.wa",
                CourseName = "Office 2013 Essentials for the Workplace - Erin Test 01",
                CustomisationId = 15853
            };
            result.Should().BeEquivalentTo(expectedUnlockData);
        }
    }
}
