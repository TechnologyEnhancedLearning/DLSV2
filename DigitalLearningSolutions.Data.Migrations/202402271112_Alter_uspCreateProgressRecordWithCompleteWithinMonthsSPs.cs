namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202402271112)]
    public class Alter_uspCreateProgressRecordWithCompleteWithinMonthsSPs : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_3623_Alter_uspCreateProgressRecordWithCompleteWithinMonthsSPs_Up);

            Execute.Sql(
                @$"UPDATE Progress SET EnrollmentMethodID = 4
                    WHERE EnrollmentMethodID = 0 OR EnrollmentMethodID > 4;"
            );
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_3623_Alter_uspCreateProgressRecordWithCompleteWithinMonthsSPs_Down);
        }
    }
}
