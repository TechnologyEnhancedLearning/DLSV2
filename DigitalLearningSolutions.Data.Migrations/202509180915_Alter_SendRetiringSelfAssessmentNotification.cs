

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202509180915)]
    public class Alter_SendRetiringSelfAssessmentNotification : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_5552_Alter_SendRetiringSelfAssessmentNotification_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_5552_Alter_SendRetiringSelfAssessmentNotification_Down);
        }
    }
}
