namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
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
                @"UPDATE aspProgress SET
                                        DiagHigh = MAX(@myScore, DiagHigh),
                                        DiagLow = MAX(@myScore, DiagLow),
                                        DiagLast = MAX(@diagLast,
                                        DiagAttempts = DiagAttempts + 1
                                    WHERE ProgressID = @progressId
                                    AND TutorialID = @tutorialId OR OriginalTutorialID = @tutorialId",
                new { progressId, tutorialId, myScore }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating diagnostic score as db update failed. " +
                    $"Progress id: {progressId}, tutorial Id: {tutorialId}, myScore: {myScore}"
                );
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
    }
}
