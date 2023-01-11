using FluentMigrator;
namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202212151111)]
    public class FixSPsForAvailableActivitiesForPassporting : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_786_GetSelfRegisteredFlag_UP);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_786_GetSelfRegisteredFlag_DOWN);
        }
    }
}
