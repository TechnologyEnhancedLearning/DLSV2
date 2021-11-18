namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202111181337)]
    public class AddCandidateAssessmentLearningLogItemsTable : Migration
    {
        public override void Up()
        {
            Create.Table("CandidateAssessmentLearningLogItems")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("CandidateAssessmentID").AsInt32().NotNullable().ForeignKey("CandidateAssessments", "ID")
                .WithColumn("LearningLogItemID").AsInt32().NotNullable().ForeignKey("LearningLogItems", "LearningLogItemID");
        }

        public override void Down()
        {
            Delete.Table("CandidateAssessmentLearningLogItems");
        }
    }
}
