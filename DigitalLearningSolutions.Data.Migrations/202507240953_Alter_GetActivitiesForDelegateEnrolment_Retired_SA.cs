using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202507240953)]
    public class Alter_GetActivitiesForDelegateEnrolment_Retired_SA : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_5535_Alter_GetActivitiesForDelegateEnrolment_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_5535_Alter_GetActivitiesForDelegateEnrolment_Down);
        }
    }
}

