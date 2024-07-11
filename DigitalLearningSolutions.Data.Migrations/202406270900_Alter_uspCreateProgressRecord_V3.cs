namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202406270900)]
    public class Alter_uspCreateProgressRecord_V3 : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_4223_Alter_uspCreateProgressRecord_V3_Up);
            Execute.Sql(Properties.Resources.TD_4223_Alter_uspCreateProgressRecordWithCompleteWithinMonths_Quiet_V2_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_4223_Alter_uspCreateProgressRecord_V3_Down);
            Execute.Sql(Properties.Resources.TD_4223_Alter_uspCreateProgressRecordWithCompleteWithinMonths_Quiet_V2_Down);
        }
    }
}
