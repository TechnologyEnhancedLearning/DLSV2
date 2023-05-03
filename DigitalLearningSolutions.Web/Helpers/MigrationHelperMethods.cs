namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Migrations;
    using FluentMigrator.Runner;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class MigrationHelperMethods
    {
        public static IServiceCollection RegisterMigrationRunner
            (this IServiceCollection serviceCollection, string connectionString)
        {
            return serviceCollection.AddFluentMigratorCore()
                .ConfigureRunner
                (
                    rb => rb
                        .AddSqlServer2016()
                        .WithGlobalCommandTimeout(TimeSpan.FromSeconds(360))
                        .WithGlobalConnectionString(connectionString)
                        .ScanIn(typeof(AddSelfAssessmentTables).Assembly)
                        .For.Migrations()
                ).AddLogging
                (
                    lb => lb
                        .AddFluentMigratorConsole()
                );
        }
    }
}
