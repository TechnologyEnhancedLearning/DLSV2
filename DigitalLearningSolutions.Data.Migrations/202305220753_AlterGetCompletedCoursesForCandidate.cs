using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202404091210)]
    public class AlterGetCompletedCoursesForCandidate : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_4015_Update_GetCompletedCoursesForCandidate_proc_up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_4015_Update_GetCompletedCoursesForCandidate_proc_down);
        }
    }
}

