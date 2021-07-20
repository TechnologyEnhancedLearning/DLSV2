namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202007271634)]
    public class AddSelfAssessmentTables : Migration
    {
        public override void Up()
        {
            Create.Table("SelfAssessments")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Name").AsString(255).NotNullable()
                .WithColumn("Description").AsString(int.MaxValue).NotNullable();
            Create.Table("CompetencyGroups")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Name").AsString(255).NotNullable();
            Create.Table("AssessmentQuestions")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Question").AsString(int.MaxValue).NotNullable()
                .WithColumn("MaxValueDescription").AsString(int.MaxValue).NotNullable()
                .WithColumn("MinValueDescription").AsString(int.MaxValue).NotNullable();
            Create.Table("Competencies")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Description").AsString(int.MaxValue).NotNullable()
                .WithColumn("CompetencyGroupID").AsInt32().NotNullable().ForeignKey("CompetencyGroups", "ID");
            Create.Table("CompetencyAssessmentQuestions")
                .WithColumn("CompetencyID").AsInt32().NotNullable().PrimaryKey().ForeignKey("Competencies", "ID")
                .WithColumn("AssessmentQuestionID").AsInt32().NotNullable().PrimaryKey().ForeignKey("Competencies", "ID");
            Create.Table("CandidateAssessments")
                .WithColumn("CandidateID").AsInt32().NotNullable().PrimaryKey().ForeignKey("Candidates", "CandidateID")
                .WithColumn("SelfAssessmentID").AsInt32().NotNullable().PrimaryKey().ForeignKey("SelfAssessments", "ID");
            Create.Table("SelfAssessmentStructure")
                .WithColumn("SelfAssessmentID").AsInt32().NotNullable().PrimaryKey().ForeignKey("SelfAssessments", "ID")
                .WithColumn("CompetencyID").AsInt32().NotNullable().PrimaryKey().ForeignKey("Competencies", "ID");
            Create.Table("SelfAssessmentResults")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("CandidateID").AsInt32().NotNullable().ForeignKey("Candidates", "CandidateID")
                .WithColumn("SelfAssessmentID").AsInt32().NotNullable().ForeignKey("SelfAssessments", "ID")
                .WithColumn("CompetencyID").AsInt32().NotNullable().ForeignKey("Competencies", "ID")
                .WithColumn("AssessmentQuestionID").AsInt32().NotNullable().ForeignKey("AssessmentQuestions", "ID")
                .WithColumn("Result").AsInt32().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("SelfAssessmentResults");
            Delete.Table("SelfAssessmentStructure");
            Delete.Table("CandidateAssessments");
            Delete.Table("CompetencyAssessmentQuestions");
            Delete.Table("Competencies");
            Delete.Table("AssessmentQuestions");
            Delete.Table("CompetencyGroups");
            Delete.Table("SelfAssessments");
        }
    }
}
