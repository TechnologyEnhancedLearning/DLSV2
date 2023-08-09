using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202306270900)]
    public class AlterGetCurrentCoursesForCandidateSP : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_1766_GetCurrentCoursesForCandidateTweak);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_1766_GetCurrentCoursesForCandidateTweak_down);
        }
    }
}
