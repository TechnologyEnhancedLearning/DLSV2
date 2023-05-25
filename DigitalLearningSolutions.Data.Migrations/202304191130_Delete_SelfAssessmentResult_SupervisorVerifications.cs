namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202304191630)]
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
            if(Schema.Table("SelfAssessmentResultsHistory").Column("CandidateID").Exists())
            {
                Execute.Sql("ALTER TABLE SelfAssessmentResults SET (SYSTEM_VERSIONING = OFF);");
                Rename.Column("CandidateID").OnTable("SelfAssessmentResultsHistory").To("CandidateID_deprecated");
                Execute.Sql("ALTER TABLE SelfAssessmentResults SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[SelfAssessmentResultsHistory]));");
            }
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
