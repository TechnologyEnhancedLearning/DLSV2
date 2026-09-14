namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202608121110)]
    public class Alter_usp_GetSelfAssessmentReport_Optimisation : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_7640_Alter_usp_GetSelfAssessmentReport_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_7640_Alter_usp_GetSelfAssessmentReport_Down);
        }
    }
}
