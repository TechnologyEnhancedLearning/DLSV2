using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202602091540)]
    public class AlterViewAdminUsersAddAdminUserId : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_6866_AlterViewAdminUsersAddAdminUserId_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_6866_AlterViewAdminUsersAddAdminUserId_Down);
        }
    }
}
