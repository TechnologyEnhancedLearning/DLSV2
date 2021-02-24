namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202102081555)]
    public class AssessmentQuestionMaxMinDescriptionNullable : Migration
    {
        public override void Up()
        {
            Alter.Column("MaxValueDescription").OnTable("AssessmentQuestions").AsString(255).Nullable();
            Alter.Column("MinValueDescription").OnTable("AssessmentQuestions").AsString(255).Nullable();
            Alter.Table("CompetencyAssessmentQuestions").AddColumn("Ordering").AsInt32().NotNullable().WithDefaultValue(0);
            Rename.Column("LevelValueID").OnTable("AssessmentQuestionLevels").To("LevelValue");
        }
        public override void Down()
        {
            Alter.Column("MaxValueDescription").OnTable("AssessmentQuestions").AsString(int.MaxValue).NotNullable();
            Alter.Column("MinValueDescription").OnTable("AssessmentQuestions").AsString(int.MaxValue).NotNullable();
            Delete.Column("Ordering").FromTable("CompetencyAssessmentQuestions");
            Rename.Column("LevelValue").OnTable("AssessmentQuestionLevels").To("LevelValueID");
        }
    }
}
