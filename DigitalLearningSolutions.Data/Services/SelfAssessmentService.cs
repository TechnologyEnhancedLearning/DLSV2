namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;

    public interface ISelfAssessmentService
    {
        SelfAssessment? GetSelfAssessmentForCandidate(int candidateId);
    }

    public class SelfAssessmentService : ISelfAssessmentService
    {
        private readonly IDbConnection connection;

        public SelfAssessmentService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public SelfAssessment? GetSelfAssessmentForCandidate(int candidateId)
        {
            return connection.QueryFirstOrDefault<SelfAssessment>(
                @"SELECT
                        C.SelfAssessmentID AS Id,
                        SA.Name,
                        SA.Description
                    FROM CandidateAssessments C
                    JOIN SelfAssessments SA on C.SelfAssessmentID = SA.ID
                    WHERE C.CandidateID = @candidateId
                ",
                new { candidateId }
            );
        }
    }
}
