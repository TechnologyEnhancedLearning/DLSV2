namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202312061623)]
    public class FixSelfAssessmentRemindersSPs : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_3190_FixSelfAssessmentReminderQueriesSP_UP);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_3190_SendOneMonthSelfAssessmentTBCRemindersSP);
            Execute.Sql(Properties.Resources.TD_3190_SendOneMonthSelfAssessmentOverdueRemindersSP);
        }
    }
}
