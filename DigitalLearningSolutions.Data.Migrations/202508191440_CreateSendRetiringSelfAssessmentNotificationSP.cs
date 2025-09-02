namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202508191440)]
    public class CreateSendRetiringSelfAssessmentNotificationSP : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_5552_SendRetiringNotification);
        }
        public override void Down()
        {
            Execute.Sql("DROP PROCEDURE [dbo].[SendRetiringSelfAssessmentNotification]");
        }
    }
}
