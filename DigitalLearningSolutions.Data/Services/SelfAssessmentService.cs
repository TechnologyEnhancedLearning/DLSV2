namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;

    public interface ISelfAssessmentService
    {
        SelfAssessment? GetSelfAssessmentForCandidate(int candidateId);
        Competency? GetNthCompetency(int n, int selfAssessmentId);
        void SetResultForCompetency(int competencyId, int selfAssessmentId, int candidateId, int assessmentQuestionId, int result);
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

        public Competency? GetNthCompetency(int n, int selfAssessmentId)
        {
            if (n >= 10)
            {
                return null;
            }
            return new Competency
            {
                Id = 1,
                CompetencyGroup = "Data, information and content",
                Description = "I understand and stick to guidelines and regulations when using data and information to make sure of security and confidentiality requirements",
                AssessmentQuestions =
                {
                    new AssessmentQuestion { Id = 1, MaxValueDescription = "Very confident", MinValueDescription = "Beginner", Question = "Where are you now" },
                    new AssessmentQuestion { Id = 2, MaxValueDescription = "Very confident", MinValueDescription = "Beginner", Question = "Where do you need to be"}
                }
            };
        }

        public void SetResultForCompetency(int competencyId, int selfAssessmentId, int candidateId, int assessmentQuestionId, int result)
        {
        }
    }
}
