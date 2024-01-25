namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202306261100)]
    public class SwitchOffAllSystemVersioning : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_2036_SwitchSystemVersioningOffAllTables_UP);
        }

        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_2036_SwitchSystemVersioningOffAllTables_DOWN);
        }
    }
}
