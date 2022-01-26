namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202201111021)]
    public class ChangeCompetencyAssessmentQuestionRoleRequirementsPrimaryKey: Migration
    {
        // This migration is undone in 202201171115_UndoPreviousConstraintMigrationsForCompetencyAssessmentQuestionRoleRequirements
        // and does not need to be run again, so it has been commented out

        public override void Up()
        {
            //Delete.PrimaryKey("PK_CompetencyAssessmentQuestionRoleRequirements")
            //    .FromTable("CompetencyAssessmentQuestionRoleRequirements");
            //Delete.Column("ID").FromTable("CompetencyAssessmentQuestionRoleRequirements");
            //Create.PrimaryKey("PK_CompetencyAssessmentQuestionRoleRequirements")
            //    .OnTable("CompetencyAssessmentQuestionRoleRequirements").Columns("SelfAssessmentID", "CompetencyID");
        }

        public override void Down()
        {
            //Delete.PrimaryKey("PK_CompetencyAssessmentQuestionRoleRequirements")
            //    .FromTable("CompetencyAssessmentQuestionRoleRequirements");
            //Alter.Table("CompetencyAssessmentQuestionRoleRequirements").AddColumn("ID").AsInt32().NotNullable().Identity();
            //Create.PrimaryKey("PK_CompetencyAssessmentQuestionRoleRequirements")
            //    .OnTable("CompetencyAssessmentQuestionRoleRequirements").Column("ID");
        }
    }
}
