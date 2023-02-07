using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202302061646)]
    public class AlterViewAdminUsersAddCentreName : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.td_264_alterviewadminusersaddcentrename_up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.td_264_alterviewadminusersaddcentrename_down);
        }
    }
}
