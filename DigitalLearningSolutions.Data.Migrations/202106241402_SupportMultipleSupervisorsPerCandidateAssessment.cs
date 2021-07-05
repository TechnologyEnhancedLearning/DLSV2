namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202106241402)]
    public class SupportMultipleSupervisorsPerCandidateAssessment : Migration
    {
        public override void Up()
        {
            Delete.PrimaryKey("PK_CandidateAssessments").FromTable("CandidateAssessments");
            Alter.Table("CandidateAssessments").AddColumn("ID").AsInt32().NotNullable().Identity();
            Create.PrimaryKey("PK_CandidateAssessments").OnTable("CandidateAssessments").Column("ID");
            Create.Table("CandidateAssessmentSupervisors")
                .WithColumn("ID").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("CandidateAssessmentID").AsInt32().NotNullable().ForeignKey("CandidateAssessments", "ID")
                .WithColumn("SupervisorDelegateId").AsInt32().NotNullable().ForeignKey("SupervisorDelegates", "ID")
                .WithColumn("SupervisorRoleTitle").AsString(100).Nullable();
            Create.Table("CandidateAssessmentSupervisorVerifications")
                .WithColumn("ID").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("CandidateAssessmentSupervisorID").AsInt32().NotNullable().ForeignKey("CandidateAssessmentSupervisors", "ID")
                .WithColumn("Requested").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
                .WithColumn("EmailSent").AsDateTime().Nullable()
                .WithColumn("Verified").AsDateTime().Nullable()
                .WithColumn("Comments").AsString(int.MaxValue).Nullable()
                .WithColumn("SignedOff").AsBoolean().NotNullable().WithDefaultValue(false);
            Create.Table("SelfAssessmentResultSupervisorVerifications")
                .WithColumn("ID").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("CandidateAssessmentSupervisorID").AsInt32().NotNullable().ForeignKey("CandidateAssessmentSupervisors", "ID")
                .WithColumn("Requested").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
                .WithColumn("EmailSent").AsDateTime().Nullable()
                .WithColumn("Verified").AsDateTime().Nullable()
                .WithColumn("Comments").AsString(int.MaxValue).Nullable()
                .WithColumn("SignedOff").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("Superceded").AsBoolean().NotNullable().WithDefaultValue(false);
            Delete.ForeignKey("FK_SelfAssessmentResults_VerifierAdminID_AdminUsers_AdminID").OnTable("SelfAssessmentResults");
            Delete.Column("VerifiedDate").FromTable("SelfAssessmentResults");
            Delete.Column("VerifierComments").FromTable("SelfAssessmentResults");
            Delete.Column("VerifierAdminID").FromTable("SelfAssessmentResults");
            Delete.Column("VerificationOutcomeId").FromTable("SelfAssessmentResults");
            Delete.Column("VerificationRequested").FromTable("SelfAssessmentResults");
            Delete.ForeignKey("FK_CandidateAssessments_SelfAssessmentID_SelfAssessments_ID").OnTable("CandidateAssessments");
            Delete.ForeignKey("FK_CandidateAssessments_SignedByAdminId_AdminUsers_AdminID").OnTable("CandidateAssessments");
            Delete.Column("SupervisorAdminId").FromTable("CandidateAssessments");
            Delete.Column("OneMonthReminderSent").FromTable("CandidateAssessments");
            Delete.Column("ExpiredReminderSent").FromTable("CandidateAssessments");
            Delete.Column("OutcomeId").FromTable("CandidateAssessments");
            Delete.Column("SignedByAdminId").FromTable("CandidateAssessments");
            Delete.Column("Signed").FromTable("CandidateAssessments");
            Delete.Column("OutcomeAgreed").FromTable("CandidateAssessments");
        }
        public override void Down()
        {
            Delete.Table("SelfAssessmentResultSupervisorVerifications");
            Delete.Table("CandidateAssessmentSupervisorVerifications");
            Delete.Table("CandidateAssessmentSupervisors");
            Delete.PrimaryKey("PK_CandidateAssessments").FromTable("CandidateAssessments");
            Create.PrimaryKey("PK_CandidateAssessments").OnTable("CandidateAssessments").Columns("CandidateID", "SelfAssessmentID");
            Alter.Table("SelfAssessmentResults")
               .AddColumn("VerificationRequested").AsDateTime().Nullable()
               .AddColumn("VerificationOutcomeId").AsInt32().Nullable()
               .AddColumn("VerifierAdminID").AsInt32().Nullable().ForeignKey("AdminUsers", "AdminID")
               .AddColumn("VerifierComments").AsString(int.MaxValue).Nullable()
               .AddColumn("VerifiedDate").AsDateTime().Nullable();
            Alter.Table("CandidateAssessments")
                .AddColumn("SupervisorAdminId").AsInt32().Nullable()
                .AddColumn("OneMonthReminderSent").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("ExpiredReminderSent").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("OutcomeId").AsInt32().Nullable()
                .AddColumn("SignedByAdminId").AsInt32().Nullable().ForeignKey("AdminUsers", "AdminID")
                .AddColumn("Signed").AsDateTime().Nullable()
                .AddColumn("OutcomeAgreed").AsBoolean().NotNullable().WithDefaultValue(false);
        }
    }
}
