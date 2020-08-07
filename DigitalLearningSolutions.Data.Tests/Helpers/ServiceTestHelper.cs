namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using DigitalLearningSolutions.Web.Helpers;
    using FluentMigrator.Runner;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceTestHelper
    {
        public static SqlConnection GetDatabaseConnection()
        {
            var config = ConfigHelper.GetAppConfig();
            var connectionString = config.GetConnectionString(ConfigHelper.DefaultConnectionStringName);
            var serviceCollection = new ServiceCollection().RegisterMigrationRunner(connectionString);
            serviceCollection.BuildServiceProvider().GetRequiredService<IMigrationRunner>().MigrateUp();

            return new SqlConnection(connectionString);
        }

    }
}
