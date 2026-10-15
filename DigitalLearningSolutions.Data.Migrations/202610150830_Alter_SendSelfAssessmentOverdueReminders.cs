using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202610150830)]
    public class Alter_SendSelfAssessmentOverdueReminders : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_7893_Alter_SendSelfAssessmentOverdueReminders_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_7893_Alter_SendSelfAssessmentOverdueReminders_Down);
        }
    }
}
