namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;

    [Migration(202010120932)]
    public class AddCentreSelfAsessments : Migration
    {
        public override void Up()
        {
            //add centre self assessments table:
            Create.Table("CentreSelfAssessments")
               .WithColumn("CentreID").AsInt32().NotNullable().PrimaryKey().ForeignKey("Centres", "CentreID")
               .WithColumn("SelfAssessmentID").AsInt32().NotNullable().PrimaryKey().ForeignKey("SelfAssessments", "ID")
               .WithColumn("AllowEnrolment").AsBoolean().NotNullable().WithDefaultValue(false);
            //add centre learning portal URL field:
            Alter.Table("Centres")
               .AddColumn("LearningPortalUrl").AsString(100).Nullable();
            //add enrolment method etc to CandidateAssessments
            Alter.Table("CandidateAssessments")
                .AddColumn("LaunchCount").AsInt32().NotNullable().WithDefaultValue(0)
                .AddColumn("CompletedDate").AsDateTime().Nullable()
                .AddColumn("RemovedDate").AsDateTime().Nullable()
                .AddColumn("RemovalMethodID").AsInt32().NotNullable().WithDefaultValue(1)
                .AddColumn("EnrolmentMethodId").AsInt32().NotNullable().WithDefaultValue(1)
                .AddColumn("EnrolledByAdminId").AsInt32().Nullable()
                .AddColumn("SupervisorAdminId").AsInt32().Nullable()
                .AddColumn("OneMonthReminderSent").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("ExpiredReminderSent").AsBoolean().NotNullable().WithDefaultValue(false);
        }
        public override void Down()
        {
            Delete.Table("CentreSelfAssessments");
            Delete.Column("LearningPortalUrl").FromTable("Centres");
            Delete.Column("LaunchCount").FromTable("CandidateAssessments");
            Delete.Column("CompletedDate").FromTable("CandidateAssessments");
            Delete.Column("RemovedDate").FromTable("CandidateAssessments");
            Delete.Column("RemovalMethodID").FromTable("CandidateAssessments");
            Delete.Column("EnrolmentMethodId").FromTable("CandidateAssessments");
            Delete.Column("EnrolledByAdminId").FromTable("CandidateAssessments");
            Delete.Column("SupervisorAdminId").FromTable("CandidateAssessments");
            Delete.Column("OneMonthReminderSent").FromTable("CandidateAssessments");
            Delete.Column("ExpiredReminderSent").FromTable("CandidateAssessments");
        }
    }
}
