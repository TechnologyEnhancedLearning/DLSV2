namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;

    [Migration(202101271612)]
    public class FixFilteredSPs : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.DLSV2_153_FilteredSPFixes);
        }

        public override void Down()
        {
            Execute.Sql(Properties.Resources.DLSV2_153_DropFilteredSPFixes);
        }
    }
}
