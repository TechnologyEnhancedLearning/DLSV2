namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Courses;
    using Microsoft.Extensions.Logging;

    public interface ICourseDataService
    {
        IEnumerable<CurrentCourse> GetCurrentCourses(int candidateId);

        IEnumerable<CompletedCourse> GetCompletedCourses(int candidateId);

        IEnumerable<AvailableCourse> GetAvailableCourses(int candidateId, int? centreId);

        void SetCompleteByDate(int progressId, int candidateId, DateTime? completeByDate);

        void RemoveCurrentCourse(int progressId, int candidateId, RemovalMethod removalMethod);

        void EnrolOnSelfAssessment(int selfAssessmentId, int candidateId);

        int GetNumberOfActiveCoursesAtCentreFilteredByCategory(int centreId, int? categoryId);

        IEnumerable<CourseStatistics> GetCourseStatisticsAtCentreFilteredByCategory(int centreId, int? categoryId);

        IEnumerable<DelegateCourseInfo> GetDelegateCoursesInfo(int delegateId);

        DelegateCourseInfo? GetDelegateCourseInfoByProgressId(int progressId);

        AttemptStats GetDelegateCourseAttemptStats(int delegateId, int customisationId);

        CourseNameInfo? GetCourseNameAndApplication(int customisationId);

        CourseDetails? GetCourseDetailsFilteredByCategory(int customisationId, int centreId, int? categoryId);

        IEnumerable<Course> GetCoursesAvailableToCentreByCategory(int centreId, int? categoryId);

        IEnumerable<Course> GetCoursesEverUsedAtCentreByCategory(int centreId, int? categoryId);

        bool DoesCourseExistAtCentre(int customisationId, int centreId, int? categoryId);

        bool DoesCourseNameExistAtCentre(
            int customisationId,
            string customisationName,
            int centreId,
            int applicationId
        );

        void UpdateLearningPathwayDefaultsForCourse(
            int customisationId,
            int completeWithinMonths,
            int validityMonths,
            bool mandatory,
            bool autoRefresh,
            int refreshToCustomisationId,
            int autoRefreshMonths,
            bool applyLpDefaultsToSelfEnrol
        );

        public void UpdateCourseDetails(
            int customisationId,
            string customisationName,
            string password,
            string notificationEmails,
            bool isAssessed,
            int tutCompletionThreshold,
            int diagCompletionThreshold
        );

        void UpdateCourseOptions(CourseOptions courseOptions, int customisationId);

        CourseOptions? GetCourseOptionsFilteredByCategory(int customisationId, int centreId, int? categoryId);

        public (int? centreId, int? courseCategoryId) GetCourseValidationDetails(int customisationId);
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

        private const string SelectDelegateCourseInfoQuery =
            @"SELECT
                pr.ProgressId,
                cu.CustomisationID AS CustomisationId,
                cu.CentreID AS CustomisationCentreId,
                cu.Active AS IsCourseActive,
                cu.AllCentres AS AllCentresCourse,
                ap.CourseCategoryID,
                ap.ApplicationName,
                cu.CustomisationName,
                auSupervisor.AdminID AS SupervisorAdminId,
                auSupervisor.Forename AS SupervisorForename,
                auSupervisor.Surname AS SupervisorSurname,
                pr.FirstSubmittedTime AS Enrolled,
                pr.SubmittedTime AS LastUpdated,
                pr.CompleteByDate AS CompleteBy,
                pr.Completed AS Completed,
                pr.Evaluated AS Evaluated,
                pr.RemovedDate,
                pr.EnrollmentMethodID AS EnrolmentMethodId,
                auEnrolledBy.AdminID AS EnrolledByAdminId,
                auEnrolledBy.Forename AS EnrolledByForename,
                auEnrolledBy.Surname AS EnrolledBySurname,
                pr.LoginCount,
                pr.Duration AS LearningTime,
                pr.DiagnosticScore,
                cu.IsAssessed,
                pr.Answer1,
                pr.Answer2,
                pr.Answer3,
                ca.CandidateID AS DelegateId,
                ca.FirstName AS DelegateFirstName,
                ca.LastName AS DelegateLastName,
                ca.EmailAddress AS DelegateEmail,
                ca.CentreID AS DelegateCentreId
            FROM Customisations cu
            INNER JOIN Applications ap ON ap.ApplicationID = cu.ApplicationID
            INNER JOIN Progress pr ON pr.CustomisationID = cu.CustomisationID
            LEFT OUTER JOIN AdminUsers auSupervisor ON auSupervisor.AdminID = pr.SupervisorAdminId
            LEFT OUTER JOIN AdminUsers auEnrolledBy ON auEnrolledBy.AdminID = pr.EnrolledByAdminID
            INNER JOIN Candidates AS ca ON ca.CandidateID = pr.CandidateID";

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

        public void RemoveCurrentCourse(int progressId, int candidateId, RemovalMethod removalMethod)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE Progress
                    SET RemovedDate = getUTCDate(),
                        RemovalMethodID = @removalMethod
                    WHERE ProgressID = @progressId
                      AND CandidateID = @candidateId
                ",
                new { progressId, candidateId, removalMethod }
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

        public int GetNumberOfActiveCoursesAtCentreFilteredByCategory(int centreId, int? adminCategoryId)
        {
            return (int)connection.ExecuteScalar(
                @"SELECT COUNT(*)
                        FROM Customisations AS c
                        JOIN Applications AS a on a.ApplicationID = c.ApplicationID
                        WHERE Active = 1 AND CentreID = @centreId
	                    AND (a.CourseCategoryID = @adminCategoryId OR @adminCategoryId IS NULL)",
                new { centreId, adminCategoryId }
            );
        }

        public IEnumerable<CourseStatistics> GetCourseStatisticsAtCentreFilteredByCategory(
            int centreId,
            int? categoryId
        )
        {
            return connection.Query<CourseStatistics>(
                @$"SELECT
                        cu.CustomisationID,
                        cu.CentreID,
                        cu.Active,
                        cu.AllCentres,
                        ap.ApplicationId,
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
                    WHERE (ap.CourseCategoryID = @categoryId OR @categoryId IS NULL)
                        AND (cu.CentreID = @centreId OR (cu.AllCentres = 1 AND ca.Active = 1))
                        AND ca.CentreID = @centreId
                        AND ap.ArchivedDate IS NULL",
                new { centreId, categoryId }
            );
        }

        public IEnumerable<DelegateCourseInfo> GetDelegateCoursesInfo(int delegateId)
        {
            return connection.Query<DelegateCourseInfo>(
                $@"{SelectDelegateCourseInfoQuery}
                    WHERE pr.CandidateID = @delegateId
                        AND ap.ArchivedDate IS NULL
                        AND pr.RemovedDate IS NULL",
                new { delegateId }
            );
        }

        public DelegateCourseInfo? GetDelegateCourseInfoByProgressId(int progressId)
        {
            return connection.QuerySingleOrDefault<DelegateCourseInfo>(
                $@"{SelectDelegateCourseInfoQuery}
                    WHERE pr.ProgressID = @progressId
                        AND ap.ArchivedDate IS NULL",
                new { progressId }
            );
        }

        public AttemptStats GetDelegateCourseAttemptStats(
            int delegateId,
            int customisationId
        )
        {
            var (totalAttempts, attemptsPassed) = connection.QueryFirstOrDefault<(int, int)>(
                @"SELECT COUNT(aa.Status) AS TotalAttempts,
                        COUNT(CASE WHEN aa.Status=1 THEN 1 END) AS AttemptsPassed
                    FROM AssessAttempts aa
                    INNER JOIN Progress AS pr ON pr.ProgressID = aa.ProgressID
                    WHERE pr.CustomisationID = @customisationId
                        AND pr.CandidateID = @delegateId
                        AND pr.RemovedDate IS NULL",
                new { delegateId, customisationId }
            );

            return new AttemptStats(totalAttempts, attemptsPassed);
        }

        public CourseDetails? GetCourseDetailsFilteredByCategory(int customisationId, int centreId, int? categoryId)
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
                        (ap.CourseCategoryID = @categoryId OR @categoryId IS NULL)
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

        public IEnumerable<Course> GetCoursesAvailableToCentreByCategory(int centreId, int? categoryId)
        {
            return connection.Query<Course>(
                @"SELECT
                        c.CustomisationID,
                        c.CentreID,
                        c.ApplicationID,
                        ap.ApplicationName,
                        c.CustomisationName,
                        c.Active
                    FROM Customisations AS c
                    INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = c.ApplicationID
                    INNER JOIN dbo.CentreApplications AS ca ON ca.ApplicationID = ap.ApplicationID
                    WHERE (c.CentreID = @centreId OR c.AllCentres = 1)
                    AND ca.CentreID = @centreID
	                AND (ap.CourseCategoryID = @categoryId OR @categoryId IS NULL)
                    AND ap.ArchivedDate IS NULL",
                new { centreId, categoryId }
            );
        }

        public IEnumerable<Course> GetCoursesEverUsedAtCentreByCategory(int centreId, int? categoryId)
        {
            return connection.Query<Course>(
                @"SELECT DISTINCT
                        c.CustomisationID,
                        c.CentreID,
                        c.ApplicationID,
                        ap.ApplicationName,
                        c.CustomisationName,
                        c.Active
                    FROM Candidates AS cn
                    INNER JOIN Progress AS p ON p.CandidateID = cn.CandidateID
                    INNER JOIN Customisations AS c ON c.CustomisationID = p.CustomisationId
                    INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = c.ApplicationID
                    WHERE cn.CentreID = @centreID
	                AND (ap.CourseCategoryID = @categoryId OR @categoryId IS NULL)
                    AND ap.ArchivedDate IS NULL",
                new { centreId, categoryId }
            );
        }

        public bool DoesCourseExistAtCentre(int customisationId, int centreId, int? categoryId)
        {
            return connection.ExecuteScalar<bool>(
                @"SELECT CASE WHEN EXISTS (
                        SELECT *
                        FROM Customisations AS c
                        JOIN Applications AS a on a.ApplicationID = c.ApplicationID
                        WHERE CustomisationID = @customisationId
                        AND c.CentreID = @centreId
                        AND (a.CourseCategoryID = @categoryId OR @categoryId IS NULL)
                    )
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT) END",
                new { customisationId, centreId, categoryId }
            );
        }

        public bool DoesCourseNameExistAtCentre(
            int customisationId,
            string customisationName,
            int centreId,
            int applicationId
        )
        {
            return connection.ExecuteScalar<bool>(
                @"SELECT CASE WHEN EXISTS (
                        SELECT CustomisationID
                        FROM dbo.Customisations
                        WHERE [ApplicationID] = @applicationID
                        AND [CentreID] = @centreID
                        AND [CustomisationName] = @customisationName
                        AND [CustomisationID] != @customisationId)
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT) END",
                new { customisationId, customisationName, centreId, applicationId }
            );
        }

        public (int? centreId, int? courseCategoryId) GetCourseValidationDetails(int customisationId)
        {
            return connection.QueryFirstOrDefault<(int?, int?)>(
                @"SELECT c.CentreId, a.CourseCategoryId
                        FROM Customisations AS c
                        INNER JOIN Applications AS a on a.ApplicationID = c.ApplicationID
                        WHERE CustomisationID = @customisationId",
                new { customisationId }
            );
        }

        public void UpdateLearningPathwayDefaultsForCourse(
            int customisationId,
            int completeWithinMonths,
            int validityMonths,
            bool mandatory,
            bool autoRefresh,
            int refreshToCustomisationId,
            int autoRefreshMonths,
            bool applyLpDefaultsToSelfEnrol
        )
        {
            connection.Execute(
                @"UPDATE Customisations
                    SET
                        CompleteWithinMonths = @completeWithinMonths,
                        ValidityMonths = @validityMonths,
                        Mandatory = @mandatory,
                        AutoRefresh = @autoRefresh,
                        RefreshToCustomisationID = @refreshToCustomisationId,
                        AutoRefreshMonths = @autoRefreshMonths,
                        ApplyLpDefaultsToSelfEnrol = @applyLpDefaultsToSelfEnrol
                    WHERE CustomisationID = @customisationId",
                new
                {
                    completeWithinMonths,
                    validityMonths,
                    mandatory,
                    autoRefresh,
                    customisationId,
                    refreshToCustomisationId,
                    autoRefreshMonths,
                    applyLpDefaultsToSelfEnrol,
                }
            );
        }

        public void UpdateCourseDetails(
            int customisationId,
            string customisationName,
            string? password,
            string? notificationEmails,
            bool isAssessed,
            int tutCompletionThreshold,
            int diagCompletionThreshold
        )
        {
            connection.Execute(
                @"UPDATE Customisations
                    SET
                        CustomisationName = @customisationName,
                        Password = @password,
                        NotificationEmails = @notificationEmails,
                        IsAssessed = @isAssessed,
                        TutCompletionThreshold = @tutCompletionThreshold,
                        DiagCompletionThreshold = @diagCompletionThreshold
                    WHERE CustomisationID = @customisationId",
                new
                {
                    customisationName,
                    password,
                    notificationEmails,
                    isAssessed,
                    tutCompletionThreshold,
                    diagCompletionThreshold,
                    customisationId,
                }
            );
        }

        public void UpdateCourseOptions(CourseOptions courseOptions, int customisationId)
        {
            connection.Execute(
                @"UPDATE cu
                    SET Active = @Active,
                        SelfRegister = @SelfRegister,
                        HideInLearnerPortal = @HideInLearnerPortal,
                        DiagObjSelect = @DiagObjSelect
                    FROM dbo.Customisations AS cu
                    WHERE
                    cu.CustomisationID = @customisationId",
                new
                {
                    courseOptions.Active,
                    courseOptions.SelfRegister,
                    courseOptions.HideInLearnerPortal,
                    courseOptions.DiagObjSelect,
                    customisationId,
                }
            );
        }

        public CourseOptions? GetCourseOptionsFilteredByCategory(int customisationId, int centreId, int? categoryId)
        {
            return connection.Query<CourseOptions>(
                @"SELECT
                        cu.Active,
                        cu.SelfRegister,
                        cu.HideInLearnerPortal,
                        cu.DiagObjSelect,
                        ap.DiagAssess
                    FROM dbo.Customisations AS cu
                    INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = cu.ApplicationID
                    LEFT JOIN dbo.Customisations AS refreshToCu ON refreshToCu.CustomisationID = cu.RefreshToCustomisationId
                    LEFT JOIN dbo.Applications AS refreshToAp ON refreshToAp.ApplicationID = refreshToCu.ApplicationID
                    WHERE
                        (ap.CourseCategoryID = @categoryId OR @categoryId IS NULL)
                        AND cu.CentreID = @centreId
                        AND ap.ArchivedDate IS NULL
                        AND cu.CustomisationID = @customisationId",
                new { customisationId, centreId, categoryId }
            ).FirstOrDefault();
        }
    }
}
