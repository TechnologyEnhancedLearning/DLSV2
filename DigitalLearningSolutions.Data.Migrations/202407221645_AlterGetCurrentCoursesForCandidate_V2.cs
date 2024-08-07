namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202407221645)]
    public class AlterGetCurrentCoursesForCandidate_V3 : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_4243_Alter_GetCurrentCoursesForCandidate_V2_proc_up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_4243_Alter_GetCurrentCoursesForCandidate_V2_proc_down);
        }
    }
}
