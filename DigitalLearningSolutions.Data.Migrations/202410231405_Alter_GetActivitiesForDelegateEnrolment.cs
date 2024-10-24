

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202410231405)]
    public class Alter_GetActivitiesForDelegateEnrolment : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_4878_Alter_GetActivitiesForDelegateEnrolment_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_4878_Alter_GetActivitiesForDelegateEnrolment_Down);
        }
    }
}
