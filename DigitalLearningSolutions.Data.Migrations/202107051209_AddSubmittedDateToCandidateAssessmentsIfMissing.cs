namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202107051209)]
    public class AddSubmittedDateToCandidateAssessmentsIfMissing : Migration
    {
        public override void Up()
        {
            if(!Schema.Table("CandidateAssessments").Column("SubmittedDate").Exists())
            {
                Alter.Table("CandidateAssessments").AddColumn("SubmittedDate").AsDateTime().Nullable();
            }
        }
        public override void Down()
        {
            if (!Schema.Table("CandidateAssessments").Column("SubmittedDate").Exists())
            {
                Alter.Table("CandidateAssessments").AddColumn("SubmittedDate").AsDateTime().Nullable();
            }
        }

    }
}
