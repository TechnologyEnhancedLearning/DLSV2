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

        public int InsertCompetencyLearningResource(
            int id,
            int competencyId,
            int lhResourceId,
            int adminId
        )
        {
            return connection.QuerySingle<int>(
                @"SET IDENTITY_INSERT dbo.CompetencyLearningResources ON
                    INSERT INTO CompetencyLearningResources
                    (ID, CompetencyId, LHResourceReferenceID, AdminId)
                    OUTPUT Inserted.ID
                    VALUES (@id, @competencyId, @lhResourceId, @adminId)
                    SET IDENTITY_INSERT dbo.CompetencyLearningResources OFF",
                new { id, competencyId, lhResourceId, adminId }
            );
        }
    }
}
