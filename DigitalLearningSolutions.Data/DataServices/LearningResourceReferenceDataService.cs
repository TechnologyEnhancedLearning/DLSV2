namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Data;
    using System.Linq;
    using Dapper;

    public interface ILearningResourceReferenceDataService
    {
        int GetLearningHubResourceReferenceById(int learningResourceReferenceId);
        string? GetLearningHubResourceLinkById(int learningResourceReferenceId);
    }

    public class LearningResourceReferenceDataService : ILearningResourceReferenceDataService
    {
        private readonly IDbConnection connection;

        public LearningResourceReferenceDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public int GetLearningHubResourceReferenceById(int learningResourceReferenceId)
        {
            return connection.Query<int>(
                @"SELECT ResourceRefID
                    FROM LearningResourceReferences
                    WHERE ID = @learningResourceReferenceId",
                new { learningResourceReferenceId }
            ).Single();
        }

        public string? GetLearningHubResourceLinkById(int learningResourceReferenceId)
        {

            return connection.Query<string?>(
                @"SELECT ResourceLink
                    FROM LearningResourceReferences
                    WHERE ID = @learningResourceReferenceId",
                new { learningResourceReferenceId }
            ).Single();
        }
    }
}
