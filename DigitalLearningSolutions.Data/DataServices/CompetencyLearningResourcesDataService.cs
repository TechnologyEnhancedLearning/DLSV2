namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;

    public interface ICompetencyLearningResourcesDataService
    {
        IEnumerable<int> GetLearningHubResourceReferenceIdsByCompetencyId(int competencyId);

        IEnumerable<int> GetCompetencyLearningResourceIdsByLearningHubResourceReference(int lhResourceReferenceId);

        IEnumerable<int> GetCompetencyIdsByLearningResourceReferenceId(int learningResourceReferenceId);
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

        public IEnumerable<int> GetLearningHubResourceReferenceIdsByCompetencyId(int competencyId)
        {
            return connection.Query<int>(
                @"SELECT
                        LHResourceReferenceID
                    FROM CompetencyLearningResources
                    WHERE CompetencyID = @competencyId",
                new { competencyId }
            );
        }

        public IEnumerable<int> GetCompetencyLearningResourceIdsByLearningHubResourceReference(int lhResourceReferenceId)
        {
            return connection.Query<int>(
                @"SELECT
                        ID
                    FROM CompetencyLearningResources
                    WHERE LHResourceReferenceID = @lhResourceReferenceId",
                new { lhResourceReferenceId }
            );
        }
    }
}
