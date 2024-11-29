namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202408141400)]
    public class Alter_uspCreateProgressRecord_V3_UtcDate : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_4436_Alter_uspCreateProgressRecord_V3_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_4436_Alter_uspCreateProgressRecord_V3_Down);
        }
    }
}
