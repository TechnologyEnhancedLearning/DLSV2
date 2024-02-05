namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202311270911)]
    public class SwitchOffPeriodFields : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_2036_SwitchOffPeriodFields_UP);
        }

        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_2036_SwitchOffPeriodFields_DOWN);
        }
    }
}
