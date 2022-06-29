namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202206281724)]
    public class AddUniquenessConstraintsToCandidateAssessmentsAndSupervisorDelegates : Migration
    {
        public override void Up()
        {
            Create.UniqueConstraint().OnTable("CandidateAssessments").Columns("DelegateUserId", "SelfAssessmentId");
            Create.UniqueConstraint().OnTable("SupervisorDelegates").Columns("SupervisorAdminId", "DelegateUserId");
        }

        public override void Down()
        {
            Delete.UniqueConstraint().FromTable("CandidateAssessments").Columns("DelegateUserId", "SelfAssessmentId");
            Delete.UniqueConstraint().FromTable("SupervisorDelegates").Columns("SupervisorAdminId", "DelegateUserId");
        }
    }
}
