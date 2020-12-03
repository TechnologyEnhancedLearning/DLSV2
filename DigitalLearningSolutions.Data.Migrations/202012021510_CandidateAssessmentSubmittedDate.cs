namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202012021510)]
    public class CandidateAssessmentSubmittedDate : Migration
    {
        public override void Up()
        {
            Alter.Table("CandidateAssessments")
                .AddColumn("SubmittedDate").AsDateTime().Nullable();
        }
        public override void Down()
        {
            Delete.Column("SubmittedDate").FromTable("CandidateAssessments");
        }
    }
}
