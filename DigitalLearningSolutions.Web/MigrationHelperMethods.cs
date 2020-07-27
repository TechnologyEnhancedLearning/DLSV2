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
                    .ScanIn(typeof(AddSelfAssessmentTables).Assembly).For.Migrations()
                ).AddLogging(lb => lb
                    .AddFluentMigratorConsole()
                );
        }
    }
}
