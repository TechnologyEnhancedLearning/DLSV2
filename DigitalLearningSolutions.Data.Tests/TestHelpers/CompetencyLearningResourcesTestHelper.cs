namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using Dapper;
    using Microsoft.Data.SqlClient;

    public class CompetencyLearningResourcesTestHelper
    {
        private readonly SqlConnection connection;

        public CompetencyLearningResourcesTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

        public void InsertLearningResourceReference(int id, int resourceRefId, int adminId, string resourceName, string resourceLink = "")
        {
            connection.Execute(
                @"SET IDENTITY_INSERT dbo.LearningResourceReferences ON
                    INSERT INTO LearningResourceReferences
                    (ID, ResourceRefID, OriginalResourceName, AdminId, ResourceLink)
                    VALUES (@id, @resourceRefId, @resourceName, @adminId, @resourceLink)
                    SET IDENTITY_INSERT dbo.LearningResourceReferences OFF",
                new { id, resourceRefId, resourceName, adminId, resourceLink }
            );
        }

        public int InsertCompetencyLearningResource(
            int id,
            int competencyId,
            int learningResourceReferenceId,
            int adminId
        )
        {
            return connection.QuerySingle<int>(
                @"SET IDENTITY_INSERT dbo.CompetencyLearningResources ON
                    INSERT INTO CompetencyLearningResources
                    (ID, CompetencyId, LearningResourceReferenceID, AdminId)
                    OUTPUT Inserted.ID
                    VALUES (@id, @competencyId, @learningResourceReferenceId, @adminId)
                    SET IDENTITY_INSERT dbo.CompetencyLearningResources OFF",
                new { id, competencyId, learningResourceReferenceId, adminId }
            );
        }

        public void InsertCompetencyResourceAssessmentQuestionParameters(
            int competencyLearningResourceId,
            int assessmentQuestionId,
            bool essential,
            int? relevanceQuestionId,
            bool compareToRoleRequirements,
            int minMatchResult,
            int maxMatchResult
        )
        {
            connection.Execute(
                @"INSERT INTO CompetencyResourceAssessmentQuestionParameters
                    (CompetencyLearningResourceId, AssessmentQuestionID, Essential, RelevanceAssessmentQuestionID, CompareToRoleRequirements, MinResultMatch, MaxResultMatch)
                    VALUES (@competencyLearningResourceId, @assessmentQuestionId, @essential, @relevanceQuestionId, @compareToRoleRequirements, @minMatchResult, @maxMatchResult)",
                new
                {
                    competencyLearningResourceId, assessmentQuestionId, essential, relevanceQuestionId,
                    compareToRoleRequirements, minMatchResult, maxMatchResult,
                }
            );
        }
    }
}
