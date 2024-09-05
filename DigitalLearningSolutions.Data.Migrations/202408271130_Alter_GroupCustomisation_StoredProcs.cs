namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202408271130)]
    public class Alter_GroupCustomisation_StoredProcs : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_4436_Alter_GroupCustomisation_Add_V2_UpdateCompleteBy_Supervisor_Up);
            Execute.Sql(Properties.Resources.TD_4436_Alter_uspCreateProgressRecordWithCompleteWithinMonths_Quiet_V2_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_4436_Alter_GroupCustomisation_Add_V2_UpdateCompleteBy_Supervisor_Down);
            Execute.Sql(Properties.Resources.TD_4436_Alter_uspCreateProgressRecordWithCompleteWithinMonths_Quiet_V2_Down);
        }

    }
}
