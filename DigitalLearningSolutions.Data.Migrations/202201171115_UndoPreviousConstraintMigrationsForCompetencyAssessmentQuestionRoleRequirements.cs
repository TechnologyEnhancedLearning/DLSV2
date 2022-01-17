namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202201171115)]
    public class UndoPreviousConstraintMigrationsForCompetencyAssessmentQuestionRoleRequirements : Migration
    {
        public override void Up()
        {
            if (Schema.Table("CompetencyAssessmentQuestionRoleRequirements").Constraint(
                "IX_CompetencyAssessmentQuestionRoleRequirements_SelfAssessmentID_CompetencyID"
            ).Exists())
            {
                // This undoes the changes from 202201120821_ChangeUniqueConstraintsOnCompetencyAssessmentQuestionRoleRequirements
                Delete.UniqueConstraint("IX_CompetencyAssessmentQuestionRoleRequirements_SelfAssessmentID_CompetencyID")
                    .FromTable("CompetencyAssessmentQuestionRoleRequirements");
            }
            else if (!Schema.Table("CompetencyAssessmentQuestionRoleRequirements").Column("ID").Exists())
            {
                // This undoes the changes from 202201111021_ChangeCompetencyAssessmentQuestionRoleRequirementsPrimaryKey
                Delete.PrimaryKey("PK_CompetencyAssessmentQuestionRoleRequirements")
                    .FromTable("CompetencyAssessmentQuestionRoleRequirements");
                Alter.Table("CompetencyAssessmentQuestionRoleRequirements").AddColumn("ID").AsInt32().NotNullable()
                    .Identity();
                Create.PrimaryKey("PK_CompetencyAssessmentQuestionRoleRequirements")
                    .OnTable("CompetencyAssessmentQuestionRoleRequirements").Column("ID");
            }
        }

        public override void Down()
        {
            // The Up migration reverts the database to how it should be.
            // Previous migrations that should not be run have been commented out.
            // Thus this Down migration does not need to do anything.
        }
    }
}
