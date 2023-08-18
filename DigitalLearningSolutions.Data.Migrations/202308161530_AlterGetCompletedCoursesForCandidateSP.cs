using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202308161530)]
    public class AlterGetCompletedCoursesForCandidateSP : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_1766_GetCompletedCoursesForCandidateTweak);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_1766_GetCompletedCoursesForCandidateTweak_down);
        }
    }
}
