namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using Microsoft.Extensions.Logging;

    public interface ISelfAssessmentService
    {
        SelfAssessment? GetSelfAssessmentForCandidate(int candidateId);
        Competency? GetNthCompetency(int n, int selfAssessmentId, int candidateId); // 1 indexed
        void SetResultForCompetency(int competencyId, int selfAssessmentId, int candidateId, int assessmentQuestionId, int result);
        IEnumerable<Competency> GetMostRecentResults(int selfAssessmentId, int candidateId);
    }

    public class SelfAssessmentService : ISelfAssessmentService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SelfAssessmentService> logger;

        private const string LatestAssessmentResults =
            @"LatestAssessmentResults AS
                         (SELECT CompetencyID,
                                 AssessmentQuestionID,
                                 Result
                          FROM SelfAssessmentResults s
                                   INNER JOIN (
                              SELECT MAX(ID) as ID
                              FROM SelfAssessmentResults
                              WHERE CandidateID = @candidateId
                                AND SelfAssessmentID = @selfAssessmentId
                              GROUP BY CompetencyID,
                                       AssessmentQuestionID
                          ) t
                                              ON s.ID = t.ID
                          WHERE CandidateID = @candidateId
                            AND SelfAssessmentID = @selfAssessmentId
                         )";
        private const string CompetencyFields = @"C.ID       AS Id,
                                                  C.Description AS Description,
                                                  CG.Name       AS CompetencyGroup,
                                                  AQ.ID         AS Id,
                                                  AQ.Question,
                                                  AQ.MaxValueDescription,
                                                  AQ.MinValueDescription,
                                                  LAR.Result";

        private const string CompetencyTables =
            @"Competencies AS C
                        INNER JOIN CompetencyGroups AS CG
                            ON C.CompetencyGroupID = CG.ID
                        INNER JOIN CompetencyAssessmentQuestions AS CAQ
                            ON CAQ.CompetencyID = C.ID
                        INNER JOIN AssessmentQuestions AS AQ
                            ON AQ.ID = CAQ.AssessmentQuestionID
                        INNER JOIN CandidateAssessments AS CA
                            ON CA.SelfAssessmentID = @selfAssessmentId
                                   AND CA.CandidateID = @candidateId
                        LEFT OUTER JOIN LatestAssessmentResults AS LAR
                            ON LAR.CompetencyID = C.ID
                                   AND LAR.AssessmentQuestionID = AQ.ID";

        public SelfAssessmentService(IDbConnection connection, ILogger<SelfAssessmentService> logger)
        {
            this.connection = connection;
            this.logger = logger;
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
                $@"WITH CompetencyRowNumber AS
                     (SELECT ROW_NUMBER() OVER (ORDER BY CompetencyID) as RowNo,
                             CompetencyID
                      FROM SelfAssessmentStructure
                      WHERE SelfAssessmentID = @selfAssessmentId
                     ),
                     {LatestAssessmentResults}
                    SELECT {CompetencyFields}
                    FROM {CompetencyTables}
                        INNER JOIN CompetencyRowNumber AS CRN
                            ON CRN.CompetencyID = C.ID
                    WHERE CRN.RowNo = @n",
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

        public void SetResultForCompetency(int competencyId, int selfAssessmentId, int candidateId, int assessmentQuestionId, int result)
        {
            if (result < 0 || result > 10)
            {
                logger.LogWarning(
                    "Not saving self assessment result as result is invalid. " +
                    $"{PrintResult(competencyId, selfAssessmentId, candidateId, assessmentQuestionId, result)}"
                );
                return;
            }

            var numberOfAffectedRows = connection.Execute(
                @"IF EXISTS (
                        SELECT * FROM CandidateAssessments AS CA
	                    INNER JOIN SelfAssessmentStructure AS SAS ON CA.SelfAssessmentID = SAS.SelfAssessmentID
	                    INNER JOIN Competencies AS C ON SAS.CompetencyID = C.ID
	                    INNER JOIN CompetencyAssessmentQuestions as CAQ ON SAS.CompetencyID = CAQ.CompetencyID
	                    WHERE CandidateID = @candidateId
                            AND CA.SelfAssessmentID = @selfAssessmentId
                            AND C.ID = @competencyId
                            AND CAQ.AssessmentQuestionID = @assessmentQuestionId
                    )
                    BEGIN
                        INSERT INTO SelfAssessmentResults VALUES(@candidateId, @selfAssessmentId, @competencyId, @assessmentQuestionId, @result, GETDATE())
                    END",
                new { competencyId, selfAssessmentId, candidateId, assessmentQuestionId, result }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not saving self assessment result as db insert failed. " +
                    $"{PrintResult(competencyId, selfAssessmentId, candidateId, assessmentQuestionId, result)}"
                );
            }
        }

        public IEnumerable<Competency> GetMostRecentResults(int selfAssessmentId, int candidateId)
        {
            var result = connection.Query<Competency, AssessmentQuestion, Competency>(
                $@"WITH {LatestAssessmentResults}
                    SELECT {CompetencyFields}
                    FROM {CompetencyTables}
                        WHERE LAR.Result IS NOT NULL",
                (competency, assessmentQuestion) =>
                {
                    competency.AssessmentQuestions.Add(assessmentQuestion);
                    return competency;
                },
                param: new { selfAssessmentId, candidateId }
            );
            return result.GroupBy(competency => competency.Id).Select(group =>
            {
                var groupedCompetency = group.First();
                groupedCompetency.AssessmentQuestions = group.Select(competency => competency.AssessmentQuestions.Single()).ToList();
                return groupedCompetency;
            });
        }

        private static string PrintResult(int competencyId, int selfAssessmentId, int candidateId, int assessmentQuestionId, int result)
        {
            return $"Competency id: {competencyId}, self assessment id: {selfAssessmentId}, candidate id: {candidateId}, " +
                   $"assessment question id: {assessmentQuestionId}, result: {result}";
        }
    }
}
