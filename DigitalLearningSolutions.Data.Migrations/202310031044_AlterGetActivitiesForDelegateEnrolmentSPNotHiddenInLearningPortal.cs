namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202310031044)]
    public class AlterGetActivitiesForDelegateEnrolmentSPNotHiddenInLearningPortal : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_2508_GetActivitiesForDelegateEnrolmentHiddenInLearningPortalTweak);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_2508_GetActivitiesForDelegateEnrolmentHiddenInLearningPortalTweak_down);
        }
    }
}
