namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202212211315)]
    public class AddForeignKeysToCandidateAssessments : Migration
    {
        public override void Up()
        {

            Create.ForeignKey("FK_CandidateAssessments_SelfAssessmentID_SelfAssessments_ID")
                .FromTable("CandidateAssessments").ForeignColumn("SelfAssessmentID").ToTable("SelfAssessments")
                .PrimaryColumn("ID");
        }
        public override void Down()
        {
            Delete.ForeignKey("FK_CandidateAssessments_SelfAssessmentID_SelfAssessments_ID").OnTable("CandidateAssessments");
        }
    }
}
