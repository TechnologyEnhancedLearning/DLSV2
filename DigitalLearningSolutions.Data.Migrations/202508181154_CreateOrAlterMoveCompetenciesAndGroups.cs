namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202508181154)]
    public class CreateOrAlterMoveCompetenciesAndGroups : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_483_uspMoveCompetencyInSelfAssessmentCreateOrAlter_UP);
            Execute.Sql(Properties.Resources.TD_483_uspMoveCompetencyGroupInSelfAssessmentCreateOrAlter_UP);
        }
        public override void Down()
        {
            Execute.Sql("DROP PROCEDURE IF EXISTS [dbo].[usp_MoveCompetencyGroupInSelfAssessment]");
            Execute.Sql("DROP PROCEDURE IF EXISTS [dbo].[usp_MoveCompetencyInSelfAssessment]");
        }
    }
}
