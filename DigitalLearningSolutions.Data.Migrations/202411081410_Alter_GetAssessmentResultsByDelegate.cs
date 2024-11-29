

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202411081410)]
    public class Alter_GetAssessmentResultsByDelegate : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_4950_Alter_GetAssessmentResultsByDelegate_UP);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_4950_Alter_GetAssessmentResultsByDelegate_DOWN);
        }
    }
}
