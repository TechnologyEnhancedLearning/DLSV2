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

        public void InsertLearningResourceReference(int id, int resourceRefId, int adminId, string resourceName)
        {
            connection.Execute(
                @"SET IDENTITY_INSERT dbo.LearningResourceReferences ON
                    INSERT INTO LearningResourceReferences
                    (ID, ResourceRefID, OriginalResourceName, AdminId)
                    VALUES (@id, @resourceRefId, @resourceName, @adminId)
                    SET IDENTITY_INSERT dbo.LearningResourceReferences OFF",
                new { id, resourceRefId, resourceName, adminId }
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
            int id,
            int competencyLearningResourceId,
            int assessmentQuestionId,
            bool essential,
            int? relevanceQuestionId,
            bool compareToRoleRequirements
        )
        {
            connection.Execute(
                @"SET IDENTITY_INSERT dbo.CompetencyResourceAssessmentQuestionParameters ON
                    INSERT INTO CompetencyResourceAssessmentQuestionParameters
                    (ID, CompetencyLearningResourceId, AssessmentQuestionID, Essential, RelevanceAssessmentQuestionID, CompareToRoleRequirements, MinResultMatch, MaxResultMatch)
                    VALUES (@id, @competencyLearningResourceId, @assessmentQuestionId, @essential, @relevanceQuestionId, @compareToRoleRequirements, 0, 0)
                    SET IDENTITY_INSERT dbo.CompetencyResourceAssessmentQuestionParameters OFF",
                new { id, competencyLearningResourceId, assessmentQuestionId, essential, relevanceQuestionId, compareToRoleRequirements }
            );
        }
    }
}
