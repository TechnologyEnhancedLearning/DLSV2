using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202305220753)]
    public class AlterGetActivitiesForEnrolment : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.td_1610_update_getactivitiesfordelegateenrolment_proc_up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.td_1610_update_getactivitiesfordelegateenrolment_proc_down);
        }
    }
}

