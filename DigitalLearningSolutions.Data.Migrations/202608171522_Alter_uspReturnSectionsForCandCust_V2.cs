using FluentMigrator;
using FluentMigrator.SqlServer;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202608171522)]
    public class Alter_uspReturnSectionsForCandCust_V2 : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_7599_Alter_uspReturnSectionsForCandCust_V2_Up);

            Create.Index("IX_Sessions_Candidate_Customisation_LoginTime")
            .OnTable("Sessions")
            .OnColumn("CandidateID").Ascending()
            .OnColumn("CustomisationID").Ascending()
            .OnColumn("LoginTime").Ascending()
            .WithOptions()
                .NonClustered()
                .Include("Duration");

            Create.Index("IX_aspProgress_ProgressID_TutorialID")
                .OnTable("aspProgress")
                .OnColumn("ProgressID").Ascending()
                .OnColumn("TutorialID").Ascending()
                .WithOptions()
                    .NonClustered()
                    .Include("TutStat")
                    .Include("TutTime")
                    .Include("DiagAttempts")
                    .Include("DiagLast");

            Create.Index("IX_AssessAttempts_Progress_Section_Status_Score")
                .OnTable("AssessAttempts")
                .OnColumn("ProgressID").Ascending()
                .OnColumn("SectionNumber").Ascending()
                .OnColumn("Status").Descending()
                .OnColumn("Score").Descending()
                .OnColumn("AssessAttemptID").Descending()
                .WithOptions()
                    .NonClustered();

        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_7599_Alter_uspReturnSectionsForCandCust_V2_Down);

            Delete.Index("IX_AssessAttempts_Progress_Section_Status_Score")
            .OnTable("AssessAttempts");

            Delete.Index("IX_aspProgress_ProgressID_TutorialID")
                .OnTable("aspProgress");

            Delete.Index("IX_Sessions_Candidate_Customisation_LoginTime")
                .OnTable("Sessions");
        }
    }
}
