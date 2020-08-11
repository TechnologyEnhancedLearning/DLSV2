namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;

    public interface ISelfAssessmentService
    {
        SelfAssessment? GetSelfAssessmentForCandidate(int candidateId);
        Competency? GetNthCompetency(int n, int selfAssessmentId, int candidateId); // 1 indexed
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

        public Competency? GetNthCompetency(int n, int selfAssessmentId, int candidateId)
        {
            Competency? competencyResult = null;
            return connection.Query<Competency, AssessmentQuestion, Competency>(
                "WITH CompetencyRowNumber AS " +
                    "(SELECT ROW_NUMBER() OVER (ORDER BY CompetencyID ASC) as RowNo, CompetencyID FROM SelfAssessmentStructure WHERE SelfAssessmentID = @selfAssessmentId) " +
                "SELECT C.ID AS Id, C.Description AS Description, CG.Name AS CompetencyGroup, AQ.ID as Id, AQ.Question, AQ.MaxValueDescription, AQ.MinValueDescription " +
                "FROM Competencies AS C " +
                "INNER JOIN CompetencyGroups AS CG ON C.CompetencyGroupID = CG.ID " +
                "INNER JOIN CompetencyAssessmentQuestions AS CAQ ON CAQ.CompetencyID = C.ID " +
                "INNER JOIN AssessmentQuestions AS AQ ON AQ.ID = CAQ.AssessmentQuestionID " +
                "INNER JOIN CompetencyRowNumber AS CRN on CRN.CompetencyID = C.ID " +
                "INNER JOIN CandidateAssessments AS CA on CA.SelfAssessmentID = @selfAssessmentId AND CA.CandidateID = @candidateId " +
                "WHERE CRN.RowNo = @n",
                (competency, assessmentQuestion) =>
                {
                    if (competencyResult == null)
                    {
                        competencyResult = competency;
                    }
                    competencyResult.AssessmentQuestions.Add(assessmentQuestion);
                    return competencyResult;
                },
                param: new { n, selfAssessmentId, candidateId }
            ).FirstOrDefault();
        }

        public void SetResultForCompetency(int competencyId, int selfAssessmentId, int candidateId, int assessmentQuestionId, int result) { }
    }
}
