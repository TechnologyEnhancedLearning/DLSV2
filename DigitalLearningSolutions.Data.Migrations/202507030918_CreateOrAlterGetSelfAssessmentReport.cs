namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(20250703091)]
    public class CreateOrAlterGetSelfAssessmentReport : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_5759_CreateOrAlterSelfAssessmentReportSPandTVF_UP);
        }
        public override void Down()
        {
            Execute.Sql("DROP PROCEDURE IF EXISTS [dbo].[usp_GetSelfAssessmentReport]");
            Execute.Sql("DROP FUNCTION IF EXISTS [dbo].[GetOtherCentresForSelfAssessmentTVF]");
        }
    }
}
