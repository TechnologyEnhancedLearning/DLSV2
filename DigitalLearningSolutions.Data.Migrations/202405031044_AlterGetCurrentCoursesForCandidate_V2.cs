﻿namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202405231044)]
    public class AlterGetCurrentCoursesForCandidate_V2 : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_3671_Alter_GetCurrentCoursesForCandidate_V2_proc_up );
            Execute.Sql(Properties.Resources.TD_3671_Alter_CheckDelegateStatusForCustomisation_func_up );
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_3671_Alter_GetCurrentCoursesForCandidate_V2_proc_down);
            Execute.Sql(Properties.Resources.TD_3671_Alter_CheckDelegateStatusForCustomisation_func_down);
        }
    }
}
