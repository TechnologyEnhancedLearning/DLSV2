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
                        CR.CentreName AS CentreName
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
                        CA.UserBookmark, CA.UnprocessedUpdates, CA.LaunchCount, CA.SubmittedDate, CR.CentreName",
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
                        SA.ManageSupervisorsDescription
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
                        SA.ReviewerCommentsLabel,SA.EnforceRoleRequirementsForSignOff, SA.ManageSupervisorsDescription",
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
    }
}
