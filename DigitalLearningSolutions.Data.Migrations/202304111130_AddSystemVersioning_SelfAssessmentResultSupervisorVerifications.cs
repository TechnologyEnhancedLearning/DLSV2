namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202304111330)]
    public class AddSystemVersioning_SelfAssessmentResultSupervisorVerifications : Migration
    {
        public override void Up()
        {
            Execute.Sql(
                   @$"DELETE FROM SelfAssessmentResultSupervisorVerifications
                        WHERE SelfAssessmentResultId NOT IN (SELECT MAX(ID)
                        FROM SelfAssessmentResults
                        GROUP BY CompetencyID, AssessmentQuestionID, DelegateUserID)"
               );
            Execute.Sql(
                  @$"DELETE FROM SelfAssessmentResults
                        WHERE ID NOT IN (
                        SELECT MAX(ID)
                        FROM SelfAssessmentResults
                        GROUP BY CompetencyID, AssessmentQuestionID, DelegateUserID)"
              );
            Create.UniqueConstraint(
                    "IX_SelfAssessmentResults_DelegateUserID_CompetencyID_AssessmentQuestionID"
                )
                .OnTable("SelfAssessmentResults").Columns(
                    "DelegateUserID",
                    "CompetencyID",
                    "AssessmentQuestionID"
                );
            Execute.Sql(Properties.Resources.TD_1220_AddSystemVersioning_SelfAssessmentResultSupervisorVerifications);

        }

        public override void Down()
        {
            Delete.UniqueConstraint(
                    "IX_SelfAssessmentResults_DelegateUserID_CompetencyID_AssessmentQuestionID"
                )
                .FromTable("SelfAssessmentResults");
            Execute.Sql(Properties.Resources.TD_1220_RemoveSystemVersioning_SelfAssessmentResultSupervisorVerifications);
        }
    }
}
