namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;

    [Migration(202009220947)]
    public class AddFilteredSPs : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.FilteredSPs);
        }

        public override void Down()
        {
            Execute.Sql(Properties.Resources.DropFilteredSPs);
        }
    }
}
