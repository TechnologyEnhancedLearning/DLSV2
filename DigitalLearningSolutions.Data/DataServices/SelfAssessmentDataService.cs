namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;

    public interface ISelfAssessmentDataService
    {
        IEnumerable<int> GetCompetencyIdsForSelfAssessment(int selfAssessmentId);
    }

    public class SelfAssessmentDataService : ISelfAssessmentDataService
    {
        private readonly IDbConnection connection;

        public SelfAssessmentDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<int> GetCompetencyIdsForSelfAssessment(int selfAssessmentId)
        {
            return connection.Query<int>(
                @"SELECT
                        CompetencyID
                    FROM SelfAssessmentStructure
                    WHERE SelfAssessmentID = @selfAssessmentId",
                new { selfAssessmentId }
            );
        }
    }
}
