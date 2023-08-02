namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202307052094)]
    public class AlterGetActivitiesForDelegateEnrolmentSPTweakDelegateStatus : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_2094_GetActivitiesForDelegateEnrolmentDelegateStatusPropertyTweak);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_2094_GetActivitiesForDelegateEnrolmentDelegateStatusPropertyTweak_down);
        }
    }
}
