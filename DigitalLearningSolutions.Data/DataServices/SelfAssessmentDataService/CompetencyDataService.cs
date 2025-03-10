﻿namespace DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using Microsoft.Extensions.Logging;
    using AssessmentQuestion = DigitalLearningSolutions.Data.Models.SelfAssessments.AssessmentQuestion;

    public partial class SelfAssessmentDataService
    {
        private const string LatestAssessmentResults =
            @"LatestAssessmentResults AS
            (
                SELECT
                    s.CompetencyID,
                    s.AssessmentQuestionID,
                    s.ID AS ResultID,
                    s.DateTime AS ResultDateTime,
                    s.Result,
                    s.SupportingComments,
                    sv.ID AS SelfAssessmentResultSupervisorVerificationId,
                    sv.Requested,
                    sv.Verified,
                    sv.Comments,
                    sv.SignedOff,
                    adu.Forename + ' ' + adu.Surname AS SupervisorName,
                    sv.CandidateAssessmentSupervisorID,
                    sv.EmailSent,
                    0 AS UserIsVerifier,
                    COALESCE (rr.LevelRAG, 0) AS ResultRAG
                FROM SelfAssessmentResults s
                INNER JOIN SelfAssessmentStructure AS sas ON s.CompetencyID = sas.CompetencyID AND sas.SelfAssessmentID = @selfAssessmentId
                LEFT OUTER JOIN DelegateAccounts AS da ON s.DelegateUserID = da.UserID
                LEFT OUTER JOIN SelfAssessmentResultSupervisorVerifications AS sv
                    ON s.ID = sv.SelfAssessmentResultId AND sv.Superceded = 0
                LEFT OUTER JOIN CandidateAssessmentSupervisors AS cas 
                    ON sv.CandidateAssessmentSupervisorID = cas.ID
                LEFT OUTER JOIN SupervisorDelegates AS sd
                    ON cas.SupervisorDelegateId = sd.ID
                LEFT OUTER JOIN AdminUsers AS adu
					ON sd.SupervisorAdminID = adu.AdminID
                LEFT OUTER JOIN CompetencyAssessmentQuestionRoleRequirements rr
                    ON s.CompetencyID = rr.CompetencyID AND s.AssessmentQuestionID = rr.AssessmentQuestionID
                        AND rr.SelfAssessmentID = @selfAssessmentId AND s.Result = rr.LevelValue
                WHERE da.ID = @delegateId
            )";

        private const string SpecificAssessmentResults =
            @"LatestAssessmentResults AS
            (
                SELECT
                    s.CompetencyID,
                    s.AssessmentQuestionID,
                    s.ID AS ResultID,
                    s.Result,
                    s.DateTime AS ResultDateTime,
                    s.SupportingComments,
                    sv.ID AS SelfAssessmentResultSupervisorVerificationId,
                    sv.Requested,
                    sv.Verified,
                    sv.Comments,
                    sv.SignedOff,
                    adu.Forename + ' ' + adu.Surname AS SupervisorName,
                    CAST(CASE WHEN COALESCE(sd.SupervisorAdminID, 0) = @adminId THEN 1 ELSE 0 END AS Bit) AS UserIsVerifier,
                    COALESCE (rr.LevelRAG, 0) AS ResultRAG
                FROM CandidateAssessments ca
                INNER JOIN SelfAssessmentResults s
                    ON s.DelegateUserID = ca.DelegateUserID
                INNER JOIN SelfAssessmentStructure AS sas ON s.CompetencyID = sas.CompetencyID AND sas.SelfAssessmentID = ca.SelfAssessmentID
                LEFT OUTER JOIN SelfAssessmentResultSupervisorVerifications AS sv
                    ON s.ID = sv.SelfAssessmentResultId AND sv.Superceded = 0
                LEFT OUTER JOIN CandidateAssessmentSupervisors AS cas 
                    ON sv.CandidateAssessmentSupervisorID = cas.ID AND cas.Removed IS NULL
                LEFT OUTER JOIN SupervisorDelegates AS sd
                    ON cas.SupervisorDelegateId = sd.ID
                LEFT OUTER JOIN AdminUsers AS adu
						     ON sd.SupervisorAdminID = adu.AdminID
                LEFT OUTER JOIN CompetencyAssessmentQuestionRoleRequirements rr
                    ON s.CompetencyID = rr.CompetencyID AND s.AssessmentQuestionID = rr.AssessmentQuestionID
                        AND rr.SelfAssessmentID = ca.SelfAssessmentID AND s.Result = rr.LevelValue
                WHERE ca.ID = @candidateAssessmentId
            )";

        private const string CompetencyFields =
            @"C.ID AS Id,
            DENSE_RANK() OVER (ORDER BY SAS.Ordering) as RowNo,
            C.Name AS Name,
            C.Description AS Description,
            CG.Name AS CompetencyGroup,
            CG.ID AS CompetencyGroupID,
            CG.Description AS CompetencyGroupDescription,
            COALESCE(
                (SELECT TOP(1) FrameworkConfig
                FROM Frameworks F
                INNER JOIN FrameworkCompetencies AS FC
                    ON FC.FrameworkID = F.ID
                WHERE FC.CompetencyID = C.ID),
            'Capability') AS Vocabulary,
            CASE
                WHEN (SELECT COUNT(*) FROM SelfAssessmentSupervisorRoles WHERE SelfAssessmentID = SAS.SelfAssessmentID) > 0
                THEN 1
                ELSE 0
            END AS HasDelegateNominatedRoles,
            SAS.Optional,
            C.AlwaysShowDescription,
            AQ.ID AS Id,
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
            LAR.ResultDateTime,
            LAR.SupportingComments,
            LAR.SelfAssessmentResultSupervisorVerificationId,
            LAR.Requested,
            LAR.Verified,
            LAR.Comments AS SupervisorComments,
            LAR.SignedOff,
            LAR.UserIsVerifier,
            LAR.ResultRAG,
            LAR.SupervisorName";

        private const string CompetencyTables =
            @"Competencies AS C
            INNER JOIN CompetencyAssessmentQuestions AS CAQ
                ON CAQ.CompetencyID = C.ID
            INNER JOIN AssessmentQuestions AS AQ
                ON AQ.ID = CAQ.AssessmentQuestionID
            INNER JOIN CandidateAssessments AS CA
                ON CA.SelfAssessmentID = @selfAssessmentId AND CA.RemovedDate IS NULL
			INNER JOIN DelegateAccounts AS DA ON CA.DelegateUserID = DA.UserID AND DA.ID = @delegateId
            LEFT OUTER JOIN LatestAssessmentResults AS LAR
                ON LAR.CompetencyID = C.ID AND LAR.AssessmentQuestionID = AQ.ID
            INNER JOIN SelfAssessmentStructure AS SAS
                ON C.ID = SAS.CompetencyID AND SAS.SelfAssessmentID = @selfAssessmentId
            INNER JOIN CompetencyGroups AS CG
                ON SAS.CompetencyGroupID = CG.ID AND SAS.SelfAssessmentID = @selfAssessmentId
            LEFT OUTER JOIN CandidateAssessmentOptionalCompetencies AS CAOC
                ON CA.ID = CAOC.CandidateAssessmentID AND C.ID = CAOC.CompetencyID AND CG.ID = CAOC.CompetencyGroupID";

        private const string SpecificCompetencyTables =
            @"Competencies AS C
            INNER JOIN CompetencyAssessmentQuestions AS CAQ
                ON CAQ.CompetencyID = C.ID
            INNER JOIN AssessmentQuestions AS AQ
                ON AQ.ID = CAQ.AssessmentQuestionID
            INNER JOIN CandidateAssessments AS CA
                ON CA.ID = @candidateAssessmentId
            LEFT OUTER JOIN LatestAssessmentResults AS LAR
                ON LAR.CompetencyID = C.ID AND LAR.AssessmentQuestionID = AQ.ID
            INNER JOIN SelfAssessmentStructure AS SAS
                ON C.ID = SAS.CompetencyID AND SAS.SelfAssessmentID = CA.SelfAssessmentID
            INNER JOIN CompetencyGroups AS CG
                ON SAS.CompetencyGroupID = CG.ID AND SAS.SelfAssessmentID = CA.SelfAssessmentID
            LEFT OUTER JOIN CandidateAssessmentOptionalCompetencies AS CAOC
                ON CA.ID = CAOC.CandidateAssessmentID AND C.ID = CAOC.CompetencyID AND CG.ID = CAOC.CompetencyGroupID";

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

        public Competency? GetNthCompetency(int n, int selfAssessmentId, int delegateId)
        {
            Competency? competencyResult = null;
            return connection.Query<Competency, AssessmentQuestion, Competency>(
                $@"WITH CompetencyRowNumber AS
                    (
                        SELECT
                            DENSE_RANK() OVER (ORDER BY SAS.Ordering) as RowNo,
                            sas.CompetencyID
                        FROM            SelfAssessmentStructure AS sas
                                        INNER JOIN CandidateAssessments AS CA ON CA.SelfAssessmentID = @selfAssessmentId AND CA.RemovedDate IS NULL
			                            INNER JOIN DelegateAccounts AS DA ON CA.DelegateUserID = DA.UserID AND DA.ID = @delegateId
                                        INNER JOIN CompetencyAssessmentQuestions AS caq ON sas.CompetencyID = caq.CompetencyID 
                                        LEFT OUTER JOIN CandidateAssessmentOptionalCompetencies AS CAOC ON CA.ID = CAOC.CandidateAssessmentID AND sas.CompetencyID = CAOC.CompetencyID AND 
                                         sas.CompetencyGroupID = CAOC.CompetencyGroupID
                        WHERE        (sas.SelfAssessmentID = @selfAssessmentId) AND (sas.Optional = 0) OR
                         (sas.SelfAssessmentID = @selfAssessmentId) AND (CAOC.IncludedInSelfAssessment = 1)
						 GROUP BY sas.CompetencyID, SAS.Ordering
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
                    competencyResult ??= competency;
                    competencyResult.AssessmentQuestions.Add(assessmentQuestion);
                    return competencyResult;
                },
                new { n, selfAssessmentId, delegateId }
            ).FirstOrDefault();
        }

        public IEnumerable<Competency> GetMostRecentResults(int selfAssessmentId, int delegateId)
        {
            var result = connection.Query<Competency, AssessmentQuestion, Competency>(
                $@"GetAssessmentResultsByDelegate",
                (competency, assessmentQuestion) =>
                {
                    competency.AssessmentQuestions.Add(assessmentQuestion);
                    return competency;
                },
                new { selfAssessmentId, delegateId },
                commandType: CommandType.StoredProcedure
            );
            return GroupCompetencyAssessmentQuestions(result);
        }

        public IEnumerable<Competency> GetCandidateAssessmentResultsById(int candidateAssessmentId, int adminId, int? selfAssessmentResultId = null)
        {
            var result = connection.Query<Competency, AssessmentQuestion, Competency>(
                $@"GetCandidateAssessmentResultsById",
                (competency, assessmentQuestion) =>
                {
                    competency.AssessmentQuestions.Add(assessmentQuestion);
                    return competency;
                },
                new { candidateAssessmentId, adminId, selfAssessmentResultId },
                commandType: CommandType.StoredProcedure
            );
            return GroupCompetencyAssessmentQuestions(result);
        }

        public IEnumerable<Competency> GetCandidateAssessmentResultsForReviewById(
            int candidateAssessmentId,
            int adminId
        )
        {
            var result = connection.Query<Competency, AssessmentQuestion, Competency>(
                $@"WITH {SpecificAssessmentResults}
                    SELECT {CompetencyFields}
                    FROM {SpecificCompetencyTables}
                    WHERE (LAR.Requested IS NOT NULL) AND (LAR.Verified IS NULL) AND (LAR.UserIsVerifier = 1)
                        AND ((CAOC.IncludedInSelfAssessment = 1) OR (SAS.Optional = 0))
                    ORDER BY SAS.Ordering, CAQ.Ordering",
                (competency, assessmentQuestion) =>
                {
                    competency.AssessmentQuestions.Add(assessmentQuestion);
                    return competency;
                },
                new { candidateAssessmentId, adminId }
            );
            return GroupCompetencyAssessmentQuestions(result);
        }

        public IEnumerable<Competency> GetResultSupervisorVerifications(int selfAssessmentId, int delegateId)
        {
            const string supervisorFields = @"
                LAR.EmailSent,
                LAR.Requested AS SupervisorVerificationRequested,
                au.Forename + ' ' + au.Surname + (CASE WHEN au.Active = 1 THEN '' ELSE ' (Inactive)' END) + ' (' + sd.SupervisorEmail + ')' AS SupervisorName,
                au.CentreName,
                SelfAssessmentResultSupervisorVerificationId AS SupervisorVerificationId,
                CandidateAssessmentSupervisorID";
            const string supervisorTables = @"
                LEFT OUTER JOIN CandidateAssessmentSupervisors AS cas ON cas.ID = CandidateAssessmentSupervisorID AND cas.Removed IS NULL
                LEFT OUTER JOIN SupervisorDelegates AS sd ON sd.ID = cas.SupervisorDelegateId AND sd.Removed IS NULL
                LEFT OUTER JOIN AdminUsers AS au ON au.AdminID = sd.SupervisorAdminID";

            var result = connection.Query<Competency, AssessmentQuestion, Competency>(
                $@"WITH {LatestAssessmentResults}
                    SELECT {supervisorFields}, {CompetencyFields}
                    FROM {CompetencyTables}
                        INNER JOIN SelfAssessments AS SA ON CA.SelfAssessmentID = SA.ID
                        {supervisorTables}
                        INNER JOIN DelegateAccounts DA1 ON CA.DelegateUserID = DA1.UserID AND au.CentreID = DA1.CentreID AND DA1.Active=1
                    WHERE (LAR.Verified IS NULL) 
	                    AND ((LAR.Result IS NOT NULL) OR (LAR.SupportingComments IS NOT NULL)) 
	                    AND (LAR.Requested IS NOT NULL) AND (au.CategoryID = 0 OR au.CategoryID = (select CategoryID from SelfAssessments where ID = @selfAssessmentId))
                    ORDER BY SupervisorVerificationRequested DESC, C.Name",
                (competency, assessmentQuestion) =>
                {
                    competency.AssessmentQuestions.Add(assessmentQuestion);
                    return competency;
                },
                new { selfAssessmentId, delegateId }
            );
            return result;
        }

        public IEnumerable<Competency> GetCandidateAssessmentResultsToVerifyById(int selfAssessmentId, int delegateId)
        {
            var result = connection.Query<Competency, AssessmentQuestion, Competency>(
                $@"WITH {LatestAssessmentResults}
                    SELECT {CompetencyFields}
                    FROM {CompetencyTables} INNER JOIN SelfAssessments AS SA ON CA.SelfAssessmentID = SA.ID
                    WHERE ((LAR.Requested IS NULL) OR (LAR.Requested < DATEADD(week, -1, getUTCDate()))) AND (LAR.Verified IS NULL) AND ((LAR.Result IS NOT NULL)
                        OR (LAR.SupportingComments IS NOT NULL)) AND ((CAOC.IncludedInSelfAssessment = 1) OR (SAS.Optional = 0)) AND ((SA.EnforceRoleRequirementsForSignOff = 0) OR (CAQ.Required = 0))
						OR ((LAR.Requested IS NULL) OR (LAR.Requested < DATEADD(week, -1, getUTCDate()))) AND (LAR.Verified IS NULL) AND (LAR.ResultRAG = 3) AND ((CAOC.IncludedInSelfAssessment = 1) 
						OR (SAS.Optional = 0)) AND (SA.EnforceRoleRequirementsForSignOff = 1)
                    ORDER BY SAS.Ordering, CAQ.Ordering",
                (competency, assessmentQuestion) =>
                {
                    competency.AssessmentQuestions.Add(assessmentQuestion);
                    return competency;
                },
                new { selfAssessmentId, delegateId }
            );
            return GroupCompetencyAssessmentQuestions(result);
        }

        public Competency? GetCompetencyByCandidateAssessmentResultId(
            int resultId,
            int candidateAssessmentId,
            int adminId
        )
        {
            Competency? competencyResult = null;
            return connection.Query<Competency, AssessmentQuestion, Competency>(
                $@"WITH {SpecificAssessmentResults}
                    SELECT {CompetencyFields},
                    LAR.SupervisorName
                    FROM {SpecificCompetencyTables}
                    WHERE ResultId = @resultId",
                (competency, assessmentQuestion) =>
                {
                    competencyResult ??= competency;
                    competencyResult.AssessmentQuestions.Add(assessmentQuestion);
                    return competencyResult;
                },
                new { resultId, candidateAssessmentId, adminId }
            ).FirstOrDefault();
        }

        public void SetResultForCompetency(
            int competencyId,
            int selfAssessmentId,
            int delegateUserId,
            int assessmentQuestionId,
            int? result,
            string? supportingComments
        )
        {
            var assessmentQuestion = connection.QueryFirstOrDefault<AssessmentQuestion>(
                @"SELECT ID, MinValue, MaxValue
                    FROM AssessmentQuestions
                    WHERE ID = @assessmentQuestionId",
                new { assessmentQuestionId }
            );
            if (assessmentQuestion == null)
            {
                logger.LogWarning(
                    "Not saving self assessment result as assessment question Id is invalid. " +
                    $"{PrintResult(competencyId, selfAssessmentId, delegateUserId, assessmentQuestionId, result)}"
                );
                return;
            }

            var minValue = assessmentQuestion.MinValue;
            var maxValue = assessmentQuestion.MaxValue;
            if (result < minValue || result > maxValue)
            {
                logger.LogWarning(
                    "Not saving self assessment result as result is invalid. " +
                    $"{PrintResult(competencyId, selfAssessmentId, delegateUserId, assessmentQuestionId, result)}"
                );
                return;
            }

            var numberOfAffectedRows = connection.Execute(
                @"IF EXISTS (
                        SELECT * FROM CandidateAssessments AS CA
                        INNER JOIN SelfAssessmentStructure AS SAS ON CA.SelfAssessmentID = SAS.SelfAssessmentID
                        INNER JOIN Competencies AS C ON SAS.CompetencyID = C.ID
                        INNER JOIN CompetencyAssessmentQuestions as CAQ ON SAS.CompetencyID = CAQ.CompetencyID
                        WHERE DelegateUserID = @delegateUserId
                            AND CA.SelfAssessmentID = @selfAssessmentId
                            AND C.ID = @competencyId
                            AND CAQ.AssessmentQuestionID = @assessmentQuestionId
                    )
                    BEGIN
                        DECLARE @existentResultId INT
                        DECLARE @existentResult INT

                        SELECT TOP 1 @existentResultId = ID, @existentResult = [Result]
                        FROM SelfAssessmentResults
                        WHERE [DelegateUserID] = @delegateUserId
                            AND [CompetencyID] = @competencyId
                            AND [AssessmentQuestionID] = @assessmentQuestionId
                        ORDER BY DateTime DESC

                        IF (@existentResultId IS NOT NULL AND ((@existentResult = @result) OR (@result IS NULL AND @existentResult IS NULL)))
                            BEGIN
                            UPDATE SelfAssessmentResults
                            SET [DateTime] = GETUTCDATE(),
                                [SupportingComments] = @supportingComments
                            WHERE ID = @existentResultId
                            END
                        IF (@existentResultId IS NOT NULL AND (@existentResult <> @result OR @result IS NULL OR @existentResult IS NULL))
                            BEGIN
                            UPDATE SelfAssessmentResults
                            SET [SelfAssessmentID] = @selfAssessmentId, [Result] = @result, [DateTime]  = GETUTCDATE(),
                                [SupportingComments] = @supportingComments
                            WHERE ID = @existentResultId

                            DELETE SARS FROM   SelfAssessmentResultSupervisorVerifications  sars
                            WHERE  SARS.SelfAssessmentResultId=@existentResultId
                            END
                         IF (@existentResultId IS NULL)
                            BEGIN
                            INSERT INTO SelfAssessmentResults
                                ([SelfAssessmentID]
                                ,[CompetencyID]
                                ,[AssessmentQuestionID]
                                ,[Result]
                                ,[DateTime]
                                ,[SupportingComments]
                                ,[DelegateUserID])
                            VALUES(@selfAssessmentId, @competencyId, @assessmentQuestionId, @result, GETUTCDATE(), @supportingComments,@delegateUserId)
                            END
                        END",
                new { competencyId, selfAssessmentId, delegateUserId, assessmentQuestionId, result, supportingComments }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not saving self assessment result as db insert failed. " +
                    $"{PrintResult(competencyId, selfAssessmentId, delegateUserId, assessmentQuestionId, result)}"
                );
            }
        }

        public IEnumerable<Competency> GetCandidateAssessmentOptionalCompetencies(int selfAssessmentId, int delegateUserId)
        {
            return connection.Query<Competency>(
                @"SELECT
                        C.ID AS Id,
                        SAS.ID AS SelfAssessmentStructureId,
                        ROW_NUMBER() OVER (ORDER BY SAS.Ordering) as RowNo,
                        C.Name AS Name,
                        C.Description AS Description,
                        CG.Name AS CompetencyGroup,
                        CG.ID AS CompetencyGroupID,
                        'Capability' AS Vocabulary,
                        SAS.Optional,
                        COALESCE (CAOC.IncludedInSelfAssessment, 0) AS IncludedInSelfAssessment,
                        SAS.GroupOptionalCompetencies 
                    FROM Competencies AS C
                    INNER JOIN CandidateAssessments AS CA
                        ON CA.SelfAssessmentID = @selfAssessmentId AND CA.DelegateUserID = @delegateUserId AND CA.RemovedDate IS NULL
                    INNER JOIN SelfAssessmentStructure AS SAS
                        ON C.ID = SAS.CompetencyID AND SAS.SelfAssessmentID = @selfAssessmentId
                    INNER JOIN CompetencyGroups AS CG
                        ON SAS.CompetencyGroupID = CG.ID AND SAS.SelfAssessmentID = @selfAssessmentId
                    LEFT OUTER JOIN CandidateAssessmentOptionalCompetencies AS CAOC
                        ON CA.ID = CAOC.CandidateAssessmentID AND C.ID = CAOC.CompetencyID AND CG.ID = CAOC.CompetencyGroupID
                    WHERE (SAS.Optional = 1)
                    ORDER BY SAS.Ordering",
                new { selfAssessmentId, delegateUserId }
            );
        }

        public void InsertCandidateAssessmentOptionalCompetenciesIfNotExist(int selfAssessmentId, int delegateUserId)
        {
            connection.Execute(
                @"UPDATE CandidateAssessmentOptionalCompetencies
                    SET IncludedInSelfAssessment = 0
                    FROM CandidateAssessmentOptionalCompetencies AS CAOC
                    INNER JOIN CandidateAssessments AS CA
                        ON CAOC.CandidateAssessmentID = CA.ID
                    INNER JOIN SelfAssessmentStructure AS SAS
                        ON CA.SelfAssessmentID = SAS.SelfAssessmentID AND CAOC.CompetencyID = SAS.CompetencyID AND CA.SelfAssessmentID = @selfAssessmentId
                            AND CAOC.CompetencyGroupID = SAS.CompetencyGroupID
                    WHERE (CA.DelegateUserID = @delegateUserId) AND (CA.RemovedDate IS NULL)
                                    ",
                new { selfAssessmentId, delegateUserId }
            );
            connection.Execute(
                @"INSERT INTO CandidateAssessmentOptionalCompetencies
                        (CandidateAssessmentId, CompetencyID, CompetencyGroupID)
                    SELECT
                        CA.ID, SAS.CompetencyID, SAS.CompetencyGroupID
                    FROM SelfAssessmentStructure AS SAS
                    INNER JOIN CandidateAssessments AS CA
                        ON SAS.SelfAssessmentID = CA.SelfAssessmentID AND CA.SelfAssessmentID = @selfAssessmentId
                            AND CA.DelegateUserID = @delegateUserId AND CA.RemovedDate IS NULL AND SAS.Optional = 1
                    WHERE NOT EXISTS (SELECT * FROM CandidateAssessmentOptionalCompetencies WHERE CandidateAssessmentID = CA.ID
                        AND CompetencyID = SAS.CompetencyID AND CompetencyGroupID = SAS.CompetencyGroupID)",
                new { selfAssessmentId, delegateUserId }
            );
        }

        public void UpdateCandidateAssessmentOptionalCompetencies(int selfAssessmentStructureId, int delegateUserId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE CandidateAssessmentOptionalCompetencies
                    SET IncludedInSelfAssessment = 1
                    FROM CandidateAssessmentOptionalCompetencies AS CAOC
                    INNER JOIN CandidateAssessments AS CA
                        ON CAOC.CandidateAssessmentID = CA.ID
                    INNER JOIN SelfAssessmentStructure AS SAS
                        ON CA.SelfAssessmentID = SAS.SelfAssessmentID AND CAOC.CompetencyID = SAS.CompetencyID
                            AND CAOC.CompetencyGroupID = SAS.CompetencyGroupID
                    WHERE (SAS.ID = @selfAssessmentStructureId) AND (CA.DelegateUserID = @delegateUserId) AND (CA.RemovedDate IS NULL)",
                new { selfAssessmentStructureId, delegateUserId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not setting CandidateAssessmentOptionalCompetencies include state as db update failed. " +
                    $"Self assessment id: {selfAssessmentStructureId}, Delegate User id: {delegateUserId} "
                );
            }
        }

        public IEnumerable<LevelDescriptor> GetLevelDescriptorsForAssessmentQuestion(
            int assessmentQuestionId,
            int minValue,
            int maxValue,
            bool zeroBased
        )
        {
            var adjustBy = zeroBased ? 1 : 0;
            return connection.Query<LevelDescriptor>(
                @"SELECT
                        COALESCE(ID,0) AS ID,
                        @assessmentQuestionId AS AssessmentQuestionID,
                        n AS LevelValue,
                        LevelLabel,
                        LevelDescription,
                        0 AS UpdatedByAdminID
                    FROM
                    (SELECT TOP (@maxValue + @adjustBy) n = ROW_NUMBER() OVER (ORDER BY number) - @adjustBy
                    FROM [master]..spt_values) AS q1
                    LEFT OUTER JOIN AssessmentQuestionLevels AS AQL
                        ON q1.n = AQL.LevelValue AND AQL.AssessmentQuestionID = @assessmentQuestionId
                    WHERE (q1.n BETWEEN @minValue AND @maxValue)",
                new { assessmentQuestionId, minValue, maxValue, adjustBy }
            );
        }

        public List<int> GetCandidateAssessmentIncludedSelfAssessmentStructureIds(int selfAssessmentId, int delegateUserId)
        {
            return connection.Query<int>(
                @"SELECT
                        SAS.ID
                    FROM CandidateAssessmentOptionalCompetencies AS CAOC
                    INNER JOIN CandidateAssessments  AS CA
                        ON CAOC.CandidateAssessmentID = CA.ID AND CA.SelfAssessmentID = @selfAssessmentId
                            AND CA.DelegateUserID = @delegateUserId AND CA.RemovedDate IS NULL
                    INNER JOIN SelfAssessmentStructure AS SAS
                            ON CAOC.CompetencyID = SAS.CompetencyID AND CAOC.CompetencyGroupID = SAS.CompetencyGroupID
                                AND SAS.SelfAssessmentID = @selfAssessmentId
                    WHERE (CAOC.IncludedInSelfAssessment = 1)",
                new { selfAssessmentId, delegateUserId }
            ).ToList();
        }

        public CompetencyAssessmentQuestionRoleRequirement? GetCompetencyAssessmentQuestionRoleRequirements(
            int competencyId,
            int selfAssessmentId,
            int assessmentQuestionId,
            int levelValue
        )
        {
            return connection.QuerySingleOrDefault<CompetencyAssessmentQuestionRoleRequirement>(
                @"SELECT
                        ID,
                        SelfAssessmentID,
                        CompetencyID,
                        AssessmentQuestionID,
                        LevelValue,
                        LevelRAG
                    FROM CompetencyAssessmentQuestionRoleRequirements
                    WHERE CompetencyID = @competencyId AND SelfAssessmentID = @selfAssessmentId
                        AND AssessmentQuestionID = @assessmentQuestionId AND LevelValue = @levelValue",
                new { selfAssessmentId, competencyId, assessmentQuestionId, levelValue }
            );
        }

        public IEnumerable<SelfAssessmentResult> GetSelfAssessmentResultsForDelegateSelfAssessmentCompetency(
            int delegateUserId,
            int selfAssessmentId,
            int competencyId
        )
        {
            return connection.Query<SelfAssessmentResult>(
                @"SELECT
                        ID,
                        SelfAssessmentID,
                        CompetencyID,
                        AssessmentQuestionID,
                        Result,
                        DateTime,
                        SupportingComments,
                        DelegateUserId
                    FROM SelfAssessmentResults
                    WHERE CompetencyID = @competencyId
                        AND SelfAssessmentID = @selfAssessmentId
                        AND DelegateUserID = @delegateUserId",
                new { selfAssessmentId, delegateUserId, competencyId }
            );
        }

        public IEnumerable<SelfAssessmentResult> GetSelfAssessmentResultswithSupervisorVerificationsForDelegateSelfAssessmentCompetency(
          int delegateUserId,
          int selfAssessmentId,
          int competencyId
      )
        {
            return connection.Query<SelfAssessmentResult>(
                @"SELECT
                         s.ID,
                        s.SelfAssessmentID,
                        s.CompetencyID,
                        s.AssessmentQuestionID,
                        s.Result,
                        s.DateTime,
                        s.SupportingComments,
                        s.DelegateUserId
                    FROM SelfAssessmentResults s inner join 
				SelfAssessmentResultSupervisorVerifications sv ON s.ID = sv.SelfAssessmentResultId AND sv.Superceded = 0 
                    WHERE s.CompetencyID = @competencyId
                        AND s.SelfAssessmentID = @selfAssessmentId
                        AND s.DelegateUserID = @delegateUserId AND sv.Verified IS NULL",
                new { selfAssessmentId, delegateUserId, competencyId }
            );
        }

        public void RemoveReviewCandidateAssessmentOptionalCompetencies(int id)
        {
           
            connection.Execute(
                     @"delete from SelfAssessmentResultSupervisorVerifications WHERE SelfAssessmentResultId = @id", new { id });

        }

        private static string PrintResult(
            int competencyId,
            int selfAssessmentId,
            int delegateUserId,
            int assessmentQuestionId,
            int? result
        )
        {
            return
                $"Competency id: {competencyId}, self assessment id: {selfAssessmentId}, delegate user id: {delegateUserId} " +
                $"assessment question id: {assessmentQuestionId}, result: {result}";
        }

        private static IEnumerable<Competency> GroupCompetencyAssessmentQuestions(IEnumerable<Competency> result)
        {
            return result.GroupBy(competency => competency.Id).Select(
                group =>
                {
                    var groupedCompetency = group.First();
                    groupedCompetency.AssessmentQuestions =
                        group.Select(competency => competency.AssessmentQuestions.Single()).ToList();
                    return groupedCompetency;
                }
            );
        }
    }
}
