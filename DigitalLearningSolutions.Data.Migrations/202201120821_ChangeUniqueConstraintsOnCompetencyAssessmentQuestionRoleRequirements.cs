namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202201120821)]
    public class ChangeUniqueConstraintsOnCompetencyAssessmentQuestionRoleRequirements : Migration
    {
        // This migration is undone in 202201171115_UndoPreviousConstraintMigrationsForCompetencyAssessmentQuestionRoleRequirements
        // and does not need to be run again, so it has been commented out

        public override void Up()
        {
            //Delete.PrimaryKey("PK_CompetencyAssessmentQuestionRoleRequirements")
            //    .FromTable("CompetencyAssessmentQuestionRoleRequirements");
            //Alter.Table("CompetencyAssessmentQuestionRoleRequirements").AddColumn("ID").AsInt32().NotNullable().Identity();
            //Create.PrimaryKey("PK_CompetencyAssessmentQuestionRoleRequirements")
            //    .OnTable("CompetencyAssessmentQuestionRoleRequirements").Column("ID");
            //Create.UniqueConstraint("IX_CompetencyAssessmentQuestionRoleRequirements_SelfAssessmentID_CompetencyID")
            //    .OnTable("CompetencyAssessmentQuestionRoleRequirements").Columns("SelfAssessmentID", "CompetencyID");
        }

        public override void Down()
        {
            //Delete.UniqueConstraint("IX_CompetencyAssessmentQuestionRoleRequirements_SelfAssessmentID_CompetencyID")
            //    .FromTable("CompetencyAssessmentQuestionRoleRequirements");
            //Delete.PrimaryKey("PK_CompetencyAssessmentQuestionRoleRequirements")
            //    .FromTable("CompetencyAssessmentQuestionRoleRequirements");
            //Delete.Column("ID").FromTable("CompetencyAssessmentQuestionRoleRequirements");
            //Create.PrimaryKey("PK_CompetencyAssessmentQuestionRoleRequirements")
            //    .OnTable("CompetencyAssessmentQuestionRoleRequirements").Columns("SelfAssessmentID", "CompetencyID");
        }
    }
}
