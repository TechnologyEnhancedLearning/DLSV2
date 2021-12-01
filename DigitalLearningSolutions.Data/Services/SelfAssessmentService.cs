namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.External.Filtered;
    using Microsoft.Extensions.Logging;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.Common.Users;

    public interface ISelfAssessmentService
    {
        //GET
        IEnumerable<CurrentSelfAssessment> GetSelfAssessmentsForCandidate(int candidateId);
        CurrentSelfAssessment? GetSelfAssessmentForCandidateById(int candidateId, int selfAssessmentId);
        Competency? GetNthCompetency(int n, int selfAssessmentId, int candidateId); // 1 indexed
        IEnumerable<LevelDescriptor> GetLevelDescriptorsForAssessmentQuestion(int assessmentQuestionId, int minValue, int maxValue, bool zeroBased);
        SupervisorComment GetSupervisorComments(int candidateId, int resultId);
        IEnumerable<Competency> GetMostRecentResults(int selfAssessmentId, int candidateId);
        IEnumerable<Competency> GetCandidateAssessmentResultsById(int candidateAssessmentId, int adminId);
        IEnumerable<Competency> GetCandidateAssessmentResultsForReviewById(int candidateAssessmentId, int adminId);
        IEnumerable<Competency> GetCandidateAssessmentResultsToVerifyById(int selfAssessmentId, int candidateId);
        IEnumerable<Competency> GetCandidateAssessmentOptionalCompetencies(int selfAssessmentId, int candidateId);
        Competency GetCompetencyByCandidateAssessmentResultId(int resultId, int candidateAssessmentId, int adminId);
        SelfAssessmentSupervisor GetSupervisorForSelfAssessmentId(int selfAssessmentId, int candidateId);
        IEnumerable<SelfAssessmentSupervisor> GetSupervisorsForSelfAssessmentId(int selfAssessmentId, int candidateId);
        IEnumerable<SelfAssessmentSupervisor> GetOtherSupervisorsForCandidate(int selfAssessmentId, int candidateId);
        IEnumerable<SelfAssessmentSupervisor> GetAllSupervisorsForSelfAssessmentId(int selfAssessmentId, int candidateId);
        IEnumerable<SelfAssessmentSupervisor> GetResultReviewSupervisorsForSelfAssessmentId(int selfAssessmentId, int candidateId);
        IEnumerable<SelfAssessmentSupervisor> GetSignOffSupervisorsForSelfAssessmentId(int selfAssessmentId, int candidateId);
        SelfAssessmentSupervisor GetSelfAssessmentSupervisorByCandidateAssessmentSupervisorId(int candidateAssessmentSupervisorId);
        List<int> GetCandidateAssessmentIncludedSelfAssessmentStructureIds(int selfAssessmentId, int candidateId);
        Profile? GetFilteredProfileForCandidateById(int candidateId, int selfAssessmentId);
        IEnumerable<Goal> GetFilteredGoalsForCandidateId(int candidateId, int selfAssessmentId);
        IEnumerable<Administrator> GetValidSupervisorsForActivity(int centreId, int selfAssessmentId, int candidateId);
        Administrator GetSupervisorByAdminId(int supervisorAdminId);
        IEnumerable<SupervisorSignOff>? GetSupervisorSignOffsForCandidateAssessment(int selfAssessmentId, int candidateId);
        //UPDATE
        void UpdateLastAccessed(int selfAssessmentId, int candidateId);
        void SetSubmittedDateNow(int selfAssessmentId, int candidateId);
        void IncrementLaunchCount(int selfAssessmentId, int candidateId);
        void SetUpdatedFlag(int selfAssessmentId, int candidateId, bool status);
        void SetBookmark(int selfAssessmentId, int candidateId, string bookmark);
        void SetCompleteByDate(int selfAssessmentId, int candidateId, DateTime? completeByDate);
        void UpdateCandidateAssessmentOptionalCompetencies(int selfAssessmentStructureId, int candidateId);
        void UpdateCandidateAssessmentSupervisorVerificationEmailSent(int candidateAssessmentSupervisorVerificationId);
        //INSERT
        void LogAssetLaunch(int candidateId, int selfAssessmentId, LearningAsset learningAsset);
        void SetResultForCompetency(int competencyId, int selfAssessmentId, int candidateId, int assessmentQuestionId, int? result, string? supportingComments);
        void InsertCandidateAssessmentOptionalCompetenciesIfNotExist(int selfAssessmentId, int candidateId);
        void InsertCandidateAssessmentSupervisorVerification(int candidateAssessmentSupervisorId);
    }

    public class SelfAssessmentService : ISelfAssessmentService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SelfAssessmentService> logger;

        private const string LatestAssessmentResults =
            @"LatestAssessmentResults AS
                         (SELECT s.CompetencyID,
                                 s.AssessmentQuestionID,
                                 s.ID AS ResultID,
                                 s.Result,
                                 s.SupportingComments,
								 sv.ID AS SelfAssessmentResultSupervisorVerificationId,
								 sv.Requested,
								 sv.Verified,
								 sv.Comments,
								 sv.SignedOff,
                                 0 AS UserIsVerifier,
                                 COALESCE (rr.LevelRAG, 0) AS ResultRAG
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
											  LEFT OUTER JOIN SelfAssessmentResultSupervisorVerifications AS sv
											  ON s.ID = sv.SelfAssessmentResultId AND sv.Superceded = 0
                                LEFT OUTER JOIN CompetencyAssessmentQuestionRoleRequirements rr
                                ON s.CompetencyID = rr.CompetencyID AND s.AssessmentQuestionID = rr.AssessmentQuestionID AND s.SelfAssessmentID = rr.SelfAssessmentID AND s.Result = rr.LevelValue

                          WHERE CandidateID = @candidateId
                            AND s.SelfAssessmentID = @selfAssessmentId
                         )";
        private const string SpecificAssessmentResults =
            @"LatestAssessmentResults AS
                         (SELECT s.CompetencyID,
                                 s.AssessmentQuestionID,
                                 s.ID AS ResultID,
                                 s.Result,
                                 s.SupportingComments,
								 sv.ID AS SelfAssessmentResultSupervisorVerificationId,
								 sv.Requested,
								 sv.Verified,
								 sv.Comments,
								 sv.SignedOff, 
								 CAST(CASE WHEN COALESCE(sd.SupervisorAdminID, 0) = @adminId THEN 1 ELSE 0 END AS Bit) AS UserIsVerifier,
                                 COALESCE (rr.LevelRAG, 0) AS ResultRAG
                           FROM CandidateAssessments ca INNER JOIN SelfAssessmentResults s ON s.CandidateID = ca.CandidateID AND s.SelfAssessmentID = ca.SelfAssessmentID
                                   INNER JOIN (
                              SELECT MAX(s1.ID) as ID
                              FROM SelfAssessmentResults AS s1 INNER JOIN CandidateAssessments AS ca1 ON  s1.CandidateID = ca1.CandidateID AND s1.SelfAssessmentID = ca1.SelfAssessmentID
                              WHERE ca1.ID = @candidateAssessmentId
                              GROUP BY CompetencyID,
                                       AssessmentQuestionID
                          ) t
                                              ON s.ID = t.ID
											  LEFT OUTER JOIN SelfAssessmentResultSupervisorVerifications AS sv
											  ON s.ID = sv.SelfAssessmentResultId AND sv.Superceded = 0
											  LEFT OUTER JOIN CandidateAssessmentSupervisors AS cas 
											  ON sv.CandidateAssessmentSupervisorID = cas.ID
											  LEFT OUTER JOIN SupervisorDelegates AS sd
											  ON cas.SupervisorDelegateId = sd.ID

                                LEFT OUTER JOIN CompetencyAssessmentQuestionRoleRequirements rr
                                ON s.CompetencyID = rr.CompetencyID AND s.AssessmentQuestionID = rr.AssessmentQuestionID AND s.SelfAssessmentID = rr.SelfAssessmentID AND s.Result = rr.LevelValue
                          WHERE ca.ID = @candidateAssessmentId
                         )";
        private const string CompetencyFields = @"C.ID       AS Id,
                                                  DENSE_RANK() OVER (ORDER BY SAS.Ordering) as RowNo,
                                                  C.Name AS Name,
                                                  C.Description AS Description,
                                                  CG.Name       AS CompetencyGroup,
                                                  CG.ID AS CompetencyGroupID,
                                                  COALESCE((SELECT TOP(1) FrameworkConfig FROM Frameworks F INNER JOIN FrameworkCompetencies AS FC ON FC.FrameworkID = F.ID WHERE FC.CompetencyID = C.ID), 'Capability') AS Vocabulary,
                                                  CASE WHEN (SELECT COUNT(*) FROM SelfAssessmentSupervisorRoles WHERE SelfAssessmentID = SAS.SelfAssessmentID) > 0 THEN 1 ELSE 0 END AS HasDelegateNominatedRoles,
                                                  SAS.Optional,
                                                  AQ.ID         AS Id,
                                                  AQ.Question,
                                                  AQ.MaxValueDescription,
                                                  AQ.MinValueDescription,
                                                  AQ.ScoringInstructions,
                                                  AQ.MinValue,
                                                  AQ.MaxValue,
                                                  AQ.AssessmentQuestionInputTypeID,
                                                  AQ.IncludeComments,
                                                  AQ.CommentsPrompt,
                                                  AQ.CommentsHint,
                                                  CAQ.Required,
                                                  LAR.ResultId,
                                                  LAR.Result,
                                                  LAR.SupportingComments,
												  LAR.SelfAssessmentResultSupervisorVerificationId,
												  LAR.Requested,
												  LAR.Verified,
												  LAR.Comments AS SupervisorComments,
												  LAR.SignedOff,
                                                  LAR.UserIsVerifier,
                                                  LAR.ResultRAG";

        private const string CompetencyTables =
            @"Competencies AS C
                        INNER JOIN CompetencyAssessmentQuestions AS CAQ
                            ON CAQ.CompetencyID = C.ID
                        INNER JOIN AssessmentQuestions AS AQ
                            ON AQ.ID = CAQ.AssessmentQuestionID
                        INNER JOIN CandidateAssessments AS CA
                            ON CA.SelfAssessmentID = @selfAssessmentId
                                   AND CA.CandidateID = @candidateId AND CA.RemovedDate IS NULL
                        LEFT OUTER JOIN LatestAssessmentResults AS LAR
                            ON LAR.CompetencyID = C.ID
                                   AND LAR.AssessmentQuestionID = AQ.ID
                        INNER JOIN SelfAssessmentStructure AS SAS
                            ON C.ID = SAS.CompetencyID
                                    AND SAS.SelfAssessmentID = @selfAssessmentId
                        INNER JOIN CompetencyGroups AS CG
                            ON SAS.CompetencyGroupID = CG.ID
                                    AND SAS.SelfAssessmentID = @selfAssessmentId
                        LEFT OUTER JOIN CandidateAssessmentOptionalCompetencies AS CAOC
                            ON CA.ID = CAOC.CandidateAssessmentID AND C.ID = CAOC.CompetencyID AND CG.ID = CAOC.CompetencyGroupID";

        private const string SpecificCompetencyTables = @"Competencies AS C INNER JOIN
             CompetencyAssessmentQuestions AS CAQ ON CAQ.CompetencyID = C.ID INNER JOIN
             AssessmentQuestions AS AQ ON AQ.ID = CAQ.AssessmentQuestionID INNER JOIN
             CandidateAssessments AS CA ON CA.ID = @candidateAssessmentId LEFT OUTER JOIN
             LatestAssessmentResults AS LAR ON LAR.CompetencyID = C.ID AND LAR.AssessmentQuestionID = AQ.ID INNER JOIN
             SelfAssessmentStructure AS SAS ON C.ID = SAS.CompetencyID AND SAS.SelfAssessmentID = CA.SelfAssessmentID INNER JOIN
             CompetencyGroups AS CG ON SAS.CompetencyGroupID = CG.ID AND SAS.SelfAssessmentID = CA.SelfAssessmentID
                        LEFT OUTER JOIN CandidateAssessmentOptionalCompetencies AS CAOC
                            ON CA.ID = CAOC.CandidateAssessmentID AND C.ID = CAOC.CompetencyID AND CG.ID = CAOC.CompetencyGroupID";

        public SelfAssessmentService(IDbConnection connection, ILogger<SelfAssessmentService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }
        public IEnumerable<CurrentSelfAssessment> GetSelfAssessmentsForCandidate(int candidateId)
        {
            return connection.Query<CurrentSelfAssessment>(
                @"SELECT CA.SelfAssessmentID AS Id,
                             SA.Name,
                             SA.Description,
                             SA.IncludesSignposting,
                             SA.SupervisorResultsReview AS IsSupervisorResultsReviewed,
                             COALESCE(SA.Vocabulary, 'Capability') AS Vocabulary,
                             COUNT(C.ID)         AS NumberOfCompetencies,
                             CA.StartedDate,
                             CA.LastAccessed,
                             CA.CompleteByDate,
                             CA.UserBookmark,
                             CA.UnprocessedUpdates,
                      CA.LaunchCount, 1 AS IsSelfAssessment, CA.SubmittedDate
                      FROM CandidateAssessments CA
                               JOIN SelfAssessments SA
                                    ON CA.SelfAssessmentID = SA.ID
                               INNER JOIN SelfAssessmentStructure AS SAS
                                          ON CA.SelfAssessmentID = SAS.SelfAssessmentID
                               INNER JOIN Competencies AS C
                                          ON SAS.CompetencyID = C.ID
                      WHERE CA.CandidateID = @candidateId AND CA.RemovedDate IS NULL AND CA.CompletedDate IS NULL
                      GROUP BY CA.SelfAssessmentID, SA.Name, SA.Description, SA.IncludesSignposting, SA.SupervisorResultsReview, COALESCE(SA.Vocabulary, 'Capability'), CA.StartedDate, CA.LastAccessed, CA.CompleteByDate, CA.UserBookmark, CA.UnprocessedUpdates, CA.LaunchCount, CA.SubmittedDate",
                new { candidateId }
            );
        }
        public CurrentSelfAssessment? GetSelfAssessmentForCandidateById(int candidateId, int selfAssessmentId)
        {
            return connection.QueryFirstOrDefault<CurrentSelfAssessment>(
                @"SELECT CA.SelfAssessmentID AS Id,
                             SA.Name,
                             SA.Description,
                             SA.QuestionLabel,
							 SA.DescriptionLabel,
                             SA.IncludesSignposting,
                             SA.SupervisorResultsReview AS IsSupervisorResultsReviewed,
                             COALESCE(SA.Vocabulary, 'Capability') AS Vocabulary,
                             COUNT(C.ID)         AS NumberOfCompetencies,
                             CA.StartedDate,
                             CA.LastAccessed,
                             CA.CompleteByDate,
                             CA.UserBookmark,
                             CA.UnprocessedUpdates,
                             CA.LaunchCount, CA.SubmittedDate, SA.LinearNavigation, SA.UseDescriptionExpanders, SA.ManageOptionalCompetenciesPrompt,
                                                  CAST(CASE WHEN SA.SupervisorSelfAssessmentReview = 1 OR SA.SupervisorResultsReview = 1 THEN 1 ELSE 0 END AS BIT) AS IsSupervised,
                                                  CASE WHEN (SELECT COUNT(*) FROM SelfAssessmentSupervisorRoles WHERE SelfAssessmentID = @selfAssessmentId AND AllowDelegateNomination = 1) > 0 THEN 1 ELSE 0 END AS HasDelegateNominatedRoles, COALESCE
                 ((SELECT TOP (1) RoleName
                  FROM    SelfAssessmentSupervisorRoles
                  WHERE (ResultsReview = 1) AND (SelfAssessmentID = @selfAssessmentId) AND
                                   ((SELECT COUNT(*) AS Expr1
                                    FROM    SelfAssessmentSupervisorRoles AS SelfAssessmentSupervisorRoles_1
                                    WHERE (ResultsReview = 1) AND (SelfAssessmentID = @selfAssessmentId)) = 1)), 'Supervisor') AS VerificationRoleName,
                COALESCE
                 ((SELECT TOP (1) RoleName
                  FROM    SelfAssessmentSupervisorRoles
                  WHERE (SelfAssessmentReview = 1) AND (SelfAssessmentID = @selfAssessmentId) AND
                                   ((SELECT COUNT(*) AS Expr1
                                    FROM    SelfAssessmentSupervisorRoles AS SelfAssessmentSupervisorRoles_1
                                    WHERE (SelfAssessmentReview = 1) AND (SelfAssessmentID = @selfAssessmentId)) = 1)), 'Supervisor') AS SignOffRoleName,
                                    SA.SignOffRequestorStatement
                             FROM CandidateAssessments CA
                               JOIN SelfAssessments SA
                                    ON CA.SelfAssessmentID = SA.ID
                               INNER JOIN SelfAssessmentStructure AS SAS
                                          ON CA.SelfAssessmentID = SAS.SelfAssessmentID
                               INNER JOIN Competencies AS C
                                          ON SAS.CompetencyID = C.ID
  INNER JOIN CompetencyGroups AS CG
                            ON SAS.CompetencyGroupID = CG.ID
                                    AND SAS.SelfAssessmentID = @selfAssessmentId
                        LEFT OUTER JOIN CandidateAssessmentOptionalCompetencies AS CAOC
                            ON CA.ID = CAOC.CandidateAssessmentID AND C.ID = CAOC.CompetencyID AND CG.ID = CAOC.CompetencyGroupID
                            WHERE CA.CandidateID = @candidateId AND CA.SelfAssessmentID = @selfAssessmentId AND CA.RemovedDate IS NULL AND CA.CompletedDate IS NULL AND ((SAS.Optional = 0) OR (CAOC.IncludedInSelfAssessment = 1))
                            GROUP BY CA.SelfAssessmentID, SA.Name, SA.Description,
                            SA.DescriptionLabel, SA.QuestionLabel,
                            SA.IncludesSignposting, SA.SignOffRequestorStatement, COALESCE(SA.Vocabulary, 'Capability'),
                            CA.StartedDate, CA.LastAccessed, CA.CompleteByDate, CA.UserBookmark, CA.UnprocessedUpdates,
                            CA.LaunchCount, CA.SubmittedDate, SA.LinearNavigation, SA.UseDescriptionExpanders,
                            SA.ManageOptionalCompetenciesPrompt, SA.SupervisorSelfAssessmentReview, SA.SupervisorResultsReview",
                new { candidateId, selfAssessmentId }
            );
        }

        public Competency? GetNthCompetency(int n, int selfAssessmentId, int candidateId)
        {
            Competency? competencyResult = null;
            return connection.Query<Competency, Models.SelfAssessments.AssessmentQuestion, Competency>(
                $@"WITH CompetencyRowNumber AS
                     (SELECT DENSE_RANK() OVER (ORDER BY SAS.Ordering) as RowNo,
                             sas.CompetencyID
                      FROM SelfAssessmentStructure as sas
INNER JOIN CandidateAssessments AS CA
                            ON CA.SelfAssessmentID = @selfAssessmentId
                                   AND CA.CandidateID = @candidateId
LEFT OUTER JOIN CandidateAssessmentOptionalCompetencies AS CAOC
                            ON CA.ID = CAOC.CandidateAssessmentID AND sas.CompetencyID = CAOC.CompetencyID AND sas.CompetencyGroupID = CAOC.CompetencyGroupID
                      WHERE (sas.SelfAssessmentID = @selfAssessmentId) AND ((sas.Optional = 0) OR (CAOC.IncludedInSelfAssessment = 1))
                     ),
                     {LatestAssessmentResults}
                    SELECT {CompetencyFields}
                    FROM {CompetencyTables}
                        INNER JOIN CompetencyRowNumber AS CRN
                            ON CRN.CompetencyID = C.ID
                    WHERE (CAOC.IncludedInSelfAssessment = 1 OR SAS.Optional = 0) AND CRN.RowNo = @n
                    ORDER BY SAS.Ordering, CAQ.Ordering",
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

        public void SetResultForCompetency(int competencyId, int selfAssessmentId, int candidateId, int assessmentQuestionId, int? result, string? supportingComments)
        {
            var assessmentQuestion = connection.QueryFirstOrDefault<Models.SelfAssessments.AssessmentQuestion>(
                @"SELECT ID, MinValue, MaxValue
                    FROM AssessmentQuestions
                    WHERE ID = @assessmentQuestionId",
                new { assessmentQuestionId }
                );
            if (assessmentQuestion == null)
            {
                logger.LogWarning(
                   "Not saving self assessment result as assessment question Id is invalid. " +
                   $"{PrintResult(competencyId, selfAssessmentId, candidateId, assessmentQuestionId, result)}"
               );
                return;
            }
            int minValue = assessmentQuestion.MinValue;
            int maxValue = assessmentQuestion.MaxValue;
            if (result != null)
            {
                if (result < minValue || result > maxValue)
                {
                    logger.LogWarning(
                        "Not saving self assessment result as result is invalid. " +
                        $"{PrintResult(competencyId, selfAssessmentId, candidateId, assessmentQuestionId, result)}"
                    );
                    return;
                }
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
                        DECLARE @existentResultId INT
                        DECLARE @existentResult INT

                        SELECT TOP 1 @existentResultId = ID, @existentResult = [Result]
                        FROM SelfAssessmentResults
                        WHERE [CandidateID] = @candidateId
	                        AND [SelfAssessmentID] = @selfAssessmentId
	                        AND [CompetencyID] = @competencyId
	                        AND [AssessmentQuestionID] = @assessmentQuestionId
                        ORDER BY DateTime DESC

                        IF (@existentResultId IS NOT NULL AND @existentResult = @result)
	                        UPDATE SelfAssessmentResults
	                        SET [DateTime] = GETUTCDATE(),
		                        [SupportingComments] = @supportingComments
	                        WHERE ID = @existentResultId
                        ELSE
                            INSERT INTO SelfAssessmentResults
                                ([CandidateID]
                                ,[SelfAssessmentID]
                                ,[CompetencyID]
                                ,[AssessmentQuestionID]
                                ,[Result]
                                ,[DateTime]
                                ,[SupportingComments])
                            VALUES(@candidateId, @selfAssessmentId, @competencyId, @assessmentQuestionId, @result, GETUTCDATE(), @supportingComments)
                    END",
                new { competencyId, selfAssessmentId, candidateId, assessmentQuestionId, result, supportingComments }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not saving self assessment result as db insert failed. " +
                    $"{PrintResult(competencyId, selfAssessmentId, candidateId, assessmentQuestionId, result)}"
                );
            }
        }

        public SupervisorComment GetSupervisorComments(int candidateId, int resultId)
        {
            return connection.Query<SupervisorComment>(
              @"SELECT  sar.AssessmentQuestionID, sea.Name, sasv.Comments, sar.CandidateID, sar.CompetencyID, com.Name as CompetencyName,
	sar.SelfAssessmentID, sasv.CandidateAssessmentSupervisorID, sasv.SelfAssessmentResultId, sasv.Verified,
	 sar.ID, sstrc.CompetencyGroupID, sea.Vocabulary, sasv.SignedOff
	 FROM SelfAssessmentResultSupervisorVerifications AS sasv INNER JOIN
	SelfAssessmentResults AS sar ON sasv.SelfAssessmentResultId = sar.ID
	INNER JOIN SelfAssessments AS sea ON sar.SelfAssessmentID = sea.ID
	INNER JOIN SelfAssessmentStructure AS sstrc ON sar.CompetencyID = sstrc.CompetencyID 
	Inner JOIN Competencies AS com ON sar.CompetencyID = com.ID
                    WHERE(sar.CandidateID = @candidateId) AND(sasv.SelfAssessmentResultId = @resultId)", new { candidateId, resultId }
              ).FirstOrDefault();
        }

        public IEnumerable<Competency> GetMostRecentResults(int selfAssessmentId, int candidateId)
        {
            var result = connection.Query<Competency, Models.SelfAssessments.AssessmentQuestion, Competency>(
                $@"WITH {LatestAssessmentResults}
                    SELECT {CompetencyFields}
                    FROM {CompetencyTables}
                    WHERE (CAOC.IncludedInSelfAssessment = 1) OR (SAS.Optional = 0)
                    ORDER BY SAS.Ordering, CAQ.Ordering",
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
        public IEnumerable<Competency> GetCandidateAssessmentResultsById(int candidateAssessmentId, int adminId)
        {
            var result = connection.Query<Competency, Models.SelfAssessments.AssessmentQuestion, Competency>(
                $@"WITH {SpecificAssessmentResults}
                    SELECT {CompetencyFields}
                    FROM {SpecificCompetencyTables}
                    WHERE (CAOC.IncludedInSelfAssessment = 1) OR (SAS.Optional = 0)
                    ORDER BY SAS.Ordering, CAQ.Ordering",
                (competency, assessmentQuestion) =>
                {
                    competency.AssessmentQuestions.Add(assessmentQuestion);
                    return competency;
                },
                param: new { candidateAssessmentId, adminId }
            );
            return result.GroupBy(competency => competency.Id).Select(group =>
            {
                var groupedCompetency = group.First();
                groupedCompetency.AssessmentQuestions = group.Select(competency => competency.AssessmentQuestions.Single()).ToList();
                return groupedCompetency;
            });
        }
        public IEnumerable<Competency> GetCandidateAssessmentResultsForReviewById(int candidateAssessmentId, int adminId)
        {
            var result = connection.Query<Competency, Models.SelfAssessments.AssessmentQuestion, Competency>(
                $@"WITH {SpecificAssessmentResults}
                    SELECT {CompetencyFields}
                    FROM {SpecificCompetencyTables}
                    WHERE (LAR.Requested IS NOT NULL) AND (LAR.Verified IS NULL) AND (LAR.UserIsVerifier = 1) AND ((CAOC.IncludedInSelfAssessment = 1) OR (SAS.Optional = 0))
                    ORDER BY SAS.Ordering, CAQ.Ordering",
                (competency, assessmentQuestion) =>
                {
                    competency.AssessmentQuestions.Add(assessmentQuestion);
                    return competency;
                },
                param: new { candidateAssessmentId, adminId }
            );
            return result.GroupBy(competency => competency.Id).Select(group =>
            {
                var groupedCompetency = group.First();
                groupedCompetency.AssessmentQuestions = group.Select(competency => competency.AssessmentQuestions.Single()).ToList();
                return groupedCompetency;
            });
        }
        public IEnumerable<Competency> GetCandidateAssessmentResultsToVerifyById(int selfAssessmentId, int candidateId)
        {
            var result = connection.Query<Competency, Models.SelfAssessments.AssessmentQuestion, Competency>(
                $@"WITH {LatestAssessmentResults}
                    SELECT {CompetencyFields}
                    FROM {CompetencyTables}
                    WHERE (LAR.Requested IS NULL) AND (LAR.Verified IS NULL) AND ((LAR.Result IS NOT NULL) OR (LAR.SupportingComments IS NOT NULL)) AND ((CAOC.IncludedInSelfAssessment = 1) OR (SAS.Optional = 0))
                    ORDER BY SAS.Ordering, CAQ.Ordering",
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

        public void UpdateLastAccessed(int selfAssessmentId, int candidateId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE CandidateAssessments SET LastAccessed = GETUTCDATE()
                      WHERE SelfAssessmentID = @selfAssessmentId AND CandidateID = @candidateId",
                new { selfAssessmentId, candidateId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating self assessment last accessed date as db update failed. " +
                    $"Self assessment id: {selfAssessmentId}, candidate id: {candidateId}"
                );
            }
        }

        public void SetCompleteByDate(int selfAssessmentId, int candidateId, DateTime? completeByDate)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE CandidateAssessments
                        SET CompleteByDate = @date
                        WHERE SelfAssessmentID = @selfAssessmentId
                          AND CandidateID = @candidateId",
                new { date = completeByDate, selfAssessmentId, candidateId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not setting self assessment complete by date as db update failed. " +
                    $"Self assessment id: {selfAssessmentId}, candidate id: {candidateId}, complete by date: {completeByDate}"
                );
            }
        }

        private static string PrintResult(int competencyId, int selfAssessmentId, int candidateId, int assessmentQuestionId, int? result)
        {
            return $"Competency id: {competencyId}, self assessment id: {selfAssessmentId}, candidate id: {candidateId}, " +
                   $"assessment question id: {assessmentQuestionId}, result: {result}";
        }
        public Profile GetFilteredProfileForCandidateById(int selfAssessmentId, int candidateId)
        {
            return connection.QueryFirstOrDefault<Profile>("GetFilteredProfileForCandidate", new { selfAssessmentId, candidateId }, commandType: CommandType.StoredProcedure);
        }
        public IEnumerable<Goal> GetFilteredGoalsForCandidateId(int selfAssessmentId, int candidateId)
        {
            return connection.Query<Goal>("GetFilteredCompetencyResponsesForCandidate", new { selfAssessmentId, candidateId }, commandType: CommandType.StoredProcedure);
        }

        public void SetUpdatedFlag(int selfAssessmentId, int candidateId, bool status)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE CandidateAssessments
                        SET UnprocessedUpdates = @status
                        WHERE SelfAssessmentID = @selfAssessmentId
                          AND CandidateID = @candidateId",
                new { status, selfAssessmentId, candidateId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not setting self assessment updated flag as db update failed. " +
                    $"Self assessment id: {selfAssessmentId}, candidate id: {candidateId}, status: {status}"
                );
            }
        }

        public void SetBookmark(int selfAssessmentId, int candidateId, string bookmark)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE CandidateAssessments
                        SET UserBookmark = @bookmark
                        WHERE SelfAssessmentID = @selfAssessmentId
                          AND CandidateID = @candidateId",
                new { bookmark, selfAssessmentId, candidateId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not setting self assessment bookmark as db update failed. " +
                    $"Self assessment id: {selfAssessmentId}, candidate id: {candidateId}, bookmark: {bookmark}"
                );
            }
        }

        public void IncrementLaunchCount(int selfAssessmentId, int candidateId)
        {
            var numberOfAffectedRows = connection.Execute(
               @"UPDATE CandidateAssessments SET LaunchCount = LaunchCount+1
                      WHERE SelfAssessmentID = @selfAssessmentId AND CandidateID = @candidateId",
               new { selfAssessmentId, candidateId }
           );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating self assessment launch count as db update failed. " +
                    $"Self assessment id: {selfAssessmentId}, candidate id: {candidateId}"
                );
            }
        }

        public void LogAssetLaunch(int candidateId, int selfAssessmentId, LearningAsset learningAsset)
        {
            connection.Execute("UpdateFilteredLearningActivity",
                new
                {
                    FilteredAssetID = learningAsset.Id,
                    Title = learningAsset.Title,
                    Description = learningAsset.Description,
                    DirectUrl = learningAsset.DirectUrl,
                    Type = learningAsset.TypeLabel,
                    Provider = learningAsset.Provider.Name,
                    Duration = learningAsset.LengthSeconds,
                    ActualDuration = learningAsset.LengthSeconds,
                    CandidateId = candidateId,
                    SelfAssessmentID = selfAssessmentId,
                    Completed = learningAsset.Completed,
                    Outcome = learningAsset.CompletedStatus,
                    Bookmark = learningAsset.IsFavourite
                }, commandType: CommandType.StoredProcedure);
        }

        public void SetSubmittedDateNow(int selfAssessmentId, int candidateId)
        {
            var numberOfAffectedRows = connection.Execute(
               @"UPDATE CandidateAssessments SET SubmittedDate = GETUTCDATE()
                      WHERE SelfAssessmentID = @selfAssessmentId AND CandidateID = @candidateId AND SubmittedDate IS NULL",
               new { selfAssessmentId, candidateId }
           );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not setting self assessment submitted date as db update failed. " +
                    $"Self assessment id: {selfAssessmentId}, candidate id: {candidateId}"
                );
            }
        }

        public IEnumerable<LevelDescriptor> GetLevelDescriptorsForAssessmentQuestion(int assessmentQuestionId, int minValue, int maxValue, bool zeroBased)
        {
            int adjustBy = zeroBased ? 1 : 0;
            return connection.Query<LevelDescriptor>(
               @"SELECT COALESCE(ID,0) AS ID, @assessmentQuestionId AS AssessmentQuestionID, n AS LevelValue, LevelLabel,
                    LevelDescription, 0 AS UpdatedByAdminID
                    FROM
                    (SELECT TOP (@maxValue + @adjustBy) n = ROW_NUMBER() OVER (ORDER BY number) - @adjustBy
                    FROM [master]..spt_values) AS q1
                    LEFT OUTER JOIN AssessmentQuestionLevels AS AQL ON q1.n = AQL.LevelValue AND AQL.AssessmentQuestionID = @assessmentQuestionId
                    WHERE (q1.n BETWEEN @minValue AND @maxValue)", new { assessmentQuestionId, minValue, maxValue, adjustBy }
               );
        }
        public Competency? GetCompetencyByCandidateAssessmentResultId(int resultId, int candidateAssessmentId, int adminId)
        {
            Competency? competencyResult = null;
            return connection.Query<Competency, Models.SelfAssessments.AssessmentQuestion, Competency>(
                $@"WITH {SpecificAssessmentResults}
                    SELECT {CompetencyFields}
                    FROM {SpecificCompetencyTables}
                    WHERE ResultId = @resultId",
                (competency, assessmentQuestion) =>
                {
                    if (competencyResult == null)
                    {
                        competencyResult = competency;
                    }

                    competencyResult.AssessmentQuestions.Add(assessmentQuestion);
                    return competencyResult;
                },
                param: new { resultId, candidateAssessmentId, adminId }
            ).FirstOrDefault();
        }

        public SelfAssessmentSupervisor GetSupervisorForSelfAssessmentId(int selfAssessmentId, int candidateId)
        {
            var supervisorDetails = connection.Query<SelfAssessmentSupervisor>(
                  @"SELECT sd.ID, sd.ID AS SupervisorDelegateID, sd.SupervisorAdminID, sd.SupervisorEmail, sd.NotificationSent,
                   au.Forename + ' ' + au.Surname AS SupervisorName, COALESCE(sasr.RoleName, 'Supervisor') AS RoleName,
                   sasr.SelfAssessmentReview, sasr.ResultsReview, sd.AddedByDelegate, sd.Confirmed
                   FROM   SupervisorDelegates AS sd INNER JOIN
                   CandidateAssessmentSupervisors AS cas ON sd.ID = cas.SupervisorDelegateId INNER JOIN
                   CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                   AdminUsers AS au ON sd.SupervisorAdminID = au.AdminID LEFT OUTER JOIN
                   SelfAssessmentSupervisorRoles AS sasr ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
                   WHERE (sd.Removed IS NULL) AND (sd.Confirmed IS NOT NULL) AND (sd.CandidateID = @candidateId)
                   AND (ca.SelfAssessmentID = @selfAssessmentId)",
                   new { selfAssessmentId, candidateId }
                  ).FirstOrDefault();

            return supervisorDetails;
        }

        public IEnumerable<SelfAssessmentSupervisor> GetSupervisorsForSelfAssessmentId(int selfAssessmentId, int candidateId)
        {
            return connection.Query<SelfAssessmentSupervisor>(
                  @"SELECT sd.ID, sd.ID AS SupervisorDelegateID, sd.SupervisorAdminID, sd.SupervisorEmail, sd.NotificationSent, au.Forename + ' ' + au.Surname AS SupervisorName, COALESCE(sasr.RoleName, 'Supervisor') AS RoleName, sasr.SelfAssessmentReview, sasr.ResultsReview, sd.AddedByDelegate, sd.Confirmed
FROM   SupervisorDelegates AS sd INNER JOIN
             CandidateAssessmentSupervisors AS cas ON sd.ID = cas.SupervisorDelegateId INNER JOIN
             CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
             AdminUsers AS au ON sd.SupervisorAdminID = au.AdminID LEFT OUTER JOIN
             SelfAssessmentSupervisorRoles AS sasr ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
WHERE (sd.Removed IS NULL) AND (sd.Confirmed IS NOT NULL) AND (sd.CandidateID = @candidateId) AND (ca.SelfAssessmentID = @selfAssessmentId)", new { selfAssessmentId, candidateId }
                  );
        }
        public IEnumerable<SelfAssessmentSupervisor> GetAllSupervisorsForSelfAssessmentId(int selfAssessmentId, int candidateId)
        {
            return connection.Query<SelfAssessmentSupervisor>(
                  @"SELECT cas.ID, sd.ID AS SupervisorDelegateID, sd.SupervisorAdminID, sd.SupervisorEmail, sd.NotificationSent, COALESCE(au.Forename + ' ' + au.Surname, sd.SupervisorEmail) AS SupervisorName, COALESCE(sasr.RoleName, 'Supervisor') AS RoleName, sasr.SelfAssessmentReview, sasr.ResultsReview, sd.AddedByDelegate, sd.Confirmed
FROM   SupervisorDelegates AS sd INNER JOIN
             CandidateAssessmentSupervisors AS cas ON sd.ID = cas.SupervisorDelegateId INNER JOIN
             CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID LEFT OUTER JOIN
             AdminUsers AS au ON sd.SupervisorAdminID = au.AdminID LEFT OUTER JOIN
             SelfAssessmentSupervisorRoles AS sasr ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
WHERE (sd.Removed IS NULL) AND (sd.CandidateID = @candidateId) AND (ca.SelfAssessmentID = @selfAssessmentId)", new { selfAssessmentId, candidateId }
                  );
        }
        public IEnumerable<SelfAssessmentSupervisor> GetResultReviewSupervisorsForSelfAssessmentId(int selfAssessmentId, int candidateId)
        {
            return connection.Query<SelfAssessmentSupervisor>(
                  @"SELECT cas.ID, sd.ID AS SupervisorDelegateID, sd.SupervisorAdminID, sd.SupervisorEmail, sd.NotificationSent, COALESCE(au.Forename + ' ' + au.Surname, sd.SupervisorEmail) AS SupervisorName, COALESCE(sasr.RoleName, 'Supervisor') AS RoleName, sasr.SelfAssessmentReview, sasr.ResultsReview, sd.AddedByDelegate, sd.Confirmed
FROM   SupervisorDelegates AS sd INNER JOIN
             CandidateAssessmentSupervisors AS cas ON sd.ID = cas.SupervisorDelegateId INNER JOIN
             CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID LEFT OUTER JOIN
             AdminUsers AS au ON sd.SupervisorAdminID = au.AdminID LEFT OUTER JOIN
             SelfAssessmentSupervisorRoles AS sasr ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
WHERE (sd.Removed IS NULL) AND (sd.Confirmed IS NOT NULL) AND (sd.CandidateID = @candidateId) AND (ca.SelfAssessmentID = @selfAssessmentId) AND (sd.SupervisorAdminID IS NOT NULL) AND (coalesce(sasr.ResultsReview, 1) = 1)", new { selfAssessmentId, candidateId }
                  );
        }
        public IEnumerable<SelfAssessmentSupervisor> GetOtherSupervisorsForCandidate(int selfAssessmentId, int candidateId)
        {
            return connection.Query<SelfAssessmentSupervisor>(
                  @"SELECT 0 AS ID, sd.ID AS SupervisorDelegateID, sd.SupervisorAdminID, sd.SupervisorEmail, sd.NotificationSent, au.Forename + ' ' + au.Surname AS SupervisorName, 'Supervisor' AS RoleName
FROM   SupervisorDelegates AS sd INNER JOIN
             CandidateAssessmentSupervisors AS cas ON sd.ID = cas.SupervisorDelegateId INNER JOIN
             CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
             AdminUsers AS au ON sd.SupervisorAdminID = au.AdminID
WHERE (sd.Removed IS NULL) AND (sd.SupervisorAdminID IS NOT NULL) AND (sd.Confirmed IS NOT NULL) AND (sd.CandidateID = @candidateId)
EXCEPT
SELECT 0 AS ID, sd.ID AS SupervisorDelegateID, sd.SupervisorAdminID, sd.SupervisorEmail, sd.NotificationSent, au.Forename + ' ' + au.Surname AS SupervisorName, 'Supervisor' AS RoleName
FROM   SupervisorDelegates AS sd INNER JOIN
             CandidateAssessmentSupervisors AS cas ON sd.ID = cas.SupervisorDelegateId INNER JOIN
             CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
             AdminUsers AS au ON sd.SupervisorAdminID = au.AdminID
WHERE (sd.Removed IS NULL) AND (sd.Confirmed IS NOT NULL) AND (sd.CandidateID = @candidateId) AND (ca.SelfAssessmentID = @selfAssessmentId)
GROUP BY sd.ID, SupervisorAdminID, SupervisorEmail, sd.NotificationSent, au.Forename + ' ' + au.Surname", new { selfAssessmentId, candidateId }
                  );
        }

        public SelfAssessmentSupervisor GetSelfAssessmentSupervisorByCandidateAssessmentSupervisorId(int candidateAssessmentSupervisorId)
        {
            return connection.Query<SelfAssessmentSupervisor>(@"SELECT cas.ID, sd.ID AS SupervisorDelegateID, sd.SupervisorAdminID, sd.SupervisorEmail, sd.NotificationSent, COALESCE(au.Forename + ' ' + au.Surname, sd.SupervisorEmail) AS SupervisorName, COALESCE(sasr.RoleName, 'Supervisor') AS RoleName, sasr.SelfAssessmentReview, sasr.ResultsReview, sd.AddedByDelegate, sd.Confirmed
FROM   SupervisorDelegates AS sd INNER JOIN
             CandidateAssessmentSupervisors AS cas ON sd.ID = cas.SupervisorDelegateId INNER JOIN
             CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID LEFT OUTER JOIN
             AdminUsers AS au ON sd.SupervisorAdminID = au.AdminID LEFT OUTER JOIN
             SelfAssessmentSupervisorRoles AS sasr ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
WHERE cas.ID = @candidateAssessmentSupervisorId", new { candidateAssessmentSupervisorId }).FirstOrDefault();
        }

        public IEnumerable<Competency> GetCandidateAssessmentOptionalCompetencies(int selfAssessmentId, int candidateId)
        {
            return connection.Query<Competency>(
                @"SELECT SAS.ID       AS Id,
                                                  ROW_NUMBER() OVER (ORDER BY SAS.Ordering) as RowNo,
                                                  C.Name AS Name,
                                                  C.Description AS Description,
                                                  CG.Name       AS CompetencyGroup,
                                                  CG.ID AS CompetencyGroupID,
                                                  'Capability' AS Vocabulary,
                                                  SAS.Optional,
                                                  COALESCE (CAOC.IncludedInSelfAssessment, 0) AS IncludedInSelfAssessment
                    FROM Competencies AS C
                        INNER JOIN CandidateAssessments AS CA
                            ON CA.SelfAssessmentID = @selfAssessmentId
                                   AND CA.CandidateID = @candidateId AND CA.RemovedDate IS NULL
                        INNER JOIN SelfAssessmentStructure AS SAS
                            ON C.ID = SAS.CompetencyID
                                    AND SAS.SelfAssessmentID = @selfAssessmentId
                        INNER JOIN CompetencyGroups AS CG
                            ON SAS.CompetencyGroupID = CG.ID
                                    AND SAS.SelfAssessmentID = @selfAssessmentId
                        LEFT OUTER JOIN CandidateAssessmentOptionalCompetencies AS CAOC
                            ON CA.ID = CAOC.CandidateAssessmentID AND C.ID = CAOC.CompetencyID AND CG.ID = CAOC.CompetencyGroupID
                    WHERE (SAS.Optional = 1)
                    ORDER BY SAS.Ordering", new { selfAssessmentId, candidateId }
            );
        }

        public List<int> GetCandidateAssessmentIncludedSelfAssessmentStructureIds(int selfAssessmentId, int candidateId)
        {
            return connection.Query<int>(
                @"SELECT SAS.ID
                    FROM CandidateAssessmentOptionalCompetencies AS CAOC
                    INNER JOIN CandidateAssessments  AS CA ON CAOC.CandidateAssessmentID = CA.ID
                            AND CA.SelfAssessmentID = @selfAssessmentId
                                   AND CA.CandidateID = @candidateId AND CA.RemovedDate IS NULL
                    INNER JOIN SelfAssessmentStructure AS SAS
                            ON CAOC.CompetencyID = SAS.CompetencyID AND CAOC.CompetencyGroupID = SAS.CompetencyGroupID AND SAS.SelfAssessmentID = @selfAssessmentId
                    WHERE (CAOC.IncludedInSelfAssessment = 1)", new { selfAssessmentId, candidateId }
            ).ToList();
        }

        public void InsertCandidateAssessmentOptionalCompetenciesIfNotExist(int selfAssessmentId, int candidateId)
        {
            connection.Execute(
                               @"UPDATE CandidateAssessmentOptionalCompetencies
                                   SET IncludedInSelfAssessment = 0
                    FROM   CandidateAssessmentOptionalCompetencies AS CAOC INNER JOIN
             CandidateAssessments AS CA ON CAOC.CandidateAssessmentID = CA.ID INNER JOIN
             SelfAssessmentStructure AS SAS ON CA.SelfAssessmentID = SAS.SelfAssessmentID AND CAOC.CompetencyID = SAS.CompetencyID AND CAOC.CompetencyGroupID = SAS.CompetencyGroupID
WHERE (CA.CandidateID = @candidateId) AND (CA.RemovedDate IS NULL)
                                    ", new { selfAssessmentId, candidateId });
            connection.Execute(
                @"INSERT INTO CandidateAssessmentOptionalCompetencies (CandidateAssessmentId, CompetencyID, CompetencyGroupID)
                    SELECT CA.ID, SAS.CompetencyID, SAS.CompetencyGroupID
                    FROM SelfAssessmentStructure AS SAS
                    INNER JOIN CandidateAssessments  AS CA ON SAS.SelfAssessmentID = CA.SelfAssessmentID
                            AND CA.SelfAssessmentID = @selfAssessmentId
                                   AND CA.CandidateID = @candidateId AND CA.RemovedDate IS NULL AND SAS.Optional = 1
								   WHERE NOT EXISTS (SELECT * FROM CandidateAssessmentOptionalCompetencies WHERE CandidateAssessmentID = CA.ID AND CompetencyID = SAS.CompetencyID AND CompetencyGroupID = SAS.CompetencyGroupID)",
                new { selfAssessmentId, candidateId });
        }

        public void UpdateCandidateAssessmentOptionalCompetencies(int selfAssessmentStructureId, int candidateId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE CandidateAssessmentOptionalCompetencies
                    SET IncludedInSelfAssessment = 1
                    FROM   CandidateAssessmentOptionalCompetencies AS CAOC INNER JOIN
             CandidateAssessments AS CA ON CAOC.CandidateAssessmentID = CA.ID INNER JOIN
             SelfAssessmentStructure AS SAS ON CA.SelfAssessmentID = SAS.SelfAssessmentID AND CAOC.CompetencyID = SAS.CompetencyID AND CAOC.CompetencyGroupID = SAS.CompetencyGroupID
WHERE (SAS.ID = @selfAssessmentStructureId) AND (CA.CandidateID = @candidateId) AND (CA.RemovedDate IS NULL)", new { selfAssessmentStructureId, candidateId }
                );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not setting CandidateAssessmentOptionalCompetencies include state as db update failed. " +
                    $"Self assessment id: {selfAssessmentStructureId}, candidate id: {candidateId} "
                );
            }
        }

        public IEnumerable<Administrator> GetValidSupervisorsForActivity(int centreId, int selfAssessmentId, int candidateId)
        {
            return connection.Query<Administrator>(
                @"SELECT AdminID, Forename, Surname, Email, ProfileImage, IsFrameworkDeveloper
                    FROM   AdminUsers
                    WHERE ((Supervisor = 1) AND (Active = 1) AND (CategoryID = 0) AND (CentreID = @centreId) OR
                       (Supervisor = 1) AND (Active = 1) AND (CategoryID =
                   (SELECT CategoryID
                     FROM    SelfAssessments
                      WHERE (ID = @selfAssessmentId))) AND (CentreID = @centreId)) AND AdminID NOT IN (SELECT sd.SupervisorAdminID
FROM   CandidateAssessmentSupervisors AS cas INNER JOIN
             SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID INNER JOIN
             CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID
WHERE (ca.SelfAssessmentID = @selfAssessmentId) AND (ca.CandidateID = @candidateId) AND (sd.SupervisorAdminID = AdminUsers.AdminID)) ",
                new { centreId, selfAssessmentId, candidateId }
                );
        }

        public Administrator GetSupervisorByAdminId(int supervisorAdminId)
        {
            return connection.Query<Administrator>(
                @"SELECT AdminID, Forename, Surname, Email, ProfileImage, IsFrameworkDeveloper
                    FROM   AdminUsers
                    WHERE (AdminID = @supervisorAdminId)", new { supervisorAdminId }
                ).Single();
        }

        public IEnumerable<SupervisorSignOff>? GetSupervisorSignOffsForCandidateAssessment(int selfAssessmentId, int candidateId)
        {
            return connection.Query<SupervisorSignOff>(
                @"SELECT casv.ID, casv.CandidateAssessmentSupervisorID, au.Forename + ' ' + au.Surname AS SupervisorName, au.Email AS SupervisorEmail, COALESCE(sasr.Rolename, 'Supervisor') AS SupervisorRoleName, casv.Requested, casv.EmailSent, casv.Verified, casv.Comments, casv.SignedOff
                    FROM   CandidateAssessmentSupervisorVerifications AS casv INNER JOIN
                         CandidateAssessmentSupervisors AS cas ON casv.CandidateAssessmentSupervisorID = cas.ID INNER JOIN
                         CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                        SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID INNER JOIN
                         AdminUsers AS au ON sd.SupervisorAdminID = au.AdminID LEFT OUTER JOIN
                         SelfAssessmentSupervisorRoles AS sasr ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
                    WHERE (ca.CandidateID = @candidateId) AND (ca.SelfAssessmentID = @selfAssessmentId) AND (sasr.SelfAssessmentReview = 1) OR
                         (ca.CandidateID = @candidateId) AND (ca.SelfAssessmentID = @selfAssessmentId) AND (sasr.SelfAssessmentReview IS NULL)
                    ORDER BY casv.Requested DESC", new { selfAssessmentId, candidateId });
        }

        public IEnumerable<SelfAssessmentSupervisor> GetSignOffSupervisorsForSelfAssessmentId(int selfAssessmentId, int candidateId)
        {
            return connection.Query<SelfAssessmentSupervisor>(
                  @"SELECT cas.ID, sd.ID AS SupervisorDelegateID, sd.SupervisorAdminID, sd.SupervisorEmail, sd.NotificationSent, COALESCE(au.Forename + ' ' + au.Surname, sd.SupervisorEmail) AS SupervisorName, COALESCE(sasr.RoleName, 'Supervisor') AS RoleName, sasr.SelfAssessmentReview, sasr.ResultsReview, sd.AddedByDelegate, sd.Confirmed
FROM   SupervisorDelegates AS sd INNER JOIN
             CandidateAssessmentSupervisors AS cas ON sd.ID = cas.SupervisorDelegateId INNER JOIN
             CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID LEFT OUTER JOIN
             AdminUsers AS au ON sd.SupervisorAdminID = au.AdminID LEFT OUTER JOIN
             SelfAssessmentSupervisorRoles AS sasr ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
WHERE (sd.Removed IS NULL) AND (sd.Confirmed IS NOT NULL) AND (sd.CandidateID = @candidateId) AND (ca.SelfAssessmentID = @selfAssessmentId) AND (sd.SupervisorAdminID IS NOT NULL) AND (coalesce(sasr.SelfAssessmentReview, 1) = 1) AND (cas.ID NOT IN (SELECT CandidateAssessmentSupervisorID FROM CandidateAssessmentSupervisorVerifications WHERE Verified IS NULL))", new { selfAssessmentId, candidateId }
                  );
        }

        public void InsertCandidateAssessmentSupervisorVerification(int candidateAssessmentSupervisorId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"INSERT INTO CandidateAssessmentSupervisorVerifications
                          ([CandidateAssessmentSupervisorID],[EmailSent])
                    VALUES(@candidateAssessmentSupervisorId, GETUTCDATE())",
                new { candidateAssessmentSupervisorId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not inserting CandidateAssessmentSupervisorVerification as db update failed. " +
                    $"candidateAssessmentSupervisorId: {candidateAssessmentSupervisorId}"
                );
            }
        }

        public void UpdateCandidateAssessmentSupervisorVerificationEmailSent(int candidateAssessmentSupervisorVerificationId)
        {
            connection.Execute(
                @"UPDATE CandidateAssessmentSupervisorVerifications
                    SET EmailSent = GETUTCDATE()
                    WHERE ID = @candidateAssessmentSupervisorVerificationId",
                new { candidateAssessmentSupervisorVerificationId }
            );
        }
    }
}
