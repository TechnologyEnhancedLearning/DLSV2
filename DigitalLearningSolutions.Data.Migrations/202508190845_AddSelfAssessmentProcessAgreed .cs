namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202508190845)]
    public class AddSelfAssessmentProcessAgreed : Migration
    {
        public override void Up()
        {
            Alter.Table("CandidateAssessments").AddColumn("SelfAssessmentProcessAgreed").AsDateTime().Nullable();
        }

        public override void Down()
        {
            Delete.Column("SelfAssessmentProcessAgreed").FromTable("CandidateAssessments");
        }
    }
}
