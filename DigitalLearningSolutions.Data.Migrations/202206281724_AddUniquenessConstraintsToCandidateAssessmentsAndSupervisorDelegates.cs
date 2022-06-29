namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;

    [Migration(202206281724)]
    public class AddUniquenessConstraintsToCandidateAssessmentsAndSupervisorDelegates : Migration
    {
        public override void Up()
        {
            Create.UniqueConstraint().OnTable("CandidateAssessments").Columns("DelegateUserId", "SelfAssessmentId");
            Create.UniqueConstraint().OnTable("SupervisorDelegates").Columns("SupervisorAdminId", "DelegateUserId");

            Create.Index().OnTable("").WithOptions().UniqueNullsNotDistinct().OnColumn("DelegateUserId").Ascending()
                .OnColumn("SelfAssessmentId").Ascending();
        }

        public override void Down()
        {
            Delete.UniqueConstraint().FromTable("CandidateAssessments").Columns("DelegateUserId", "SelfAssessmentId");
            Delete.UniqueConstraint().FromTable("SupervisorDelegates").Columns("SupervisorAdminId", "DelegateUserId");
        }
    }
}
