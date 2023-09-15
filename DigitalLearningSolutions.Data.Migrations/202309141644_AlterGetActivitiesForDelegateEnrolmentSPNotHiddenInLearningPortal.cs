namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202309141644)]
    public class AlterGetActivitiesForDelegateEnrolmentSPNotHiddenInLearningPortal : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_2508_GetActivitiesForDelegateEnrolmentNotHiddenInLearningPortal);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_2508_GetActivitiesForDelegateEnrolmentNotHiddenInLearningPortal_down);
        }
    }
}
