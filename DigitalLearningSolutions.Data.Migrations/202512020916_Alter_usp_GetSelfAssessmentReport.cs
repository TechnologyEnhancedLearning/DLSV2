

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202512020916)]
    public class Alter_usp_GetSelfAssessmentReport : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_6437_usp_GetSelfAssessmentReport_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_6437_usp_GetSelfAssessmentReport_Down);
        }
    }
}
