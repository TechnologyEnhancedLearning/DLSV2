namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;

    [Migration(202210030601)]
    public class AddAvailableCustomisationsForCentreFiltered_V6 : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.DLSV2_581_GetActiveAvailableCustomisationsForCentreFiltered_V6);
        }

        public override void Down()
        {
            Execute.Sql(Properties.Resources.DropActiveAvailableV6);
        }
    }
}
