

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202607081170)]
    public class Alter_usp_GetSelfAssessmentReport_Count_Issue : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_7397_Alter_usp_GetSelfAssessmentReport_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_7397_Alter_usp_GetSelfAssessmentReport_Down);
        }
    }
}
