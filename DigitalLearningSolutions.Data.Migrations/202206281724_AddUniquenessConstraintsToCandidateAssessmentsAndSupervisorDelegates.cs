namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;

    [Migration(202206281724)]
    public class AddUniquenessConstraintsToCandidateAssessmentsAndSupervisorDelegates : Migration
    {
        private const string CandidateAssessmentsIndexName = "IX_CandidateAssessments_DelegateUserId_SelfAssessmentId";
        private const string SupervisorDelegatesIndexName = "IX_SupervisorDelegates_DelegateUserId_SupervisorAdminId";

        public override void Up()
        {
            Create.Index(CandidateAssessmentsIndexName)
                .OnTable("CandidateAssessments").WithOptions().UniqueNullsNotDistinct()
                .OnColumn("SelfAssessmentID").Ascending()
                .OnColumn("DelegateUserID").Ascending();
            Create.Index(SupervisorDelegatesIndexName)
                .OnTable("SupervisorDelegates").WithOptions().UniqueNullsNotDistinct()
                .OnColumn("SupervisorAdminID").Ascending()
                .OnColumn("DelegateUserID").Ascending();
        }

        public override void Down()
        {
            Delete.Index(CandidateAssessmentsIndexName).OnTable("CandidateAssessments");
            Delete.Index(SupervisorDelegatesIndexName).OnTable("SupervisorDelegates");
        }
    }
}
