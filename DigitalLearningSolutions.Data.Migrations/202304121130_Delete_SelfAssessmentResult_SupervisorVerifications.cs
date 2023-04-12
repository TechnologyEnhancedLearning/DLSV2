namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202304121630)]
    public class Delete_SelfAssessmentResult_SupervisorVerifications : Migration
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


        }

        public override void Down()
        {
            Delete.UniqueConstraint(
                    "IX_SelfAssessmentResults_DelegateUserID_CompetencyID_AssessmentQuestionID"
                )
                .FromTable("SelfAssessmentResults");
        }
    }
}
