namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202506110906)]
    public class AddSqlMaintenanceSolution : Migration
    {

        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_5670_MaintenanceScripts_UP);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_5670_MaintenanceScripts_DOWN);
        }
    }
}
