namespace DigitalLearningSolutions.Web.Helpers
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
                    .ScanIn(
                        typeof(AddSelfAssessmentTables).Assembly,
                        typeof(AddSelfAssessmentResultTimestamp).Assembly,
                        typeof(AddSelfAssessmentDates).Assembly,
                        typeof(AddSelfAssessmentUseApiFlag).Assembly,
                        typeof(AddFilteredMappingTables).Assembly,
                        typeof(AddFilteredSPs).Assembly,
                        typeof(AddCandidateAssessmentNewFields).Assembly,
                        typeof(AddCentreSelfAsessments).Assembly,
                        typeof(AddSelfAssessmentAdditionalFields).Assembly,
                        typeof(AddNewGetAvailableSproc).Assembly,
                        typeof(ApplyLPDefaultSPs).Assembly,
                        typeof(ReflectiveAccounts).Assembly,
                        typeof(CompetencyFrameworkDBChanges).Assembly,
                        typeof(AddInsertCustomisationV3sp).Assembly,
                        typeof(CandidateAssessmentSubmittedDate).Assembly,
                        typeof(ChangesForDigitalCapability).Assembly
                    ).For.Migrations()
                ).AddLogging(lb => lb
                    .AddFluentMigratorConsole()
                );
        }
    }
}
