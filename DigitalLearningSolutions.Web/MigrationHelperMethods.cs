namespace DigitalLearningSolutions.Web
{
    using DigitalLearningSolutions.Data.Migrations;
    using FluentMigrator.Runner;
    using Microsoft.Extensions.DependencyInjection;

    public static class MigrationHelperMethods
    {
        public static IServiceCollection RegisterMigrationRunner(this IServiceCollection serviceCollection, string connectionString)
        {
            return serviceCollection.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer2016()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(ExampleMigration).Assembly).For.Migrations() // TODO: remove example migration once we have a real migration
                ).AddLogging(lb => lb
                    .AddFluentMigratorConsole()
                );
        }
    }
}
