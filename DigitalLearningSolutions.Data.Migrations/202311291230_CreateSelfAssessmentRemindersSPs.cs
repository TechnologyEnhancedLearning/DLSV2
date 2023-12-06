namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202311291230)]
    public class CreateSelfAssessmentRemindersSPs : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_3190_SendOneMonthSelfAssessmentTBCRemindersSP);
            Execute.Sql(Properties.Resources.TD_3190_SendOneMonthSelfAssessmentOverdueRemindersSP);
        }
        public override void Down()
        {
            Execute.Sql("DROP PROCEDURE [dbo].[SendOneMonthSelfAssessmentTBCReminders]");
            Execute.Sql("DROP PROCEDURE [dbo].[SendSelfAssessmentOverdueReminders]");
        }
    }
}
