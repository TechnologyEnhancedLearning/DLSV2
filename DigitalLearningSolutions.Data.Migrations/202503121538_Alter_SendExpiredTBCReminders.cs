

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202503121538)]
    public class Alter_SendExpiredTBCReminders : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_5412_Alter_SendExpiredTBCReminders_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_5412_Alter_SendExpiredTBCReminders_Down);
        }
    }
}
