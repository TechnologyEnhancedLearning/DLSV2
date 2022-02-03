﻿namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using Microsoft.Extensions.Logging;

    public interface IProgressDataService
    {
        IEnumerable<Progress> GetDelegateProgressForCourse(int delegateId, int customisationId);

        void UpdateProgressSupervisorAndCompleteByDate(int progressId, int supervisorAdminId, DateTime? completeByDate);

        int CreateNewDelegateProgress(
            int delegateId,
            int customisationId,
            int customisationVersion,
            DateTime submittedTime,
            int enrollmentMethodId,
            int? enrolledByAdminId,
            DateTime? completeByDate,
            int supervisorAdminId
        );

        void CreateNewAspProgress(int tutorialId, int progressId);

        void InsertNewAspProgressRecordsForTutorialIfNoneExist(int tutorialId, int customisationId);

        void ClearAspProgressVerificationRequest(int progressId);

        void SetCompletionDate(int progressId, DateTime? completeByDate);

        void UpdateDiagnosticScore(int progressId, int tutorialId, int myScore);

        void UnlockProgress(int progressId);

        int? GetDiagnosticScore(int progressId);

        // TODO HEEDLS-567 rename this, it's not 'for course' exactly, but by progress
        IEnumerable<DetailedSectionProgress> GetSectionProgressDataForCourse(int progressId);

        IEnumerable<DetailedTutorialProgress> GetTutorialProgressDataForSection(int progressId, int sectionId);
    }

    public class ProgressDataService : IProgressDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<ProgressDataService> logger;

        public ProgressDataService(IDbConnection connection, ILogger<ProgressDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public IEnumerable<Progress> GetDelegateProgressForCourse(
            int delegateId,
            int customisationId
        )
        {
            return connection.Query<Progress>(
                @"SELECT ProgressId,
                        CandidateID,
                        CustomisationID,
                        Completed,
                        RemovedDate,
                        SystemRefreshed,
                        SupervisorAdminID,
                        CompleteByDate,
                        EnrollmentMethodID,
                        EnrolledByAdminID,
                        SubmittedTime,
                        CustomisationVersion
                    FROM Progress
                    WHERE CandidateID = @delegateId
                        AND CustomisationID = @customisationId",
                new { delegateId, customisationId }
            );
        }

        public void UpdateProgressSupervisorAndCompleteByDate(
            int progressId,
            int supervisorAdminId,
            DateTime? completeByDate
        )
        {
            connection.Execute(
                @"UPDATE Progress SET
                        SupervisorAdminID = @supervisorAdminId,
                        CompleteByDate = @completeByDate
                    WHERE ProgressID = @progressId",
                new { progressId, supervisorAdminId, completeByDate }
            );
        }

        public int CreateNewDelegateProgress(
            int delegateId,
            int customisationId,
            int customisationVersion,
            DateTime submittedTime,
            int enrollmentMethodId,
            int? enrolledByAdminId,
            DateTime? completeByDate,
            int supervisorAdminId
        )
        {
            var progressId = connection.QuerySingle<int>(
                @"INSERT INTO Progress(
                        CandidateID,
                        CustomisationID,
                        CustomisationVersion,
                        SubmittedTime,
                        EnrollmentMethodID,
                        EnrolledByAdminID,
                        CompleteByDate,
                        SupervisorAdminID)
                    OUTPUT Inserted.ProgressID
                    VALUES (
                        @delegateId,
                        @customisationId,
                        @customisationVersion,
                        @submittedTime,
                        @enrollmentMethodId,
                        @enrolledByAdminId,
                        @completeByDate,
                        @supervisorAdminId)",
                new
                {
                    delegateId,
                    customisationId,
                    customisationVersion,
                    submittedTime,
                    enrollmentMethodId,
                    enrolledByAdminId,
                    completeByDate,
                    supervisorAdminId,
                }
            );

            return progressId;
        }

        public void CreateNewAspProgress(int tutorialId, int progressId)
        {
            connection.Execute(
                @"INSERT INTO aspProgress (TutorialId, ProgressId)
                    VALUES (@tutorialId, @progressId)",
                new { tutorialId, progressId }
            );
        }

        public void InsertNewAspProgressRecordsForTutorialIfNoneExist(int tutorialId, int customisationId)
        {
            connection.Execute(
                @"INSERT INTO aspProgress
                    (TutorialID, ProgressID)
                    SELECT
                        @tutorialID,
                        ProgressID
                    FROM Progress
                    WHERE RemovedDate IS NULL
                        AND CustomisationID = @customisationID
                        AND ProgressID NOT IN
                            (SELECT ProgressID
                            FROM aspProgress
                            WHERE TutorialID = @tutorialID
                                AND CustomisationID = @customisationID)",
                new { tutorialId, customisationId }
            );
        }

        public void ClearAspProgressVerificationRequest(int progressId)
        {
            connection.Execute(
                @"UPDATE aspProgress SET
                        SupervisorVerificationRequested = NULL
                    WHERE ProgressID = @progressId",
                new { progressId }
            );
        }

        public void SetCompletionDate(int progressId, DateTime? completionDate)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE Progress
                        SET Completed = @completionDate
                        WHERE ProgressID = @progressId",
                new { completionDate, progressId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not setting current course completion date as db update failed. " +
                    $"Progress id: {progressId}, completion date: {completionDate}"
                );
            }
        }

        public void UpdateDiagnosticScore(int progressId, int tutorialId, int myScore)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE ap
                        SET ap.DiagHigh = IIF(@myScore > DiagHigh, @myScore, DiagHigh),
                            ap.DiagLow = CASE WHEN ap.DiagAttempts = 0 THEN @myScore
                                ELSE IIF(@myScore > DiagLow, DiagLow, @myScore) END,
                            ap.DiagLast = @myScore,
                            ap.DiagAttempts = DiagAttempts + 1
                        FROM aspProgress AS ap
                        INNER JOIN Tutorials AS t ON t.TutorialID = ap.TutorialID
                        WHERE ap.ProgressID = @progressId
                        AND (ap.TutorialID = @tutorialId OR t.OriginalTutorialID = @tutorialId)",
                new { progressId, tutorialId, myScore }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating diagnostic score as db update failed. " +
                    $"Progress id: {progressId}, tutorial Id: {tutorialId}, myScore: {myScore}"
                );
                throw new Exception("No aspProgress records were affected when updating diagnostic score");
            }
        }

        public void UnlockProgress(int progressId)
        {
            connection.Execute(
                @"UPDATE Progress SET
                        PLLocked = 0
                    WHERE ProgressID = @progressId",
                new { progressId }
            );
        }

        public int? GetDiagnosticScore(int progressId)
        {
            return connection.Query<int?>(
                @"SELECT DiagnosticScore FROM Progress WHERE ProgressID = @progressId",
                new { progressId }
            ).Single();
        }

        public IEnumerable<DetailedSectionProgress> GetSectionProgressDataForCourse(int progressId)
        {
            return connection.Query<DetailedSectionProgress>(
                @"SELECT
                        s.SectionName,
                        (SUM(asp1.TutStat) * 100) / (COUNT(t.TutorialID) * 2) AS Completion,
                        SUM(asp1.TutTime) AS TotalTime,
                        s.AverageSectionMins AS AverageTime,
                        (s.PLAssessPath IS NOT NULL AND cu.IsAssessed IS 1) AS PostLearningAssessment,
                        COALESCE (MAX(ISNULL(aa.Score, 0)), 0) AS Outcome,
                        (SELECT COUNT(AssessAttemptID) AS PLAttempts
                            FROM AssessAttempts AS aa
                            WHERE (ProgressID = @ProgressID) AND (SectionNumber = s.SectionNumber)) AS Attempts,
                        MAX(ISNULL(CAST(ct.Status AS BIT), 0)) AS Passed,
                    FROM
                        aspProgress AS asp1
                        INNER JOIN Progress AS p ON asp1.ProgressID = p.ProgressID
                        INNER JOIN Sections AS s
                        INNER JOIN Tutorials AS t ON s.SectionID = t.SectionID
                        INNER JOIN CustomisationTutorials AS ct ON t.TutorialID = ct.TutorialID ON asp1.TutorialID = t.TutorialID
                        INNER JOIN Customisations AS cu ON p.CustomisationID = cu.CustomisationID
                        LEFT OUTER JOIN AssessAttempts AS aa ON p.ProgressID = aa.ProgressID AND s.SectionNumber = aa.SectionNumber
                    WHERE
                        (ct.CustomisationID = p.CustomisationID) AND (p.ProgressID = @ProgressID) AND (s.ArchivedDate IS NULL)
                        AND (ct.Status = 1 OR ct.DiagStatus = 1 OR cu.IsAssessed = 1)
                    ",
                new { progressId }
            );
        }

        public IEnumerable<DetailedTutorialProgress> GetTutorialProgressDataForSection(int progressId, int sectionId)
        {
            return connection.Query<DetailedTutorialProgress>(
                @"SELECT
                        t.TutorialName,
                        ts.Status AS TutorialStatus,
                        ap.TutTime AS TimeTaken,
                        CASE WHEN t.OverrideTutorialMins > 0 THEN t.OverrideTutorialMins ELSE t.AverageTutMins END AS AvgTime,
                        CASE WHEN (ap.DiagAttempts > 0 AND ct.DiagStatus = 1) THEN ap.DiagLast ELSE NULL END AS DiagnosticScore,
                        t.DiagAssessOutOf AS PossibleScore
                    FROM
                        Progress AS p
                        INNER JOIN Tutorials AS t
                        INNER JOIN CustomisationTutorials AS ct ON t.TutorialID = ct.TutorialID
                        INNER JOIN Customisations AS c ON ct.CustomisationID = c.CustomisationID ON p.CustomisationID = c.CustomisationID AND p.CustomisationID = ct.CustomisationID
                        INNER JOIN TutStatus AS ts ON
                        INNER JOIN aspProgress AS ap ON ts.TutStatusID = ap.TutStat ON P.ProgressID = ap.ProgressID AND t.TutorialID = ap.TutorialID
                    WHERE (t.SectionID = @SectionID)
                        AND (p.ProgressID = @ProgressID)
                        AND (ct.Status = 1)
                        AND (c.Active = 1)
                        AND (t.ArchivedDate IS NULL)
                    ",
                new { progressId, sectionId }
            );
        }
    }
}
