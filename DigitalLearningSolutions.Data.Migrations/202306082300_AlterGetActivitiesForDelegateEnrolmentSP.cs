using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202306082300)]
    public class AlterGetActivitiesForDelegateEnrolmentSP : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_1766_GetActivitiesForDelegateEnrolmentTweak);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_1766_GetActivitiesForDelegateEnrolmentTweak_down);
        }
    }
}
