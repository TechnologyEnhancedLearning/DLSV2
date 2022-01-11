namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202201111021)]
    public class ChangeCompetencyAssessmentQuestionRoleRequirementsPrimaryKey: Migration
    {
        public override void Up()
        {
            Delete.PrimaryKey("PK_CompetencyAssessmentQuestionRoleRequirements")
                .FromTable("CompetencyAssessmentQuestionRoleRequirements");
            Delete.Column("ID").FromTable("CompetencyAssessmentQuestionRoleRequirements");
            Create.PrimaryKey("PK_CompetencyAssessmentQuestionRoleRequirements")
                .OnTable("CompetencyAssessmentQuestionRoleRequirements").Columns("SelfAssessmentID", "CompetencyID");
        }

        public override void Down()
        {
            Delete.PrimaryKey("PK_CompetencyAssessmentQuestionRoleRequirements")
                .FromTable("CompetencyAssessmentQuestionRoleRequirements");
            Alter.Table("CompetencyAssessmentQuestionRoleRequirements").AddColumn("ID").AsInt32().NotNullable()
                .PrimaryKey().Identity();
        }
    }
}
