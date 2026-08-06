namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202608061170)]
    public class _202608061545_TD_7598_Alter_GetCompletedCoursesForCandidate : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_7598_Alter_GetCompletedCoursesForCandidate_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_7598_Alter_GetCompletedCoursesForCandidate_Down);
        }
    }
}
