namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Progress;
    using DigitalLearningSolutions.Data.Models.Tracker;
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

        void LockProgress(int progressId);

        IEnumerable<LearningLogEntry> GetLearningLogEntries(int progressId);

        Progress? GetProgressByProgressId(int progressId);

        IEnumerable<DetailedSectionProgress> GetSectionProgressDataForProgressEntry(int progressId);

        IEnumerable<DetailedTutorialProgress> GetTutorialProgressDataForSection(int progressId, int sectionId);

        SectionAndApplicationDetailsForAssessAttempts? GetSectionAndApplicationDetailsForAssessAttempts(
            int sectionId,
            int customisationId
        );

        void UpdateCourseAdminFieldForDelegate(
            int progressId,
            int promptNumber,
            string? answer
        );

        void UpdateProgressDetailsForStoreAspProgressV2(
            int progressId,
            int customisationVersion,
            DateTime submittedTime,
            string progressText
        );

        void UpdateAspProgressTutTime(
            int tutorialId,
            int progressId,
            int tutTime
        );

        void UpdateAspProgressTutStat(
            int tutorialId,
            int progressId,
            int tutStat
        );

        int GetCompletionStatusForProgress(int progressId);

        IEnumerable<AssessAttempt> GetAssessAttemptsForProgressSection(int progressId, int sectionNumber);

        void InsertAssessAttempt(
            int delegateId,
            int customisationId,
            int version,
            DateTime insertionDate,
            int sectionNumber,
            int score,
            bool status,
            int progressId
        );
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
                        CustomisationVersion,
                        DiagnosticScore
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

        public void LockProgress(int progressId)
        {
            connection.Execute(
                @"UPDATE Progress SET
                        PLLocked = 1
                    WHERE ProgressID = @progressId",
                new { progressId }
            );
        }

        public IEnumerable<LearningLogEntry> GetLearningLogEntries(int progressId)
        {
            return connection.Query<LearningLogEntry>(
                @"SELECT
                        ses.LoginTime AS [When],
                        ses.Duration  AS LearningTime,
                        NULL AS AssessmentTaken,
                        NULL AS AssessmentScore,
                        NULL AS AssessmentStatus
                    FROM [Sessions] AS ses
                    INNER JOIN Progress AS pr ON pr.CustomisationID = ses.CustomisationID AND pr.CandidateID = ses.CandidateID
                    WHERE pr.ProgressID = @progressId
                    UNION ALL
                    SELECT
                        aa.[Date] AS [When],
                        NULL AS Duration,
                        sec.SectionName AS AssessmentTaken,
                        aa.Score AS AssessmentScore,
                        aa.[Status] AS AssessmentStatus
                    FROM AssessAttempts AS aa
                    INNER JOIN dbo.Customisations AS cu ON cu.CustomisationID = aa.CustomisationID
                    INNER JOIN Applications AS a ON a.ApplicationID = cu.ApplicationID
                    LEFT JOIN Sections AS sec ON sec.ApplicationID = cu.ApplicationID AND sec.SectionNumber = aa.SectionNumber
                    WHERE aa.ProgressID = @progressId
                        AND a.DefaultContentTypeID <> 4",
                new { progressId }
            );
        }

        public Progress? GetProgressByProgressId(int progressId)
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
                        CustomisationVersion,
                        DiagnosticScore
                    FROM Progress
                    WHERE ProgressID = @progressId",
                new { progressId }
            ).SingleOrDefault();
        }

        public IEnumerable<DetailedSectionProgress> GetSectionProgressDataForProgressEntry(int progressId)
        {
            return connection.Query<DetailedSectionProgress>(
                @"SELECT
                        s.SectionID,
                        s.SectionName,
                        (SUM(asp1.TutStat) * 100) / (COUNT(t.TutorialID) * 2) AS Completion,
                        SUM(asp1.TutTime) AS TotalTime,
                        s.AverageSectionMins AS AverageTime,
                        cu.IsAssessed AS IsAssessed,
                        s.PLAssessPath AS PostLearningAssessPath,
                        MAX(ISNULL(aa.Score, 0)) AS Outcome,
                        (SELECT COUNT(AssessAttemptID) AS PLAttempts
                            FROM AssessAttempts AS a_a
                            WHERE (ProgressID = @progressId) AND (SectionNumber = s.SectionNumber)) AS Attempts,
                        MAX(ISNULL(CAST(aa.Status AS INT), 0)) AS Passed
                    FROM
                        aspProgress AS asp1
                        INNER JOIN Progress AS p ON asp1.ProgressID = p.ProgressID
                        INNER JOIN Customisations AS cu ON p.CustomisationID = cu.CustomisationID
                        INNER JOIN Applications AS a ON a.ApplicationID = cu.ApplicationID
                        INNER JOIN Sections AS s
                        INNER JOIN Tutorials AS t ON s.SectionID = t.SectionID
                        INNER JOIN CustomisationTutorials AS ct ON t.TutorialID = ct.TutorialID ON asp1.TutorialID = t.TutorialID
                        LEFT OUTER JOIN AssessAttempts AS aa ON asp1.ProgressID = aa.ProgressID AND s.SectionNumber = aa.SectionNumber
                    WHERE
                        (ct.CustomisationID = p.CustomisationID) AND (p.ProgressID = @progressId) AND (s.ArchivedDate IS NULL)
                        AND (ct.Status = 1 OR ct.DiagStatus = 1 OR cu.IsAssessed = 1)
                        AND a.DefaultContentTypeID <> 4
                    GROUP BY
                        s.SectionID,
                        s.ApplicationID,
                        s.SectionNumber,
                        s.SectionName,
                        s.ConsolidationPath,
                        s.DiagAssessPath,
                        s.PLAssessPath,
                        s.AverageSectionMins,
                        cu.IsAssessed,
                        p.CandidateID,
                        p.CustomisationID,
                        p.PLLocked
                    ORDER BY s.SectionNumber",
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
                        INNER JOIN Applications AS a ON a.ApplicationID = c.ApplicationID
                        INNER JOIN TutStatus AS ts
                        INNER JOIN aspProgress AS ap ON ts.TutStatusID = ap.TutStat ON P.ProgressID = ap.ProgressID AND t.TutorialID = ap.TutorialID
                    WHERE (t.SectionID = @sectionID)
                        AND (p.ProgressID = @ProgressID)
                        AND (ct.Status = 1)
                        AND (c.Active = 1)
                        AND (t.ArchivedDate IS NULL)
                        AND a.DefaultContentTypeID <> 4
                    ORDER BY t.TutorialID",
                new { progressId, sectionId }
            );
        }

        public SectionAndApplicationDetailsForAssessAttempts? GetSectionAndApplicationDetailsForAssessAttempts(
            int sectionId,
            int customisationId
        )
        {
            return connection.Query<SectionAndApplicationDetailsForAssessAttempts>(
                @"SELECT s.SectionNumber, a.PLAPassThreshold, a.AssessAttempts
                    FROM dbo.Sections AS s 
                    INNER JOIN dbo.Applications AS a ON a.ApplicationID = s.ApplicationID 
                    INNER JOIN dbo.Customisations AS c ON c.ApplicationID = a.ApplicationID
                    WHERE s.SectionID = @sectionId AND c.CustomisationID = @customisationId
                        AND a.DefaultContentTypeID <> 4",
                new { sectionId, customisationId }
            ).SingleOrDefault();
        }

        public void UpdateCourseAdminFieldForDelegate(
            int progressId,
            int promptNumber,
            string? answer
        )
        {
            connection.Execute(
                $@"UPDATE Progress
                        SET Answer{promptNumber} = @answer
                        WHERE ProgressID = @progressId",
                new { progressId, promptNumber, answer }
            );
        }

        public void UpdateProgressDetailsForStoreAspProgressV2(
            int progressId,
            int customisationVersion,
            DateTime submittedTime,
            string progressText
        )
        {
            connection.Execute(
                @"UPDATE Progress
                    SET
                        CustomisationVersion = @customisationVersion,
                        SubmittedTime = @submittedTime,
                        ProgressText = @progressText,
                        DiagnosticScore =
                            (SELECT CASE WHEN SUM(t.DiagAssessOutOf) > 0
                                THEN CAST((SUM(ap.DiagHigh) * 1.0) / (SUM(t.DiagAssessOutOf) * 1.0) * 100 AS Int)
                                ELSE 0 END AS DiagPercent
                            FROM aspProgress AS ap
                            INNER JOIN Progress AS p ON ap.ProgressID = p.ProgressID
                            INNER JOIN Customisations AS c ON p.CustomisationID = c.CustomisationID
                            INNER JOIN CustomisationTutorials AS ct ON ap.TutorialID = ct.TutorialID AND c.CustomisationID = ct.CustomisationID
                            INNER JOIN Tutorials AS t ON ct.TutorialID = t.TutorialID
                            WHERE ap.ProgressID = @progressId AND ct.DiagStatus = 1)
                    WHERE (ProgressID = @progressId)",
                new { progressId, customisationVersion, submittedTime, progressText }
            );
        }

        public void UpdateAspProgressTutTime(
            int tutorialId,
            int progressId,
            int tutTime
        )
        {
            connection.Execute(
                @"UPDATE aspProgress
                    SET TutTime = TutTime + @tutTime
                    WHERE (TutorialID = @tutorialId) AND (ProgressID = @progressId)",
                new { tutorialId, progressId, tutTime }
            );
        }

        public void UpdateAspProgressTutStat(
            int tutorialId,
            int progressId,
            int tutStat
        )
        {
            connection.Execute(
                @"UPDATE aspProgress
                    SET TutStat = @tutStat
                    WHERE (TutorialID = @tutorialId)
                      AND (ProgressID = @progressId)
                      AND (TutStat < @tutStat)",
                new { tutorialId, progressId, tutStat }
            );
        }

        public int GetCompletionStatusForProgress(int progressId)
        {
            return connection.QuerySingle<int>(
                "GetAndReturnCompletionStatusByProgID",
                new { progressId },
                commandType: CommandType.StoredProcedure
            );
        }

        public IEnumerable<AssessAttempt> GetAssessAttemptsForProgressSection(int progressId, int sectionNumber)
        {
            return connection.Query<AssessAttempt>(
                @"SELECT
                        AssessAttemptID,
                        CandidateID,
                        CustomisationID,
                        CustomisationVersion,
                        Date,
                        AssessInstance,
                        SectionNumber,
                        Score,
                        Status,
                        ProgressId
                    FROM dbo.AssessAttempts
                    WHERE ProgressID = @progressId AND SectionNumber = @sectionNumber",
                new { progressId, sectionNumber }
            );
        }

        public void InsertAssessAttempt(
            int delegateId,
            int customisationId,
            int version,
            DateTime insertionDate,
            int sectionNumber,
            int score,
            bool status,
            int progressId
        )
        {
            connection.Execute(
                @"INSERT INTO AssessAttempts
                        (CandidateID, CustomisationID, CustomisationVersion, Date, AssessInstance, SectionNumber, Score, Status, ProgressID)
                    VALUES (@delegateId, @customisationId, @version, @insertionDate, 1, @sectionNumber, @score, @status, @progressId)",
                new { delegateId, customisationId, version, insertionDate, sectionNumber, score, status, progressId }
            );
        }
    }
}
