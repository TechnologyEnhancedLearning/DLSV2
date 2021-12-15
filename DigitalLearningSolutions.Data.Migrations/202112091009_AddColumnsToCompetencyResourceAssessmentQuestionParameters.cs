namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202112091009)]
    public class AddColumnsToCompetencyResourceAssessmentQuestionParameters : Migration
    {
        public override void Up()
        {
            Alter.Table("CompetencyResourceAssessmentQuestionParameters")
                .AddColumn("RelevanceAssessmentQuestionID").AsInt32().Nullable().WithDefaultValue(null)
                .ForeignKey("AssessmentQuestions", "ID")
                .AddColumn("CompareToRoleRequirements").AsBoolean().NotNullable().WithDefaultValue(false);
        }

        public override void Down()
        {
            Delete.ForeignKey(
                    "FK_CompetencyResourceAssessmentQuestionParameters_RelevanceAssessmentQuestionID_AssessmentQuestions_ID"
                )
                .OnTable("CompetencyResourceAssessmentQuestionParameters");
            Delete.Column("RelevanceAssessmentQuestionID").FromTable("CompetencyResourceAssessmentQuestionParameters");
            Delete.Column("CompareToRoleRequirements").FromTable("CompetencyResourceAssessmentQuestionParameters");
        }
    }
}
