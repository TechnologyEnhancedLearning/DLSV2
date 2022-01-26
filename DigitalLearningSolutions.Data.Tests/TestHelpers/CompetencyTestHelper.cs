namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using Dapper;
    using Microsoft.Data.SqlClient;

    public class CompetencyTestHelper
    {
        private readonly SqlConnection connection;

        public CompetencyTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

        public void InsertCompetencyAssessmentQuestionRoleRequirement(
            int id,
            int selfAssessmentId,
            int competencyId,
            int assessmentQuestionId,
            int levelRag,
            int levelValue = 0
        )
        {
            connection.Execute(
                @"SET IDENTITY_INSERT dbo.CompetencyAssessmentQuestionRoleRequirements ON
                    INSERT INTO CompetencyAssessmentQuestionRoleRequirements
                    (ID, SelfAssessmentID, CompetencyID, AssessmentQuestionID, LevelValue, LevelRAG)
                    VALUES (@id, @selfAssessmentId, @competencyId, @assessmentQuestionId, @levelValue, @levelRag)
                    SET IDENTITY_INSERT dbo.CompetencyAssessmentQuestionRoleRequirements OFF",
                new { id, selfAssessmentId, competencyId, assessmentQuestionId, levelRag, levelValue }
            );
        }
    }
}
