namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202109080846)]
    public class FixCompetencyIDFKConstraint : Migration
    {
        public override void Up()
        {
            Delete.ForeignKey("FK_CompetencyAssessmentQuestions_AssessmentQuestionID_Competencies_ID").OnTable("CompetencyAssessmentQuestions");
            Alter.Column("AssessmentQuestionID").OnTable("CompetencyAssessmentQuestions").AsInt32().NotNullable().ForeignKey("AssessmentQuestions", "ID");
        }
        public override void Down()
        {
            Delete.ForeignKey("FK_CompetencyAssessmentQuestions_AssessmentQuestionID_AssessmentQuestionID_ID").OnTable("CompetencyAssessmentQuestions");
            Alter.Column("AssessmentQuestionID").OnTable("CompetencyAssessmentQuestions").AsInt32().NotNullable().ForeignKey("Competencies", "ID");
        }
    }
}
