using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202302061645)]
    public class AlterViewAdminUsersAddCentreName : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.td_264_AlterViewAdminUsersAddCentreName_UP);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.td_264_AlterViewAdminUsersAddCentreName_DOWN);
        }
    }
}
