using FluentMigrator;
using FluentMigrator.SqlServer;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202611121700)]
    public class Alter_usp_GetSelfAssessmentReport_CurrentSignOffStatus : Migration
    {
        public override void Up()
        {
            Create.Index("IX_CASVerifications_CandidateAssessmentSupervisorID_Requested_ID")
                .OnTable("CandidateAssessmentSupervisorVerifications")
                .OnColumn("CandidateAssessmentSupervisorID").Ascending()
                .OnColumn("Requested").Descending()
                .OnColumn("ID").Descending()
                .WithOptions()
                .NonClustered()
                .Include("Verified")
                .Include("SignedOff");

            Execute.Sql(Properties.Resources.TD_8036_Alter_usp_GetSelfAssessmentReport_Up);
        }

        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_8036_Alter_usp_GetSelfAssessmentReport_Down);
            Delete.Index("IX_CASVerifications_CandidateAssessmentSupervisorID_Requested_ID")
                .OnTable("CandidateAssessmentSupervisorVerifications");
        }
    }
}
