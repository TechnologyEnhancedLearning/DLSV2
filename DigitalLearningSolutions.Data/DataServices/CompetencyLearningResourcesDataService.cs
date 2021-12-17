namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;

    public interface ICompetencyLearningResourcesDataService
    {
        IEnumerable<int> GetCompetencyIdsByLearningResourceReferenceId(int lhResourceReferenceId);
        void AddCompetencyLearningResource(int competencyID, int learningResourceReferenceId, int adminId);
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

        public void AddCompetencyLearningResource(int competencyID, int learningResourceReferenceID, int adminId)
        {
            connection.Execute(
                @$"INSERT INTO CompetencyLearningResources(CompetencyID, LearningResourceReferenceID, AdminID)
                    VALUES (@competencyID, @learningResourceReferenceID, @adminID)",
                new { competencyID, learningResourceReferenceID, adminId }
            );
        }
    }
}
