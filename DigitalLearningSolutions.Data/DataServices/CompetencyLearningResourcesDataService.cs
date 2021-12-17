namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;

    public interface ICompetencyLearningResourcesDataService
    {
        IEnumerable<int> GetCompetencyIdsByLearningResourceReferenceId(int lhResourceReferenceId);
        void AddCompetencyLearningResource(int resourceRefID, string originalResourceName, int competencyID, int adminId);
    }

    public class CompetencyLearningResourcesDataService : ICompetencyLearningResourcesDataService
    {
        private readonly IDbConnection connection;

        public CompetencyLearningResourcesDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<int> GetCompetencyIdsByLearningResourceReferenceId(int learningResourceReferenceId)
        {
            return connection.Query<int>(
                @"SELECT
                        CompetencyID
                    FROM CompetencyLearningResources
                    WHERE LearningResourceReferenceID = @learningResourceReferenceId",
                new { learningResourceReferenceId }
            );
        }

        public void AddCompetencyLearningResource(int resourceRefID, string originalResourceName, int competencyID, int adminId)
        {
            connection.Execute(
                @$" DECLARE @learningResourceReferenceID int
                    IF NOT EXISTS(SELECT * FROM LearningResourceReferences WHERE @resourceRefID = resourceRefID)
                        BEGIN
	                        INSERT INTO LearningResourceReferences(ResourceRefID, OriginalResourceName, AdminID, Added)
	                        VALUES(@resourceRefID, @originalResourceName, @adminID, GETDATE())
	                        SELECT @learningResourceReferenceID = SCOPE_IDENTITY()
                        END
                    ELSE
                        BEGIN
	                        SELECT TOP 1 @learningResourceReferenceID = ID 
	                        FROM LearningResourceReferences 
	                        WHERE @resourceRefID = resourceRefID
                        END
                    INSERT INTO CompetencyLearningResources(CompetencyID, LearningResourceReferenceID, AdminID)
                           VALUES (@competencyID, @learningResourceReferenceID, @adminID)",
                new { resourceRefID, originalResourceName, competencyID, adminId }
            );
        }
    }
}
