namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202107161056)]
    public class SelfAssessmentRAGResultsAndSupervisorRoles : Migration
    {
        public override void Up()
        {
            Delete.PrimaryKey("PK_SelfAssessmentStructure").FromTable("SelfAssessmentStructure");
            Alter.Table("SelfAssessmentStructure").AddColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity();
            Create.PrimaryKey("PK_SelfAssessmentStructure").OnTable("SelfAssessmentStructure").Columns("ID");
            Alter.Table("SelfAssessments")
                .AddColumn("SupervisorSelfAssessmentReview").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("SupervisorResultsReview").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("RAGResults").AsBoolean().NotNullable().WithDefaultValue(false);
            Create.Table("SelfAssessmentSupervisorRoles")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("SelfAssessmentID").AsInt32().NotNullable().ForeignKey("SelfAssessments", "ID")
                .WithColumn("RoleName").AsString(100).NotNullable()
                .WithColumn("SelfAssessmentReview").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("ResultsReview").AsBoolean().NotNullable().WithDefaultValue(true);
            Create.Table("CompetencyAssessmentQuestionRoleRequirements")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("SelfAssessmentID").AsInt32().NotNullable().ForeignKey("SelfAssessments", "ID")
                .WithColumn("CompetencyID").AsInt32().NotNullable().ForeignKey("Competencies", "ID")
                .WithColumn("AssessmentQuestionID").AsInt32().NotNullable().ForeignKey("AssessmentQuestions", "ID")
                .WithColumn("LevelValue").AsInt32().NotNullable()
                .WithColumn("LevelRAG").AsInt32().NotNullable();
        }
        public override void Down()
        {
            Delete.PrimaryKey("PK_SelfAssessmentStructure").FromTable("SelfAssessmentStructure");
            Delete.Column("ID").FromTable("SelfAssessmentStructure");
            Create.PrimaryKey("PK_SelfAssessmentStructure").OnTable("SelfAssessmentStructure").Columns("SelfAssessmentID", "CompetencyID");
            Delete.Table("CompetencyAssessmentQuestionRoleRequirements");
            Delete.Table("SelfAssessmentSupervisorRoles");
            Delete.Column("SupervisorSelfAssessmentReview").FromTable("SelfAssessments");
            Delete.Column("SupervisorResultsReview").FromTable("SelfAssessments");
            Delete.Column("RAGResults").FromTable("SelfAssessments");
        }
    }
}
