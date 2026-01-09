namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;

    [Migration(202601091000)]
    public class SupervisorDashboardIndexes : Migration
    {
        public override void Up()
        {
            Create.Index("IX_SupervisorDelegates_Admin_Removed")
                .OnTable("SupervisorDelegates")
                .OnColumn("SupervisorAdminID").Ascending()
                .OnColumn("Removed").Ascending()
                .WithOptions()
                    .NonClustered()
                .Include("DelegateUserID");

            Create.Index("IX_CandidateAssessmentSupervisors_SupervisorDelegateId_Removed")
                .OnTable("CandidateAssessmentSupervisors")
                .OnColumn("SupervisorDelegateId").Ascending()
                .OnColumn("Removed").Ascending()
                .WithOptions()
                    .NonClustered()
                .Include("CandidateAssessmentID");

            Create.Index("IX_CandidateAssessments_RemovedDate")
                .OnTable("CandidateAssessments")
                .OnColumn("RemovedDate").Ascending()
                .WithOptions()
                    .NonClustered()
                .Include("SelfAssessmentID")
                .Include("DelegateUserID");

            Create.Index("IX_CandidateAssessmentSupervisorVerifications_CandidateAssessmentSupervisorID_Verified")
                .OnTable("CandidateAssessmentSupervisorVerifications")
                .OnColumn("CandidateAssessmentSupervisorID").Ascending()
                .OnColumn("Verified").Ascending()
                .WithOptions()
                    .NonClustered();

            Create.Index("IX_SRSV_CandidateAssessmentSupervisorID_SelfAssessmentResultId_Superceded_Verified")
                .OnTable("SelfAssessmentResultSupervisorVerifications")
                .OnColumn("CandidateAssessmentSupervisorID").Ascending()
                .OnColumn("SelfAssessmentResultId").Ascending()
                .OnColumn("Superceded").Ascending()
                .OnColumn("Verified").Ascending()
                .WithOptions()
                    .NonClustered()
                .Include("Requested");

            Create.Index("IX_SelfAssessmentResults_SelfAssessmentID_Result")
                .OnTable("SelfAssessmentResults")
                .OnColumn("SelfAssessmentID").Ascending()
                .OnColumn("Result").Ascending()
                .WithOptions()
                    .NonClustered()
                .Include("CompetencyID")
                .Include("DateTime");
        }
        public override void Down()
        {
            Delete.Index("IX_SupervisorDelegates_Admin_Removed").OnTable("SupervisorDelegates");
            Delete.Index("IX_CandidateAssessmentSupervisors_SupervisorDelegateId_Removed").OnTable("CandidateAssessmentSupervisors");
            Delete.Index("IX_CandidateAssessments_RemovedDate").OnTable("CandidateAssessments");
            Delete.Index("IX_CASVerifications_CandidateAssessmentSupervisorID_Verified").OnTable("CandidateAssessmentSupervisorVerifications");
            Delete.Index("IX_SRSV_CandidateAssessmentSupervisorID_SelfAssessmentResultId_Superceded_Verified").OnTable("SelfAssessmentResultSupervisorVerifications");
            Delete.Index("IX_SelfAssessmentResults_SelfAssessmentID_Result").OnTable("SelfAssessmentResults");
        }
    }
}
