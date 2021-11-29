namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;

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
    }

    public class ProgressDataService : IProgressDataService
    {
        private readonly IDbConnection connection;

        public ProgressDataService(IDbConnection connection)
        {
            this.connection = connection;
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
                    supervisorAdminId
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
    }
}
