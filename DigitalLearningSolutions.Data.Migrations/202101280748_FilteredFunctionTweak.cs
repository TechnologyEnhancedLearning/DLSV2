namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;

    [Migration(202101280748)]
    public class FilteredFunctionTweak : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.DLSV2_153_FilteredFunctionTweak);
        }

        public override void Down()
        {
            Execute.Sql(Properties.Resources.DLSV2_153_DropFilteredFunctionTweak);
        }
    }
}
