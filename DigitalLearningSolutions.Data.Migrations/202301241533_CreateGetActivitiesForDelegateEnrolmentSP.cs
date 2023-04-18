using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202301241533)]
    public class CreateGetActivitiesForDelegateEnrolmentSP : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.td_1043_getactivitiesforenrolment);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.td_1043_getactivitiesforenrolment_down);
        }
    }
}
