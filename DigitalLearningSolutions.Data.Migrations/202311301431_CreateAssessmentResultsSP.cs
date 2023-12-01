namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202311301431)]
    public class CreateAssessmentResultsSP : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_3187_CreateGetCandidateAssessmentResultsById_SP);
            Execute.Sql(Properties.Resources.TD_3187_CreateGetAssessmentResultsByDelegate_SP);
        }

        public override void Down()
        {
            Execute.Sql(@$"DROP PROCEDURE [dbo].[GetCandidateAssessmentResultsById]");
            Execute.Sql(@$"DROP PROCEDURE [dbo].[GetAssessmentResultsByDelegate]");
        }
    }
}
