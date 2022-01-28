namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
    using DigitalLearningSolutions.Data.Models.LearningResources;

    public interface ILearningResourceReferenceDataService
    {
        int GetLearningHubResourceReferenceById(int learningResourceReferenceId);

        string? GetLearningHubResourceLinkByResourceRefId(int learningResourceReferenceId);

        public IEnumerable<ResourceReferenceWithResourceDetails> GetResourceReferenceDetailsByReferenceIds(
            IEnumerable<int> resourceReferenceIds
        );
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

        public string? GetLearningHubResourceLinkByResourceRefId(int learningResourceReferenceId)
        {
            return connection.Query<string?>(
                @"SELECT ResourceLink
                    FROM LearningResourceReferences
                    WHERE ResourceRefID = @learningResourceReferenceId",
                new { learningResourceReferenceId }
            ).Single();
        }

        public IEnumerable<ResourceReferenceWithResourceDetails> GetResourceReferenceDetailsByReferenceIds(
            IEnumerable<int> resourceReferenceIds
        )
        {
            var results = connection.Query<LearningResourceReference>(
                @"SELECT
                        ID,
                        ResourceRefID,
                        AdminID,
                        Added,
                        OriginalResourceName,
                        ResourceLink,
                        OriginalDescription,
                        OriginalResourceType,
                        OriginalCatalogueName,
                        OriginalRating
                    FROM LearningResourceReferences
                    WHERE ResourceRefID IN @resourceReferenceIds",
                new { resourceReferenceIds }
            );

            return results.Select(r => r.MapToResourceReferenceWithResourceDetails());
        }
    }
}
