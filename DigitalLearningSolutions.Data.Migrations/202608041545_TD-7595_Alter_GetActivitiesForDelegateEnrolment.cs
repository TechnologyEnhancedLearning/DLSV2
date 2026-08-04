namespace DigitalLearningSolutions.Data.Migrations
{
     using FluentMigrator;

    [Migration(202607081170)]
    public class _202608041545_TD_7595_Alter_GetActivitiesForDelegateEnrolment : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_7595_Alter_GetActivitiesForDelegateEnrolment_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_7595_Alter_GetActivitiesForDelegateEnrolment_Down);
        }
    }
}
