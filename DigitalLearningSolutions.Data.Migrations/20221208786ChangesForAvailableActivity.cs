namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;

    [Migration(20221208786)]
    public class ChangesForAvailableActivity : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_786_GetSelfRegisteredFlag_UP);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_786_GetSelfRegisteredFlag_DOWN);
        }
    }
}
