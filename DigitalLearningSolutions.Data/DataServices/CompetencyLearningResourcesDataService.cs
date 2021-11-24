namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.LearningResources;

    public interface ICompetencyLearningResourcesDataService
    {
        CompetencyLearningResource GetCompetencyLearningResourceById(int competencyLearningResourceId);

        IEnumerable<int> GetCompetencyIdsByLearningHubResourceReference(int lhResourceReferenceId);
    }

    public class CompetencyLearningResourcesDataService : ICompetencyLearningResourcesDataService
    {
        private readonly IDbConnection connection;

        public CompetencyLearningResourcesDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public CompetencyLearningResource GetCompetencyLearningResourceById(int competencyLearningResourceId)
        {
            return connection.Query<CompetencyLearningResource>(
                @"SELECT
                        ID,
                        CompetencyID,
                        LHResourceReferenceID AS LearningHubResourceReferenceId,
                        AdminID
                    FROM CompetencyLearningResources
                    WHERE ID = @competencyLearningResourceId",
                new { competencyLearningResourceId }
            ).Single();
        }

        public IEnumerable<int> GetCompetencyIdsByLearningHubResourceReference(int lhResourceReferenceId)
        {
            return connection.Query<int>(
                @"SELECT
                        CompetencyID
                    FROM CompetencyLearningResources
                    WHERE LHResourceReferenceID = @lhResourceReferenceId",
                new { lhResourceReferenceId }
            );
        }
    }
}
