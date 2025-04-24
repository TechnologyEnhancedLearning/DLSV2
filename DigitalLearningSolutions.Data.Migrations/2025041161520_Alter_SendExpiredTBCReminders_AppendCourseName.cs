

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(2025041161520)]
    public class Alter_SendExpiredTBCReminders_AppendCourseName : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_5514_Alter_SendExpiredTBCReminders_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_5514_Alter_SendExpiredTBCReminders_Down);
        }
    }
}
