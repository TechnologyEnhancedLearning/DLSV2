namespace DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService
{
    using System;
    using System.Collections.Generic;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using Microsoft.Extensions.Logging;

    public partial class SelfAssessmentDataService
    {
        public IEnumerable<CurrentSelfAssessment> GetSelfAssessmentsForCandidate(int delegateUserId, int centreId)
        {
            return connection.Query<CurrentSelfAssessment>(
                @"SELECT
                        CA.SelfAssessmentID AS Id,
                        SA.Name,
                        SA.Description,
                        SA.IncludesSignposting,
                        SA.IncludeRequirementsFilters,
                        SA.SupervisorResultsReview AS IsSupervisorResultsReviewed,
                        SA.ReviewerCommentsLabel,
                        COALESCE(SA.Vocabulary, 'Capability') AS Vocabulary,
                        COUNT(C.ID) AS NumberOfCompetencies,
                        CA.StartedDate,
                        CA.LastAccessed,
                        CA.CompleteByDate,
                        CA.ID AS CandidateAssessmentId,
                        CA.UserBookmark,
                        CA.UnprocessedUpdates,
                        CA.LaunchCount,
                        1 AS IsSelfAssessment,
                        CA.SubmittedDate,
                        CR.CentreName AS CentreName,
                        CA.EnrolmentMethodId
                    FROM Centres AS CR INNER JOIN
                        CandidateAssessments AS CA INNER JOIN
                        SelfAssessments AS SA ON CA.SelfAssessmentID = SA.ID ON CR.CentreID = CA.CentreID INNER JOIN
                        CentreSelfAssessments AS csa ON csa.SelfAssessmentID = SA.ID AND csa.CentreID = @centreId LEFT OUTER JOIN
                        Competencies AS C RIGHT OUTER JOIN
                        SelfAssessmentStructure AS SAS ON C.ID = SAS.CompetencyID ON CA.SelfAssessmentID = SAS.SelfAssessmentID
                    WHERE (CA.DelegateUserID = @delegateUserId) AND (CA.RemovedDate IS NULL) AND (CA.CompletedDate IS NULL)
                    GROUP BY
                        CA.SelfAssessmentID, SA.Name, SA.Description, SA.IncludesSignposting, SA.SupervisorResultsReview,
                        SA.ReviewerCommentsLabel, SA.IncludeRequirementsFilters,
                        COALESCE(SA.Vocabulary, 'Capability'), CA.StartedDate, CA.LastAccessed, CA.CompleteByDate,
                        CA.ID,
                        CA.UserBookmark, CA.UnprocessedUpdates, CA.LaunchCount, CA.SubmittedDate, CR.CentreName,CA.EnrolmentMethodId",
                new { delegateUserId, centreId }
            );
        }

        public CurrentSelfAssessment? GetSelfAssessmentForCandidateById(int delegateUserId, int selfAssessmentId)
        {
            return connection.QueryFirstOrDefault<CurrentSelfAssessment>(
                @"SELECT
                        CA.SelfAssessmentID AS Id,
                        SA.Name,
                        SA.Description,
                        SA.QuestionLabel,
                        SA.DescriptionLabel,
                        SA.IncludesSignposting,
                        SA.IncludeRequirementsFilters,
                        SA.SupervisorResultsReview AS IsSupervisorResultsReviewed,
                        SA.SupervisorSelfAssessmentReview,
                        SA.ReviewerCommentsLabel,
                        SA.EnforceRoleRequirementsForSignOff,
                        COALESCE(SA.Vocabulary, 'Capability') AS Vocabulary,
                        COUNT(C.ID) AS NumberOfCompetencies,
                        CA.StartedDate,
                        CA.LastAccessed,
                        CA.CompleteByDate,
                        CA.ID AS CandidateAssessmentId,
                        CA.UserBookmark,
                        CA.UnprocessedUpdates,
                        CA.LaunchCount,
                        CA.SubmittedDate,
                        SA.LinearNavigation,
                        SA.UseDescriptionExpanders,
                        SA.ManageOptionalCompetenciesPrompt,
                        CAST(CASE WHEN SA.SupervisorSelfAssessmentReview = 1 OR SA.SupervisorResultsReview = 1 THEN 1 ELSE 0 END AS BIT) AS IsSupervised,
                        CASE
                            WHEN (SELECT COUNT(*) FROM SelfAssessmentSupervisorRoles WHERE SelfAssessmentID = @selfAssessmentId AND AllowDelegateNomination = 1) > 0
                            THEN 1
                            ELSE 0
                        END AS HasDelegateNominatedRoles,
                        COALESCE(
                            (SELECT TOP (1) RoleName FROM SelfAssessmentSupervisorRoles
                            WHERE (ResultsReview = 1) AND (SelfAssessmentID = @selfAssessmentId) AND
                                   ((SELECT COUNT(*) AS Expr1 FROM SelfAssessmentSupervisorRoles AS SelfAssessmentSupervisorRoles_1
                                    WHERE (ResultsReview = 1) AND (SelfAssessmentID = @selfAssessmentId)) = 1)),
                            'Supervisor') AS VerificationRoleName,
                        COALESCE(
                            (SELECT TOP (1) RoleName FROM SelfAssessmentSupervisorRoles
                            WHERE (SelfAssessmentReview = 1) AND (SelfAssessmentID = @selfAssessmentId) AND
                                   ((SELECT COUNT(*) AS Expr1
                                    FROM SelfAssessmentSupervisorRoles AS SelfAssessmentSupervisorRoles_1
                                    WHERE (SelfAssessmentReview = 1) AND (SelfAssessmentID = @selfAssessmentId)) = 1)),
                            'Supervisor') AS SignOffRoleName,
                        SA.SignOffRequestorStatement,
                        SA.ManageSupervisorsDescription,
                        CA.NonReportable
                    FROM CandidateAssessments CA
                    JOIN SelfAssessments SA
                        ON CA.SelfAssessmentID = SA.ID
                    INNER JOIN SelfAssessmentStructure AS SAS
                        ON CA.SelfAssessmentID = SAS.SelfAssessmentID
                    INNER JOIN Competencies AS C
                        ON SAS.CompetencyID = C.ID
                    INNER JOIN CompetencyGroups AS CG
                            ON SAS.CompetencyGroupID = CG.ID AND SAS.SelfAssessmentID = @selfAssessmentId
                    LEFT OUTER JOIN CandidateAssessmentOptionalCompetencies AS CAOC
                            ON CA.ID = CAOC.CandidateAssessmentID AND C.ID = CAOC.CompetencyID AND CG.ID = CAOC.CompetencyGroupID
                    WHERE CA.DelegateUserID = @delegateUserId AND CA.SelfAssessmentID = @selfAssessmentId AND CA.RemovedDate IS NULL
                        AND CA.CompletedDate IS NULL AND ((SAS.Optional = 0) OR (CAOC.IncludedInSelfAssessment = 1))
                    GROUP BY
                        CA.SelfAssessmentID, SA.Name, SA.Description,
                        SA.DescriptionLabel, SA.QuestionLabel,
                        SA.IncludesSignposting, SA.IncludeRequirementsFilters, SA.SignOffRequestorStatement, COALESCE(SA.Vocabulary, 'Capability'),
                        CA.StartedDate, CA.LastAccessed, CA.CompleteByDate,
                        CA.ID, CA.UserBookmark, CA.UnprocessedUpdates,
                        CA.LaunchCount, CA.SubmittedDate, SA.LinearNavigation, SA.UseDescriptionExpanders,
                        SA.ManageOptionalCompetenciesPrompt, SA.SupervisorSelfAssessmentReview, SA.SupervisorResultsReview,
                        SA.ReviewerCommentsLabel,SA.EnforceRoleRequirementsForSignOff, SA.ManageSupervisorsDescription,CA.NonReportable",
                new { delegateUserId, selfAssessmentId }
            );
        }

        public void UpdateLastAccessed(int selfAssessmentId, int delegateUserId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE CandidateAssessments SET LastAccessed = GETUTCDATE()
                      WHERE SelfAssessmentID = @selfAssessmentId AND DelegateUserID = @delegateUserId",
                new { selfAssessmentId, delegateUserId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating self assessment last accessed date as db update failed. " +
                    $"Self assessment id: {selfAssessmentId}, Delegate User id: {delegateUserId}"
                );
            }
        }

        public void RemoveSignoffRequests(int selfAssessmentId, int delegateUserId, int competencyGroupId)
        {
            var candidateAssessmentSupervisorVerificationsId = connection.QueryFirst<int>(
                @" SELECT casv.ID 
                    FROM( SELECT DISTINCT casv.* FROM CandidateAssessmentSupervisorVerifications AS casv
                    INNER JOIN CandidateAssessmentSupervisors AS cas
                        ON casv.CandidateAssessmentSupervisorID = cas.ID
                    INNER JOIN CandidateAssessments AS ca
                        ON cas.CandidateAssessmentID = ca.ID
                    INNER JOIN SupervisorDelegates AS sd
                        ON cas.SupervisorDelegateId = sd.ID
                    INNER JOIN AdminUsers AS au
                        ON sd.SupervisorAdminID = au.AdminID
                    LEFT OUTER JOIN SelfAssessmentSupervisorRoles AS sasr
                        ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
						  INNER JOIN SelfAssessmentStructure AS SAS 
						  ON CA.SelfAssessmentID = SAS.SelfAssessmentID
                    INNER JOIN CompetencyGroups AS CG 
					ON SAS.CompetencyGroupID = CG.ID AND SAS.SelfAssessmentID =@selfAssessmentId
                    WHERE ((ca.DelegateUserID = @delegateUserId) AND (ca.SelfAssessmentID = @selfAssessmentId) AND (sasr.SelfAssessmentReview = 1) AND (CG.ID =@competencyGroupId) AND (casv.SignedOff =0))
                        OR ((ca.DelegateUserID = @delegateUserId) AND (ca.SelfAssessmentID = @selfAssessmentId) AND (sasr.SelfAssessmentReview IS NULL) AND (CG.ID =@competencyGroupId) AND (casv.SignedOff =0))
						) AS casv;
						",
                new { selfAssessmentId, delegateUserId, competencyGroupId }
            );
            if(candidateAssessmentSupervisorVerificationsId > 0) 
            {
                var numberOfAffectedRows = connection.Execute(
                  @"   DELETE FROM CandidateAssessmentSupervisorVerifications WHERE ID = @candidateAssessmentSupervisorVerificationsId ",
                new { candidateAssessmentSupervisorVerificationsId });
            }
        }

        public void SetCompleteByDate(int selfAssessmentId, int delegateUserId, DateTime? completeByDate)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE CandidateAssessments
                        SET CompleteByDate = @date
                        WHERE SelfAssessmentID = @selfAssessmentId
                          AND DelegateUserID = @delegateUserId",
                new { date = completeByDate, selfAssessmentId, delegateUserId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not setting self assessment complete by date as db update failed. " +
                    $"Self assessment id: {selfAssessmentId}, Delegate User id: {delegateUserId}, complete by date: {completeByDate}"
                );
            }
        }
        public void SetUpdatedFlag(int selfAssessmentId, int delegateUserId, bool status)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE CandidateAssessments
                        SET UnprocessedUpdates = @status
                        WHERE SelfAssessmentID = @selfAssessmentId
                          AND DelegateUserID = @delegateUserId",
                new { status, selfAssessmentId, delegateUserId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not setting self assessment updated flag as db update failed. " +
                    $"Self assessment id: {selfAssessmentId}, Delegate User id: {delegateUserId}, status: {status}"
                );
            }
        }

        public void SetBookmark(int selfAssessmentId, int delegateUserId, string bookmark)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE CandidateAssessments
                        SET UserBookmark = @bookmark
                        WHERE SelfAssessmentID = @selfAssessmentId
                          AND DelegateUserID = @delegateUserId",
                new { bookmark, selfAssessmentId, delegateUserId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not setting self assessment bookmark as db update failed. " +
                    $"Self assessment id: {selfAssessmentId}, Delegate User id: {delegateUserId}, bookmark: {bookmark}"
                );
            }
        }

        public void IncrementLaunchCount(int selfAssessmentId, int delegateUserId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE CandidateAssessments SET LaunchCount = LaunchCount+1
                      WHERE SelfAssessmentID = @selfAssessmentId AND DelegateUserID = @delegateUserId",
                new { selfAssessmentId, delegateUserId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating self assessment launch count as db update failed. " +
                    $"Self assessment id: {selfAssessmentId}, Delegate User id: {delegateUserId}"
                );
            }
        }

        public void SetSubmittedDateNow(int selfAssessmentId, int delegateUserId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE CandidateAssessments SET SubmittedDate = GETUTCDATE()
                      WHERE SelfAssessmentID = @selfAssessmentId AND DelegateUserID = @delegateUserId AND SubmittedDate IS NULL",
                new { selfAssessmentId, delegateUserId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not setting self assessment submitted date as db update failed. " +
                    $"Self assessment id: {selfAssessmentId}, Delegate User id: {delegateUserId}"
                );
            }
        }

        public void RemoveEnrolment(int selfAssessmentId, int delegateUserId)
        {
            connection.Execute(
                @"UPDATE CandidateAssessments SET RemovedDate = GETUTCDATE()
                      WHERE SelfAssessmentID = @selfAssessmentId AND DelegateUserID = @delegateUserId",
                new { selfAssessmentId, delegateUserId }
            );
        }

        public IEnumerable<CandidateAssessment> GetCandidateAssessments(int delegateUserId, int selfAssessmentId)
        {
            return connection.Query<CandidateAssessment>(
                @"SELECT
                        ID,
                        DelegateUserID,
                        SelfAssessmentID,
                        CompletedDate,
                        RemovedDate
                    FROM CandidateAssessments
                    WHERE SelfAssessmentID = @selfAssessmentId
                        AND DelegateUserID = @delegateUserId",
                new { selfAssessmentId, delegateUserId }
            );
        }
        public CompetencySelfAssessmentCertificate GetCompetencySelfAssessmentCertificate(int candidateAssessmentID)
        {
            return connection.QueryFirstOrDefault<CompetencySelfAssessmentCertificate>(
                @"SELECT casv.ID,
                    sa.Name AS SelfAssessment, 
                    Learner.FirstName + '  ' + Learner.LastName AS LearnerName,
                    Learner.ProfessionalRegistrationNumber AS LearnerPRN,
                    casv.Verified, 
                    ce.CentreName, 
                    Supervisor.FirstName + ' ' + Supervisor.LastName AS SupervisorName, 
                    Supervisor.ProfessionalRegistrationNumber AS SupervisorPRN,
                    b.BrandName, 
                    b.BrandImage,
                    ca.ID  CandidateAssessmentID,
                    ca.SelfAssessmentID ,
                    ca.SelfAssessmentID,  COALESCE(SA.Vocabulary, 'Capability') AS Vocabulary,
					cas.SupervisorDelegateId,CONVERT(VARCHAR(2), DAY(Verified)) +
    CASE 
        WHEN DAY(Verified) % 100 IN (11, 12, 13) THEN 'th'
        WHEN DAY(Verified) % 10 = 1 THEN 'st'
        WHEN DAY(Verified) % 10 = 2 THEN 'nd'
        WHEN DAY(Verified) % 10 = 3 THEN 'rd'
        ELSE 'th'
    END +
    ' ' +
    FORMAT(Verified, 'MMMM yyyy') AS FormattedDate
             FROM   CandidateAssessmentSupervisorVerifications AS casv INNER JOIN
             CandidateAssessmentSupervisors AS cas ON casv.CandidateAssessmentSupervisorID = cas.ID INNER JOIN
             AdminAccounts AS aa ON casv.ID = aa.ID INNER JOIN
             Users AS Supervisor ON aa.UserID = Supervisor.ID INNER JOIN
             CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
             Users AS Learner ON ca.DelegateUserID = Learner.ID INNER JOIN
             SelfAssessments AS sa ON ca.SelfAssessmentID = sa.ID INNER JOIN
             Brands AS b ON sa.BrandID = b.BrandID INNER JOIN
             Centres AS ce ON aa.CentreID = ce.CentreID
             WHERE (ca.Id=@candidateAssessmentID) AND  (casv.SignedOff = 1) AND (NOT (casv.Verified IS NULL))",
                new { candidateAssessmentID }
            );
        }

        public IEnumerable<CompetencyCountSelfAssessmentCertificate> GetCompetencyCountSelfAssessmentCertificate(int candidateAssessmentID)
        {
            return connection.Query<CompetencyCountSelfAssessmentCertificate>(
                @"SELECT
                    cg.ID AS CompetencyGroupID,
                    cg.Name AS CompetencyGroup,
                    COUNT(caoc.CompetencyID) AS OptionalCompetencyCount
                    FROM   CandidateAssessmentOptionalCompetencies AS caoc INNER JOIN
             CompetencyGroups AS cg ON caoc.CompetencyGroupID = cg.ID
                WHERE (caoc.CandidateAssessmentID = @candidateAssessmentID) AND (caoc.IncludedInSelfAssessment = 1)
                    GROUP BY cg.Name, cg.ID",
                new { candidateAssessmentID }
            );
        }
        public IEnumerable<Accessor> GetAccessor(int selfAssessmentId)
        {
            return connection.Query<Accessor>(
                @"SELECT 
                    Accessor.FirstName + '  ' + Accessor.LastName AS AccessorName,
                    Accessor.ProfessionalRegistrationNumber AS AccessorPRN
             FROM   CandidateAssessmentSupervisorVerifications AS casv INNER JOIN
             CandidateAssessmentSupervisors AS cas ON casv.CandidateAssessmentSupervisorID = cas.ID INNER JOIN
             CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
             Users AS Accessor ON ca.DelegateUserID = Accessor.ID INNER JOIN
             SelfAssessments AS sa ON ca.SelfAssessmentID = sa.ID 
             WHERE 
			 (sa.id =@selfAssessmentId) and (Accessor.ProfessionalRegistrationNumber is not null) AND
			 (casv.SignedOff = 1) AND (NOT (casv.Verified IS NULL))",
                new { selfAssessmentId }
            );
        }
        public ActivitySummaryCompetencySelfAssesment GetActivitySummaryCompetencySelfAssesment(int CandidateAssessmentSupervisorVerificationsId)
        {
            return connection.QueryFirstOrDefault<ActivitySummaryCompetencySelfAssesment>(
                @"SELECT ca.ID AS CandidateAssessmentID, ca.SelfAssessmentID, sa.Name AS RoleName, casv.ID AS CandidateAssessmentSupervisorVerificationId,
                 (SELECT COUNT(sas1.CompetencyID) AS CompetencyAssessmentQuestionCount
                 FROM    SelfAssessmentStructure AS sas1 INNER JOIN
                              CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
                              CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID LEFT OUTER JOIN
                              CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
                 WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) OR
                              (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1)) AS CompetencyAssessmentQuestionCount,
                 (SELECT COUNT(sas1.CompetencyID) AS ResultCount
                     FROM   SelfAssessmentStructure AS sas1 INNER JOIN
             CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
             CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID LEFT OUTER JOIN
             SelfAssessmentResults AS sar1 ON sar1.SelfAssessmentID =sas1.SelfAssessmentID and sar1.CompetencyID=sas1.CompetencyID AND sar1.AssessmentQuestionID = caq1.AssessmentQuestionID AND sar1.DelegateUserID = ca1.DelegateUserID
                 LEFT OUTER JOIN    CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
                  WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) OR
             (ca1.ID = ca.ID) AND (NOT (sar1.Result IS NULL)) AND (caoc1.IncludedInSelfAssessment = 1) OR
             (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.SupportingComments IS NULL)) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL))) AS ResultCount,
                 (SELECT COUNT(sas1.CompetencyID) AS VerifiedCount
                FROM   SelfAssessmentResultSupervisorVerifications AS sasrv INNER JOIN
             SelfAssessmentResults AS sar1 ON sasrv.SelfAssessmentResultId = sar1.ID AND sasrv.Superceded = 0 RIGHT OUTER JOIN
             SelfAssessmentStructure AS sas1 INNER JOIN
             CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
             CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.SelfAssessmentID=sas1.SelfAssessmentID and sar1.CompetencyID=sas1.CompetencyID AND sar1.AssessmentQuestionID = caq1.AssessmentQuestionID AND sar1.DelegateUserID = ca1.DelegateUserID
				 LEFT OUTER JOIN    CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
                    WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (sasrv.SignedOff = 1) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.Result IS NULL)) AND (sasrv.SignedOff = 1) OR
             (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.SupportingComments IS NULL)) AND (sasrv.SignedOff = 1) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL)) AND (sasrv.SignedOff = 1)) AS VerifiedCount
                  FROM   NRPProfessionalGroups AS npg RIGHT OUTER JOIN
             NRPSubGroups AS nsg RIGHT OUTER JOIN
             SelfAssessmentSupervisorRoles AS sasr RIGHT OUTER JOIN
             SelfAssessments AS sa INNER JOIN
             CandidateAssessmentSupervisorVerifications AS casv INNER JOIN
             CandidateAssessmentSupervisors AS cas ON casv.CandidateAssessmentSupervisorID = cas.ID AND casv.Verified IS NULL AND cas.Removed IS NULL INNER JOIN
             CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID ON sa.ID = ca.SelfAssessmentID ON sasr.ID = cas.SelfAssessmentSupervisorRoleID ON nsg.ID = sa.NRPSubGroupID ON npg.ID = sa.NRPProfessionalGroupID LEFT OUTER JOIN
             NRPRoles AS nr ON sa.NRPRoleID = nr.ID
              WHERE (casv.ID = @CandidateAssessmentSupervisorVerificationsId)",
                new { CandidateAssessmentSupervisorVerificationsId }
            );
        }
    }
}
