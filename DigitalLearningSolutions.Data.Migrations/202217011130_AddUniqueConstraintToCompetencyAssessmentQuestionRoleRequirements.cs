namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202217011130)]
    public class AddUniqueConstraintToCompetencyAssessmentQuestionRoleRequirements : Migration
    {
        public override void Up()
        {
            Create.UniqueConstraint(
                    "IX_CompetencyAssessmentQuestionRoleRequirements_SelfAssessmentID_CompetencyID_AssessmentQuestionID_LevelValue"
                )
                .OnTable("CompetencyAssessmentQuestionRoleRequirements").Columns(
                    "SelfAssessmentID",
                    "CompetencyID",
                    "AssessmentQuestionID",
                    "LevelValue"
                );
        }

        public override void Down()
        {
            Delete.UniqueConstraint(
                    "IX_CompetencyAssessmentQuestionRoleRequirements_SelfAssessmentID_CompetencyID_AssessmentQuestionID_LevelValue"
                )
                .FromTable("CompetencyAssessmentQuestionRoleRequirements");
        }
    }
}
