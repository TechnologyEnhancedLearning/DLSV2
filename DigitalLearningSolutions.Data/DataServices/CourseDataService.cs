namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Courses;
    using Microsoft.Extensions.Logging;

    public interface ICourseDataService
    {
        IEnumerable<CurrentCourse> GetCurrentCourses(int candidateId);
        IEnumerable<CompletedCourse> GetCompletedCourses(int candidateId);
        IEnumerable<AvailableCourse> GetAvailableCourses(int candidateId, int? centreId);
        void SetCompleteByDate(int progressId, int candidateId, DateTime? completeByDate);
        void RemoveCurrentCourse(int progressId, int candidateId);
        void EnrolOnSelfAssessment(int selfAssessmentId, int candidateId);
        int GetNumberOfActiveCoursesAtCentreForCategory(int centreId, int categoryId);
        IEnumerable<CourseStatistics> GetCourseStatisticsAtCentreForCategoryId(int centreId, int categoryId);
        IEnumerable<DelegateCourseInfo> GetDelegateCoursesInfo(int delegateId);
        (int totalAttempts, int attemptsPassed) GetDelegateCourseAttemptStats(int delegateId, int customisationId);
        CourseDetails? GetCourseDetails(int customisationId, int centreId, int categoryId);
        CourseNameInfo? GetCourseNameAndApplication(int customisationId);
    }

    public class CourseDataService : ICourseDataService
    {
        private const string DelegateCountQuery =
            @"(SELECT COUNT(pr.CandidateID)
                FROM dbo.Progress AS pr
                INNER JOIN dbo.Candidates AS can ON can.CandidateID = pr.CandidateID 
                WHERE pr.CustomisationID = cu.CustomisationID
                AND can.CentreID = @centreId
                AND RemovedDate IS NULL) AS DelegateCount";

        private const string CompletedCountQuery =
            @"(SELECT COUNT(pr.CandidateID)
                FROM dbo.Progress AS pr
                INNER JOIN dbo.Candidates AS can ON can.CandidateID = pr.CandidateID 
                WHERE pr.CustomisationID = cu.CustomisationID AND pr.Completed IS NOT NULL
                AND can.CentreID = @centreId) AS CompletedCount";

        private const string AllAttemptsQuery =
            @"(SELECT COUNT(aa.AssessAttemptID)
                FROM dbo.AssessAttempts AS aa
                INNER JOIN dbo.Candidates AS can ON can.CandidateID = aa.CandidateID
                WHERE aa.CustomisationID = cu.CustomisationID AND aa.[Status] IS NOT NULL
                AND can.CentreID = @centreId) AS AllAttempts";

        private const string AttemptsPassedQuery =
            @"(SELECT COUNT(aa.AssessAttemptID)
                FROM dbo.AssessAttempts AS aa
                INNER JOIN dbo.Candidates AS can ON can.CandidateID = aa.CandidateID
                WHERE aa.CustomisationID = cu.CustomisationID AND aa.[Status] = 1
                AND can.CentreID = @centreId) AS AttemptsPassed";

        private const string LastAccessedQuery =
            @"(SELECT TOP 1 SubmittedTime
                FROM dbo.Progress AS pr
                INNER JOIN dbo.Candidates AS can ON can.CandidateID = pr.CandidateID
                WHERE pr.CustomisationID = cu.CustomisationID
                AND can.CentreID = @centreId
                AND RemovedDate IS NULL
                ORDER BY SubmittedTime DESC) AS LastAccessed";

        private readonly IDbConnection connection;
        private readonly ILogger<CourseDataService> logger;

        public CourseDataService(IDbConnection connection, ILogger<CourseDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public IEnumerable<CurrentCourse> GetCurrentCourses(int candidateId)
        {
            return connection.Query<CurrentCourse>(
                "GetCurrentCoursesForCandidate_V2",
                new { candidateId },
                commandType: CommandType.StoredProcedure
            );
        }

        public IEnumerable<CompletedCourse> GetCompletedCourses(int candidateId)
        {
            return connection.Query<CompletedCourse>(
                "GetCompletedCoursesForCandidate",
                new { candidateId },
                commandType: CommandType.StoredProcedure
            );
        }

        public IEnumerable<AvailableCourse> GetAvailableCourses(int candidateId, int? centreId)
        {
            return connection.Query<AvailableCourse>(
                @"GetActiveAvailableCustomisationsForCentreFiltered_V5",
                new { candidateId, centreId },
                commandType: CommandType.StoredProcedure
            );
        }

        public void SetCompleteByDate(int progressId, int candidateId, DateTime? completeByDate)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE Progress
                        SET CompleteByDate = @date
                        WHERE ProgressID = @progressId
                          AND CandidateID = @candidateId",
                new { date = completeByDate, progressId, candidateId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not setting current course complete by date as db update failed. " +
                    $"Progress id: {progressId}, candidate id: {candidateId}, complete by date: {completeByDate}"
                );
            }
        }

        public void RemoveCurrentCourse(int progressId, int candidateId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE Progress
                    SET RemovedDate = getUTCDate(),
                        RemovalMethodID = 1
                    WHERE ProgressID = @progressId
                      AND CandidateID = @candidateId
                ",
                new { progressId, candidateId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    $"Not removing current course as db update failed. Progress id: {progressId}, candidate id: {candidateId}"
                );
            }
        }

        public void EnrolOnSelfAssessment(int selfAssessmentId, int candidateId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"INSERT INTO [dbo].[CandidateAssessments]
                           ([CandidateID]
                           ,[SelfAssessmentID])
                     VALUES
                           (@candidateId,
                           @selfAssessmentId)",
                new { selfAssessmentId, candidateId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not enrolled delegate on self assessment as db insert failed. " +
                    $"Self assessment id: {selfAssessmentId}, candidate id: {candidateId}"
                );
            }
        }

        public int GetNumberOfActiveCoursesAtCentreForCategory(int centreId, int adminCategoryId)
        {
            return (int)connection.ExecuteScalar(
                @"SELECT COUNT(*)
                        FROM Customisations AS c
                        JOIN Applications AS a on a.ApplicationID = c.ApplicationID
                        WHERE Active = 1 AND CentreID = @centreId 
	                    AND (a.CourseCategoryID = @adminCategoryId OR @adminCategoryId = 0)",
                new { centreId, adminCategoryId }
            );
        }

        public IEnumerable<CourseStatistics> GetCourseStatisticsAtCentreForCategoryId(int centreId, int categoryId)
        {
            return connection.Query<CourseStatistics>(
                @$"SELECT
                        cu.CustomisationID,
                        cu.CentreID,
                        cu.Active,
                        cu.AllCentres,
                        ap.ApplicationName,
                        cu.CustomisationName,
                        {DelegateCountQuery},
                        {CompletedCountQuery},
                        {AllAttemptsQuery},
                        {AttemptsPassedQuery},
                        cu.HideInLearnerPortal,
                        cc.CategoryName,
                        ct.CourseTopic,
                        cu.LearningTimeMins AS LearningMinutes
                    FROM dbo.Customisations AS cu
                    INNER JOIN dbo.CentreApplications AS ca ON ca.ApplicationID = cu.ApplicationID
                    INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = ca.ApplicationID
                    INNER JOIN dbo.CourseCategories AS cc ON cc.CourseCategoryID = ap.CourseCategoryID
                    INNER JOIN dbo.CourseTopics AS ct ON ct.CourseTopicID = ap.CourseTopicId
                    WHERE (ap.CourseCategoryID = @categoryId OR @categoryId = 0) 
                        AND (cu.CentreID = @centreId OR (cu.AllCentres = 1 AND ca.Active = 1))
                        AND ca.CentreID = @centreId
                        AND ap.ArchivedDate IS NULL",
                new { centreId, categoryId }
            );
        }

        public IEnumerable<DelegateCourseInfo> GetDelegateCoursesInfo(int delegateId)
        {
            return connection.Query<DelegateCourseInfo>(
                @"SELECT
                        cu.CustomisationID AS CustomisationId,
                        ap.ApplicationName,
                        cu.CustomisationName,
                        au.Forename AS SupervisorForename,
                        au.Surname AS SupervisorSurname,
                        pr.FirstSubmittedTime AS Enrolled,
                        pr.SubmittedTime AS LastUpdated,
                        pr.CompleteByDate AS CompleteBy,
                        pr.Completed AS Completed,
                        pr.Evaluated AS Evaluated,
                        pr.EnrollmentMethodID AS EnrolmentMethodId,
                        pr.LoginCount,
                        pr.Duration AS LearningTime,
                        pr.DiagnosticScore,
                        cu.IsAssessed,
                        pr.Answer1,
                        pr.Answer2,
                        pr.Answer3
                    FROM Customisations cu
                    INNER JOIN Applications ap ON ap.ApplicationID = cu.ApplicationID
                    INNER JOIN Progress pr ON pr.CustomisationID = cu.CustomisationID
                    LEFT OUTER JOIN AdminUsers au ON au.AdminID = pr.SupervisorAdminId
                    WHERE pr.CandidateID = @delegateId
                        AND ap.ArchivedDate IS NULL
                        AND pr.RemovedDate IS NULL",
                new { delegateId }
            );
        }

        public (int totalAttempts, int attemptsPassed) GetDelegateCourseAttemptStats(
            int delegateId,
            int customisationId
        )
        {
            return connection.QueryFirstOrDefault<(int, int)>(
                @"SELECT COUNT(aa.Status) AS TotalAttempts,
                        COUNT(CASE WHEN aa.Status=1 THEN 1 END) AS AttemptsPassed
                    FROM AssessAttempts aa
                    INNER JOIN Progress AS pr ON pr.ProgressID = aa.ProgressID
                    WHERE pr.CustomisationID = @customisationId
                        AND pr.CandidateID = @delegateId
                        AND pr.RemovedDate IS NULL",
                new { delegateId, customisationId }
            );
        }

        public CourseDetails? GetCourseDetails(int customisationId, int centreId, int categoryId)
        {
            return connection.Query<CourseDetails>(
                @$"SELECT
                        cu.CustomisationID,
                        cu.CentreID,
                        cu.Active,
                        cu.ApplicationID,
                        ap.ApplicationName,
                        cu.CustomisationName,
                        cu.CurrentVersion,
                        cu.CreatedDate,
                        cu.[Password],
                        cu.NotificationEmails,
                        ap.PLAssess AS PostLearningAssessment,
                        cu.IsAssessed,
                        ap.DiagAssess,
                        cu.TutCompletionThreshold,
                        cu.DiagCompletionThreshold,
                        cu.SelfRegister,
                        cu.DiagObjSelect,
                        cu.HideInLearnerPortal,
                        cu.CompleteWithinMonths,
                        cu.ValidityMonths,
                        cu.Mandatory,
                        cu.AutoRefresh,
                        cu.RefreshToCustomisationID,
                        refreshToCu.CustomisationName AS refreshToCustomisationName,
                        refreshToAp.ApplicationName AS refreshToApplicationName,
                        cu.AutoRefreshMonths,
                        cu.ApplyLPDefaultsToSelfEnrol,
                        {LastAccessedQuery},
                        {DelegateCountQuery},
                        {CompletedCountQuery}
                    FROM dbo.Customisations AS cu
                    INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = cu.ApplicationID
                    LEFT JOIN dbo.Customisations AS refreshToCu ON refreshToCu.CustomisationID = cu.RefreshToCustomisationId
                    LEFT JOIN dbo.Applications AS refreshToAp ON refreshToAp.ApplicationID = refreshToCu.ApplicationID
                    WHERE
                        (ap.CourseCategoryID = @categoryId OR @categoryId = 0) 
                        AND cu.CentreID = @centreId
                        AND ap.ArchivedDate IS NULL 
                        AND cu.CustomisationID = @customisationId",
                new { customisationId, centreId, categoryId }
            ).FirstOrDefault();
        }

        public CourseNameInfo? GetCourseNameAndApplication(int customisationId)
        {
            var names = connection.QueryFirstOrDefault<CourseNameInfo>(
                @"SELECT cu.CustomisationName, ap.ApplicationName
                        FROM Customisations cu
                        JOIN Applications ap ON cu.ApplicationId = ap.ApplicationId 
                        WHERE cu.CustomisationId = @customisationId",
                new { customisationId }
            );
            if (names == null)
            {
                logger.LogWarning(
                    $"No customisation found for customisation id {customisationId}"
                );
            }

            return names;
        }
    }
}
