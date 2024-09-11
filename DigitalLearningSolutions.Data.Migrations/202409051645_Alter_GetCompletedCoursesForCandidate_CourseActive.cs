

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202409051645)]
    public class Alter_GetCompletedCoursesForCandidate_CourseActive : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_4634_Alter_GetCompletedCoursesForCandidate_UP);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_4634_Alter_GetCompletedCoursesForCandidate_DOWN);
        }
    }
}
