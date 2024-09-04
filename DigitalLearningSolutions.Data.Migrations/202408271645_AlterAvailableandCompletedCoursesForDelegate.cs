

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202409031645)]
    public class AlterAvailableandCompletedCoursesForDelegate : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_4243_Alter_GetActivitiesForDelegateEnrolment_proc_up);
            Execute.Sql(Properties.Resources.TD_4243_Alter_GetCompletedCoursesForCandidate_proc_up);
            Execute.Sql(Properties.Resources.TD_4243_Alter_GetCurrentCoursesForCandidate_V2_proc_up);
        }
        public override void Down()
        {
           Execute.Sql(Properties.Resources.TD_4243_Alter_GetActivitiesForDelegateEnrolment_proc_down);
          Execute.Sql(Properties.Resources.TD_4243_Alter_GetCompletedCoursesForCandidate_proc_down);
          Execute.Sql(Properties.Resources.TD_4243_Alter_GetCurrentCoursesForCandidate_V2_proc_down);
        }
    }
}
