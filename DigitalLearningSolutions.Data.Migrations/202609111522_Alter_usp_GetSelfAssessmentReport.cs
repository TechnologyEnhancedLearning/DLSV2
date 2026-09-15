using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202609111522)]
    public class Update_Alter_usp_GetSelfAssessmentReport : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_7776_Alter_usp_GetSelfAssessmentReport_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_7776_Alter_usp_GetSelfAssessmentReport_Down);
        }
    }
}
