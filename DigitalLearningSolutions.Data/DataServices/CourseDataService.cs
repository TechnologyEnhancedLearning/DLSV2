namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public interface ICourseDataService
    {
        IEnumerable<CurrentCourse> GetCurrentCourses(int candidateId);

        IEnumerable<CompletedCourse> GetCompletedCourses(int candidateId);

        IEnumerable<AvailableCourse> GetAvailableCourses(int candidateId, int? centreId);

        IEnumerable<AvailableCourse> GetAvailableCourses(int delegateId, int? centreId, int categoryId);

        void SetCompleteByDate(int progressId, int candidateId, DateTime? completeByDate);

        void RemoveCurrentCourse(int progressId, int candidateId, RemovalMethod removalMethod);

        void EnrolOnSelfAssessment(int selfAssessmentId, int delegateUserId, int centreId);

        int EnrolSelfAssessment(int selfAssessmentId, int delegateUserId, int centreId);

        int GetNumberOfActiveCoursesAtCentreFilteredByCategory(int centreId, int? categoryId);

        IEnumerable<CourseStatistics> GetCourseStatisticsAtCentreFilteredByCategory(int centreId, int? categoryId);

        public IEnumerable<CourseStatistics> GetCourseStatisticsAtCentreFilteredByCategory(
            int centreId,
            int? categoryId,
            int exportQueryRowLimit,
            int currentRun,
            string? searchString,
            string? sortBy,
            string? filterString,
            string sortDirection
        );

        public int GetCourseStatisticsAtCentreFilteredByCategoryResultCount(
            int centreId,
            int? categoryId,
            string? searchString
        );

        IEnumerable<CourseStatistics> GetNonArchivedCourseStatisticsAtCentreFilteredByCategory(int centreId, int? categoryId);

        IEnumerable<DelegateCourseInfo> GetDelegateCoursesInfo(int delegateId);

        DelegateCourseInfo? GetDelegateCourseInfoByProgressId(int progressId);

        CourseNameInfo? GetCourseNameAndApplication(int customisationId);

        bool GetSelfRegister(int customisationId);

        CourseDetails? GetCourseDetailsFilteredByCategory(int customisationId, int centreId, int? categoryId);

        IEnumerable<CourseAssessmentDetails> GetCoursesAvailableToCentreByCategory(int centreId, int? categoryId);

        IEnumerable<CourseAssessmentDetails> GetNonArchivedCoursesAvailableToCentreByCategory(int centreId, int? categoryId);

        IEnumerable<ApplicationDetails> GetApplicationsAvailableToCentreByCategory(int centreId, int? categoryId);

        IEnumerable<ApplicationDetails> GetApplicationsByBrandId(int brandId);

        Dictionary<int, int> GetNumsOfRecentProgressRecordsForBrand(int brandId, DateTime threeMonthsAgo);

        IEnumerable<Course> GetCoursesEverUsedAtCentreByCategory(int centreId, int? categoryId);

        bool DoesCourseNameExistAtCentre(
            string customisationName,
            int centreId,
            int applicationId,
            int customisationId = 0
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

        public CourseValidationDetails? GetCourseValidationDetails(int customisationId, int centreId);

        int CreateNewCentreCourse(Customisation customisation);

        IEnumerable<DelegateCourseInfo> GetDelegateCourseInfosForCourse(int customisationId, int centreId);

        IEnumerable<CourseDelegateForExport> GetDelegatesOnCourseForExport(int customisationId, int centreId);

        int GetCourseDelegatesCountForExport(string searchString, string sortBy, string sortDirection,
            int customisationId, int centreId, bool? isDelegateActive, bool? isProgressLocked, bool? removed, bool? hasCompleted, string? answer1, string? answer2, string? answer3);

        IEnumerable<CourseDelegateForExport> GetCourseDelegatesForExport(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection,
            int customisationId, int centreId, bool? isDelegateActive, bool? isProgressLocked, bool? removed, bool? hasCompleted, string? answer1, string? answer2, string? answer3);

        int EnrolOnActivitySelfAssessment(int selfAssessmentId, int candidateId, int supervisorId, string adminEmail,
            int selfAssessmentSupervisorRoleId, DateTime? completeByDate, int delegateUserId, int centreId);

        bool IsCourseCompleted(int candidateId, int customisationId);

        public IEnumerable<Course> GetApplicationsAvailableToCentre(int centreId);

        public (IEnumerable<CourseStatistics>, int) GetCourseStatisticsAtCentre(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection, int centreId, int? categoryId, bool allCentreCourses, bool? hideInLearnerPortal,
           string isActive, string categoryName, string courseTopic, string hasAdminFields);

        public (IEnumerable<DelegateCourseInfo>, int) GetDelegateCourseInfosPerPageForCourse(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection,
            int customisationId, int centreId, bool? isDelegateActive, bool? isProgressLocked, bool? removed, bool? hasCompleted, string? answer1, string? answer2, string? answer3);

        public IEnumerable<CourseStatistics> GetDelegateCourseStatisticsAtCentre(string searchString, int centreId, int? categoryId, bool allCentreCourses, bool? hideInLearnerPortal, string isActive, string categoryName, string courseTopic, string hasAdminFields);

        public IEnumerable<DelegateAssessmentStatistics> GetDelegateAssessmentStatisticsAtCentre(string searchString, int centreId, string categoryName, string isActive);
    }

    public class CourseDataService : ICourseDataService
    {
        private const string DelegateCountQuery =
            @"(SELECT COUNT(pr.CandidateID)
                FROM dbo.Progress AS pr WITH (NOLOCK)
                INNER JOIN dbo.Candidates AS can WITH (NOLOCK) ON can.CandidateID = pr.CandidateID
                WHERE pr.CustomisationID = cu.CustomisationID
                AND can.CentreID = @centreId
                AND RemovedDate IS NULL) AS DelegateCount";

        private const string CompletedCountQuery =
            @"(SELECT COUNT(pr.CandidateID)
                FROM dbo.Progress AS pr WITH (NOLOCK) 
                INNER JOIN dbo.Candidates AS can WITH (NOLOCK) ON can.CandidateID = pr.CandidateID
                WHERE pr.CustomisationID = cu.CustomisationID AND pr.Completed IS NOT NULL
                AND can.CentreID = @centreId) AS CompletedCount";

        private const string AllAttemptsQuery =
            @"(SELECT COUNT(aa.AssessAttemptID)
                FROM dbo.AssessAttempts AS aa WITH (NOLOCK) 
                INNER JOIN dbo.Candidates AS can WITH (NOLOCK) ON can.CandidateID = aa.CandidateID
                WHERE aa.CustomisationID = cu.CustomisationID AND aa.[Status] IS NOT NULL
                AND can.CentreID = @centreId) AS AllAttempts";

        private const string AttemptsPassedQuery =
            @"(SELECT COUNT(aa.AssessAttemptID)
                FROM dbo.AssessAttempts AS aa WITH (NOLOCK) 
                INNER JOIN dbo.Candidates AS can WITH (NOLOCK) ON can.CandidateID = aa.CandidateID
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

        private const string DelegateAllAttemptsQuery =
            @"(SELECT COUNT(aa.AssessAttemptID)
                FROM dbo.AssessAttempts AS aa WITH (NOLOCK)
                INNER JOIN dbo.DelegateAccounts AS dacc WITH (NOLOCK) ON dacc.ID = aa.CandidateID
                WHERE aa.CustomisationID = cu.CustomisationID AND aa.[Status] IS NOT NULL
                AND dacc.ID = da.ID) AS AllAttempts";

        private const string DelegateAttemptsPassedQuery =
            @"(SELECT COUNT(aa.AssessAttemptID)
                FROM dbo.AssessAttempts AS aa WITH (NOLOCK)
                INNER JOIN dbo.DelegateAccounts AS dacc WITH (NOLOCK) ON dacc.ID = aa.CandidateID
                WHERE aa.CustomisationID = cu.CustomisationID AND aa.[Status] = 1
                AND dacc.ID = da.ID) AS AttemptsPassed";

        private const string DelegatePassRateQuery =
            @"CASE
					WHEN (SELECT COUNT(aa.AssessAttemptID)
                        FROM dbo.AssessAttempts AS aa WITH (NOLOCK)
                        INNER JOIN dbo.DelegateAccounts AS dacc WITH (NOLOCK) ON dacc.ID = aa.CandidateID
                        WHERE aa.CustomisationID = cu.CustomisationID AND aa.[Status] IS NOT NULL
                        AND dacc.ID = da.ID) = 0 THEN 0
					ELSE ROUND((100 * (CAST((SELECT COUNT(aa.AssessAttemptID)
                        FROM dbo.AssessAttempts AS aa WITH (NOLOCK)
                        INNER JOIN dbo.DelegateAccounts AS dacc WITH (NOLOCK) ON dacc.ID = aa.CandidateID
                        WHERE aa.CustomisationID = cu.CustomisationID AND aa.[Status] = 1
                        AND dacc.ID = da.ID) AS FLOAT) / (SELECT COUNT(aa.AssessAttemptID)
                        FROM dbo.AssessAttempts AS aa WITH (NOLOCK)
                        INNER JOIN dbo.DelegateAccounts AS dacc WITH (NOLOCK) ON dacc.ID = aa.CandidateID
                        WHERE aa.CustomisationID = cu.CustomisationID AND aa.[Status] IS NOT NULL
                        AND dacc.ID = da.ID))),2,0)
			END AS PassRate";

        private const string TutorialWithLearningCountQuery =
            @"SELECT COUNT(ct.TutorialID)
                FROM CustomisationTutorials AS ct
                INNER JOIN Tutorials AS t ON ct.TutorialID = t.TutorialID
                WHERE ct.Status = 1 AND ct.CustomisationID = c.CustomisationID";

        private const string TutorialWithDiagnosticCountQuery =
            @"SELECT COUNT(ct.TutorialID)
                FROM CustomisationTutorials AS ct
                INNER JOIN Tutorials AS t ON ct.TutorialID = t.TutorialID
                INNER JOIN Customisations AS c ON c.CustomisationID = ct.CustomisationID
                INNER JOIN Applications AS a ON a.ApplicationID = c.ApplicationID
                WHERE ct.DiagStatus = 1 AND a.DiagAssess = 1 AND ct.CustomisationID = c.CustomisationID
                    AND a.ArchivedDate IS NULL AND a.DefaultContentTypeID <> 4";

        private readonly IDbConnection connection;
        private readonly ILogger<CourseDataService> logger;
        private readonly ISelfAssessmentDataService selfAssessmentDataService;

        private readonly string CourseStatisticsQuery = @$"SELECT
                        cu.CustomisationID,
                        cu.CentreID,
                        cu.Active,
                        CASE WHEN ap.ArchivedDate IS NOT NULL THEN 0 ELSE cu.Active END AS Active,
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
                        cu.LearningTimeMins AS LearningMinutes,
                        cu.IsAssessed,
                        CASE WHEN ap.ArchivedDate IS NOT NULL THEN 1 ELSE 0 END AS Archived,
                        ((SELECT COUNT(pr.CandidateID)
		                    FROM dbo.Progress AS pr WITH (NOLOCK) 
		                    INNER JOIN dbo.Candidates AS can WITH (NOLOCK) ON can.CandidateID = pr.CandidateID
		                    WHERE pr.CustomisationID = cu.CustomisationID
		                    AND can.CentreID = @centreId
		                    AND RemovedDate IS NULL) - 
		                (SELECT COUNT(pr.CandidateID)
		                    FROM dbo.Progress AS pr WITH (NOLOCK) 
		                    INNER JOIN dbo.Candidates AS can WITH (NOLOCK) ON can.CandidateID = pr.CandidateID
		                    WHERE pr.CustomisationID = cu.CustomisationID AND pr.Completed IS NOT NULL
		                    AND can.CentreID = @centreId)) AS InProgressCount 
                    FROM dbo.Customisations AS cu
                    INNER JOIN dbo.CentreApplications AS ca ON ca.ApplicationID = cu.ApplicationID
                    INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = ca.ApplicationID
                    INNER JOIN dbo.CourseCategories AS cc ON cc.CourseCategoryID = ap.CourseCategoryID
                    INNER JOIN dbo.CourseTopics AS ct ON ct.CourseTopicID = ap.CourseTopicId
                    WHERE (ap.CourseCategoryID = @categoryId OR @categoryId IS NULL)
                        AND (cu.CentreID = @centreId OR (cu.AllCentres = 1 AND ca.Active = 1))
                        AND ca.CentreID = @centreId
                        AND ap.DefaultContentTypeID <> 4";

        private readonly string selectDelegateCourseInfoQuery =
            @$"SELECT
                cu.CustomisationID AS CustomisationId,
                cu.CustomisationName,
                ap.ApplicationName,
                ap.CourseCategoryID,
                cu.IsAssessed,
                cu.CentreID AS CustomisationCentreId,
                cu.Active AS IsCourseActive,
                cu.AllCentres AS AllCentresCourse,
                pr.ProgressId,
                pr.PLLocked as IsProgressLocked,
                pr.SubmittedTime AS LastUpdated,
                pr.CompleteByDate AS CompleteBy,
                pr.RemovedDate,
                pr.Completed AS Completed,
                pr.Evaluated AS Evaluated,
                pr.LoginCount,
                Sum(apr.TutTime) AS LearningTime,
                pr.DiagnosticScore,
                LTRIM(RTRIM(pr.Answer1)) AS Answer1,
                LTRIM(RTRIM(pr.Answer2)) AS Answer2,
                LTRIM(RTRIM(pr.Answer3)) AS Answer3,
                {DelegateAllAttemptsQuery},
                {DelegateAttemptsPassedQuery},
                pr.FirstSubmittedTime AS Enrolled,
                pr.EnrollmentMethodID AS EnrolmentMethodId,
                uEnrolledBy.FirstName AS EnrolledByForename,
                uEnrolledBy.LastName AS EnrolledBySurname,
                aaEnrolledBy.Active AS EnrolledByAdminActive,
                aaSupervisor.ID AS SupervisorAdminId,
                uSupervisor.FirstName AS SupervisorForename,
                uSupervisor.LastName AS SupervisorSurname,
                aaSupervisor.Active AS SupervisorAdminActive,
                da.ID AS DelegateId,
                da.CandidateNumber,
                u.FirstName AS DelegateFirstName,
                u.LastName AS DelegateLastName,
                COALESCE(ucd.Email, u.PrimaryEmail) AS DelegateEmail,
                da.Active AS IsDelegateActive,
                u.HasBeenPromptedForPrn,
                u.ProfessionalRegistrationNumber,
                da.CentreID AS DelegateCentreId,
                ap.ArchivedDate AS CourseArchivedDate
            FROM Customisations cu
            INNER JOIN Applications AS ap ON ap.ApplicationID = cu.ApplicationID
            INNER JOIN Progress AS pr ON pr.CustomisationID = cu.CustomisationID
            INNER JOIN aspProgress AS apr ON pr.ProgressID = apr.ProgressID
            LEFT OUTER JOIN AdminAccounts AS aaSupervisor ON aaSupervisor.ID = pr.SupervisorAdminId
            LEFT OUTER JOIN Users AS uSupervisor ON uSupervisor.ID = aaSupervisor.UserID
            LEFT OUTER JOIN AdminAccounts AS aaEnrolledBy ON aaEnrolledBy.ID = pr.EnrolledByAdminID
            LEFT OUTER JOIN Users AS uEnrolledBy ON uEnrolledBy.ID = aaEnrolledBy.UserID
            INNER JOIN DelegateAccounts AS da ON da.ID = pr.CandidateID
            INNER JOIN Users AS u ON u.ID = da.UserID
            LEFT JOIN UserCentreDetails AS ucd ON ucd.UserID = da.UserID AND ucd.centreID = da.CentreID";

        private readonly string courseAssessmentDetailsQuery = $@"SELECT
                        c.CustomisationID,
                        c.CentreID,
                        c.ApplicationID,
                        ap.ApplicationName,
                        c.CustomisationName,
                        c.Active,
                        c.IsAssessed,
                        cc.CategoryName,
                        ct.CourseTopic,
                        CASE WHEN ({TutorialWithLearningCountQuery}) > 0 THEN 1 ELSE 0 END  AS HasLearning,
                        CASE WHEN ({TutorialWithDiagnosticCountQuery}) > 0 THEN 1 ELSE 0 END AS HasDiagnostic
                    FROM Customisations AS c
                    INNER JOIN Applications AS ap ON ap.ApplicationID = c.ApplicationID
                    INNER JOIN CourseCategories AS cc ON ap.CourseCategoryId = cc.CourseCategoryId
                    INNER JOIN CourseTopics AS ct ON ap.CourseTopicId = ct.CourseTopicId
                    WHERE (c.CentreID = @centreId OR c.AllCentres = 1)
                        AND (ap.CourseCategoryID = @categoryId OR @categoryId IS NULL)
                        AND EXISTS (SELECT CentreApplicationID FROM CentreApplications WHERE (ApplicationID = c.ApplicationID) AND (CentreID = @centreID) AND (Active = 1))
                        AND ap.DefaultContentTypeID <> 4";

        public CourseDataService(IDbConnection connection, ILogger<CourseDataService> logger, ISelfAssessmentDataService selfAssessmentDataService)
        {
            this.connection = connection;
            this.logger = logger;
            this.selfAssessmentDataService = selfAssessmentDataService;
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
                @"GetActivitiesForDelegateEnrolment",
                new { delegateId = candidateId, centreId, categoryId = 0 },
                commandType: CommandType.StoredProcedure
            );
        }

        public IEnumerable<AvailableCourse> GetAvailableCourses(int delegateId, int? centreId, int categoryId)
        {
            return connection.Query<AvailableCourse>(
                @"GetActivitiesForDelegateEnrolment",
                new { delegateId, centreId, categoryId },
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

        public int EnrolOnActivitySelfAssessment(int selfAssessmentId, int candidateId, int supervisorId, string adminEmail,
            int selfAssessmentSupervisorRoleId, DateTime? completeByDate, int delegateUserId, int centreId)
        {
            IClockUtility clockUtility = new ClockUtility();
            DateTime startedDate = clockUtility.UtcNow;
            DateTime lastAccessed = startedDate;
            dynamic? completeByDateDynamic = null;
            if (completeByDate == null || completeByDate.GetValueOrDefault().Year > 1753)
            {
                completeByDateDynamic = completeByDate!;
            }
            var candidateAssessmentId = (int)connection.ExecuteScalar(
                @"SELECT COALESCE
                 ((SELECT ID
                  FROM    CandidateAssessments
                  WHERE (SelfAssessmentID = @selfAssessmentId) AND (DelegateUserID  = @delegateUserId) AND (CompletedDate IS NULL)), 0) AS ID",
                new { selfAssessmentId, delegateUserId }
            );

            if (candidateAssessmentId == 0)
            {
                candidateAssessmentId = connection.QuerySingle<int>(
                    @"INSERT INTO [dbo].[CandidateAssessments]
                           ([DelegateUserID]
                           ,[SelfAssessmentID]
                           ,[StartedDate]
                           ,[LastAccessed]
                           ,[CompleteByDate]
                           ,[CentreID])
                    OUTPUT INSERTED.Id
                     VALUES
                           (@DelegateUserID,
                           @selfAssessmentId,
                           @startedDate,
                           @lastAccessed,
                           @completeByDateDynamic,
                           @centreId);",
                    new { delegateUserId, selfAssessmentId, startedDate, lastAccessed, completeByDateDynamic, centreId }
                );
            }

            int supervisorDelegateId = (int)connection.ExecuteScalar(
                    @"SELECT COALESCE
                 ((SELECT TOP 1 ID FROM SupervisorDelegates WHERE SupervisorAdminID = @supervisorId AND DelegateUserId = @delegateUserId), 0) AS ID",
                    new { supervisorId, delegateUserId }
                );
            if (supervisorDelegateId == 0 && supervisorId > 0)
            {
                supervisorDelegateId = connection.QuerySingle<int>(@"INSERT INTO SupervisorDelegates
                    (SupervisorAdminID,
                        DelegateEmail,
                        DelegateUserId,
                        SupervisorEmail,
                        AddedByDelegate)
                    OUTPUT INSERTED.Id
                    SELECT
                        @supervisorId,
                        COALESCE(UCD.Email, U.PrimaryEmail),
                        DA.UserID,
                        @adminEmail,
                        0
                        FROM   DelegateAccounts AS DA
                        INNER JOIN Users AS U
                        ON DA.UserID = U.ID
                        LEFT OUTER JOIN UserCentreDetails AS UCD ON
                        DA.UserID = UCD.UserID AND
                        DA.CentreID = UCD.CentreID
                        WHERE (DA.UserID = @delegateUserId)", new { supervisorId, delegateUserId, adminEmail });
            }

            if (candidateAssessmentId > 0 && supervisorDelegateId > 0 && selfAssessmentSupervisorRoleId > 0)
            {
                int candidateAssessmentSupervisorsId = (int)connection.ExecuteScalar(
                    @"SELECT COALESCE
                             ((SELECT TOP 1 ID FROM CandidateAssessmentSupervisors WHERE CandidateAssessmentID = @candidateAssessmentID AND SupervisorDelegateId = @supervisorDelegateId), 0) AS ID",
                    new { candidateAssessmentId, supervisorDelegateId }
                );

                if (candidateAssessmentSupervisorsId == 0)
                {
                    int numberOfAffectedRows = connection.Execute(
                        @"INSERT INTO CandidateAssessmentSupervisors (CandidateAssessmentID, SupervisorDelegateId, SelfAssessmentSupervisorRoleID)
                        VALUES (@candidateAssessmentId, @supervisorDelegateId, @selfAssessmentSupervisorRoleId)",
                        new { candidateAssessmentId, supervisorDelegateId, selfAssessmentSupervisorRoleId }
                    );
                }
            }

            if (candidateAssessmentId > 1)
            {
                string sqlQuery = $@"
                BEGIN TRANSACTION
                UPDATE CandidateAssessments SET RemovedDate = NULL
                  WHERE ID = @candidateAssessmentId

                UPDATE CandidateAssessmentSupervisors SET Removed = NULL
                  {((selfAssessmentSupervisorRoleId > 0) ? " ,SelfAssessmentSupervisorRoleID = @selfAssessmentSupervisorRoleID" : string.Empty)}
                  WHERE CandidateAssessmentID = @candidateAssessmentId

                COMMIT TRANSACTION";

                connection.Execute(sqlQuery
                , new { candidateAssessmentId, selfAssessmentSupervisorRoleId });
            }

            if (candidateAssessmentId < 1)
            {
                logger.LogWarning(
                    "Not enrolled delegate on self assessment as db insert failed. " +
                    $"Self assessment id: {selfAssessmentId}, user id: {delegateUserId}"
                );
            }

            return candidateAssessmentId;
        }

        public void EnrolOnSelfAssessment(int selfAssessmentId, int delegateUserId, int centreId)
        {
            var enrolmentExists = (int)connection.ExecuteScalar(
                @"SELECT COALESCE
                 ((SELECT ID
                  FROM    CandidateAssessments
                  WHERE (SelfAssessmentID = @selfAssessmentId) AND (DelegateUserID = @delegateUserId) AND (CompletedDate IS NULL)), 0) AS ID",
                new { selfAssessmentId, delegateUserId }
            );

            if (enrolmentExists > 0)
            {
                var result = connection.QueryFirstOrDefault<DateTime?>(
                @"SELECT RemovedDate
                  FROM    CandidateAssessments
                  WHERE ID = @enrolmentExists",
                new { enrolmentExists }
                );
                DateTime? removedDate = result ?? null;
                if (removedDate != null)
                {
                    connection.Execute(
                        @"UPDATE CandidateAssessments
                        SET RemovedDate = NULL
                        WHERE ID = @enrolmentExists",
                        new { enrolmentExists }
                );
                }
            }

            if (enrolmentExists == 0)
            {
                enrolmentExists = connection.Execute(
                    @"INSERT INTO [dbo].[CandidateAssessments]
                           ([DelegateUserID]
                           ,[SelfAssessmentID]
                           ,[CentreID])
                     VALUES
                           (@delegateUserId,
                           @selfAssessmentId,
                            @centreId)",
                    new { selfAssessmentId, delegateUserId, centreId }
                );
            }

            if (enrolmentExists < 1)
            {
                logger.LogWarning(
                    "Not enrolled delegate on self assessment as db insert failed. " +
                    $"Self assessment id: {selfAssessmentId}, delgate user id: {delegateUserId}"
                );
            }
        }

        public int EnrolSelfAssessment(int selfAssessmentId, int delegateUserId, int centreId)
        {
            var enrolmentExists = connection.QuerySingle<int>(
                @"SELECT COALESCE
                 ((SELECT ID
                  FROM    CandidateAssessments
                  WHERE (SelfAssessmentID = @selfAssessmentId) AND (DelegateUserID = @delegateUserId) AND (RemovedDate IS NULL) AND (CompletedDate IS NULL)), 0) AS ID",
                new { selfAssessmentId, delegateUserId }
            );

            if (enrolmentExists == 0)
            {
                enrolmentExists = connection.Execute(
                    @"INSERT INTO [dbo].[CandidateAssessments]
                           ([delegateUserID]
                           ,[SelfAssessmentID]
                           ,[CentreID])
                     OUTPUT Inserted.ID
                     VALUES
                           (@delegateUserId,
                           @selfAssessmentId,
                           @centreId)",
                    new { selfAssessmentId, delegateUserId, centreId }
                );
            }

            if (enrolmentExists < 1)
            {
                logger.LogWarning(
                    "Not enrolled delegate on self assessment as db insert failed. " +
                    $"Self assessment id: {selfAssessmentId}, delegate user id: {delegateUserId}"
                );
            }
            return enrolmentExists;
        }


        public int GetNumberOfActiveCoursesAtCentreFilteredByCategory(int centreId, int? adminCategoryId)
        {
            return (int)connection.ExecuteScalar(
                @"SELECT COUNT(*)
                        FROM dbo.Customisations AS cu
                        INNER JOIN dbo.CentreApplications AS ca ON ca.ApplicationID = cu.ApplicationID
                        INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = ca.ApplicationID
                        WHERE (ap.CourseCategoryID = @adminCategoryId OR @adminCategoryId IS NULL)
                            AND cu.Active = 1
                            AND cu.CentreID = @centreId
                            AND ca.CentreID = @centreId
                            AND ap.DefaultContentTypeID <> 4",
                new { centreId, adminCategoryId }
            );
        }

        public IEnumerable<CourseStatistics> GetCourseStatisticsAtCentreFilteredByCategory(
            int centreId,
            int? categoryId
        )
        {
            return connection.Query<CourseStatistics>(
                CourseStatisticsQuery,
                new { centreId, categoryId }
            );
        }

        public IEnumerable<CourseStatistics> GetCourseStatisticsAtCentreFilteredByCategory(
            int centreId,
            int? categoryId,
            int exportQueryRowLimit,
            int currentRun,
            string? searchString,
            string? sortBy,
            string? filterString,
            string sortDirection
        )
        {
            string orderBy;
            string sortOrder;

            if (sortDirection == "Ascending")
                sortOrder = " ASC ";
            else
                sortOrder = " DESC ";

            if (sortBy == "CourseName" || sortBy == "SearchableName")
                orderBy = " ORDER BY ap.ApplicationName + cu.CustomisationName " + sortOrder;
            else
                orderBy = " ORDER BY " + sortBy + sortOrder + ", LTRIM(RTRIM(ap.ApplicationName)) + LTRIM(RTRIM(cu.CustomisationName))";

            string search = string.Empty;
            if (!string.IsNullOrEmpty(searchString))
            {
                search = " AND ( ap.ApplicationName + IIF(cu.CustomisationName IS NULL, '', ' - ' + cu.CustomisationName) LIKE N'%' + @searchString + N'%')";
            }

            string sql = @$"{CourseStatisticsQuery} {search} {orderBy}
                        OFFSET @exportQueryRowLimit * (@currentRun - 1) ROWS
                        FETCH NEXT @exportQueryRowLimit ROWS ONLY";
            return connection.Query<CourseStatistics>(
                sql,
                new { centreId, categoryId, exportQueryRowLimit, currentRun, orderBy, searchString }
            );
        }

        public int GetCourseStatisticsAtCentreFilteredByCategoryResultCount(
            int centreId,
            int? categoryId,
            string? searchString
        )
        {
            string search = string.Empty;
            if (!string.IsNullOrEmpty(searchString))
            {
                search = " AND ( ap.ApplicationName + IIF(cu.CustomisationName IS NULL, '', ' - ' + cu.CustomisationName) LIKE N'%' + @searchString + N'%')";
            }
            int ResultCount = connection.ExecuteScalar<int>(@$"SELECT  COUNT(*) AS Matches FROM dbo.Customisations AS cu
                    INNER JOIN dbo.CentreApplications AS ca ON ca.ApplicationID = cu.ApplicationID
                    INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = ca.ApplicationID
                    INNER JOIN dbo.CourseCategories AS cc ON cc.CourseCategoryID = ap.CourseCategoryID
                    INNER JOIN dbo.CourseTopics AS ct ON ct.CourseTopicID = ap.CourseTopicId
                    WHERE (ap.CourseCategoryID = @categoryId OR @categoryId IS NULL)
                        AND (cu.CentreID = @centreId OR (cu.AllCentres = 1 AND ca.Active = 1))
                        AND ca.CentreID = @centreId
                        {search}
                        AND ap.DefaultContentTypeID <> 4", new { centreId, categoryId, searchString },
                    commandTimeout: 3000);
            return ResultCount;
        }

        public (IEnumerable<CourseStatistics>, int) GetCourseStatisticsAtCentre(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection, int centreId, int? categoryId, bool allCentreCourses, bool? hideInLearnerPortal,
            string isActive, string categoryName, string courseTopic, string hasAdminFields
        )
        {
            string courseStatisticsSelect = @$" SELECT
                        cu.CustomisationID,
                        cu.CentreID,
                        cu.Active,
                        CASE WHEN ap.ArchivedDate IS NOT NULL THEN 0 ELSE cu.Active END AS Active,
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
                        cu.LearningTimeMins AS LearningMinutes,
                        cu.IsAssessed,
                        CASE WHEN ap.ArchivedDate IS NOT NULL THEN 1 ELSE 0 END AS Archived,
                        ((SELECT COUNT(pr.CandidateID)
		                    FROM dbo.Progress AS pr WITH (NOLOCK) 
		                    INNER JOIN dbo.Candidates AS can WITH (NOLOCK) ON can.CandidateID = pr.CandidateID
		                    WHERE pr.CustomisationID = cu.CustomisationID
		                    AND can.CentreID = @centreId
		                    AND RemovedDate IS NULL) - 
		                (SELECT COUNT(pr.CandidateID)
		                    FROM dbo.Progress AS pr WITH (NOLOCK) 
		                    INNER JOIN dbo.Candidates AS can WITH (NOLOCK) ON can.CandidateID = pr.CandidateID
		                    WHERE pr.CustomisationID = cu.CustomisationID AND pr.Completed IS NOT NULL
		                    AND can.CentreID = @centreId)) AS InProgressCount ";
            string courseStatisticsFromTable = @$" FROM dbo.Customisations AS cu WITH (NOLOCK)
                    INNER JOIN dbo.CentreApplications AS ca WITH (NOLOCK) ON ca.ApplicationID = cu.ApplicationID
                    INNER JOIN dbo.Applications AS ap WITH (NOLOCK) ON ap.ApplicationID = ca.ApplicationID
                    INNER JOIN dbo.CourseCategories AS cc WITH (NOLOCK) ON cc.CourseCategoryID = ap.CourseCategoryID
                    INNER JOIN dbo.CourseTopics AS ct WITH (NOLOCK) ON ct.CourseTopicID = ap.CourseTopicId

                    LEFT JOIN CoursePrompts AS cp1 WITH (NOLOCK) 
			            ON cu.CourseField1PromptID = cp1.CoursePromptID
		            LEFT JOIN CoursePrompts AS cp2 WITH (NOLOCK) 
			            ON cu.CourseField2PromptID = cp2.CoursePromptID
		            LEFT JOIN CoursePrompts AS cp3 WITH (NOLOCK) 
			            ON cu.CourseField3PromptID = cp3.CoursePromptID

                    WHERE (ap.CourseCategoryID = @categoryId OR @categoryId IS NULL)
                        AND (cu.CentreID = @centreId OR (cu.AllCentres = 1 AND ca.Active = @allCentreCourses))
                        AND ca.CentreID = @centreId
                        AND ap.DefaultContentTypeID <> 4
                        AND ( ap.ApplicationName + IIF(cu.CustomisationName IS NULL, '', ' - ' + cu.CustomisationName) LIKE N'%' + @searchString + N'%')
                        AND ((@isActive = 'Any') OR (@isActive = 'true' AND (cu.Active = 1 AND ap.ArchivedDate IS NULL)) OR (@isActive = 'false' AND ((cu.Active = 0 OR ap.ArchivedDate IS NOT NULL))))
                        AND ((@categoryName = 'Any') OR (cc.CategoryName = @categoryName))
                        AND ((@courseTopic = 'Any') OR (ct.CourseTopic = @courseTopic))
                        AND ((@hasAdminFields = 'Any') OR (@hasAdminFields = 'true' AND (cp1.CoursePrompt IS NOT NULL OR cp2.CoursePrompt IS NOT NULL OR cp3.CoursePrompt IS NOT NULL))
                                                       OR (@hasAdminFields = 'false' AND (cp1.CoursePrompt IS NULL AND cp2.CoursePrompt IS NULL AND cp3.CoursePrompt IS NULL)))";

            if (hideInLearnerPortal != null)
                courseStatisticsFromTable += " AND cu.HideInLearnerPortal = @hideInLearnerPortal";

            string orderBy;
            string sortOrder;

            if (sortDirection == "Ascending")
                sortOrder = " ASC ";
            else
                sortOrder = " DESC ";

            if (sortBy == "CourseName" || sortBy == "SearchableName")
                orderBy = " ORDER BY ap.ApplicationName " + sortOrder + ", cu.CustomisationName " + sortOrder;
            else
                orderBy = " ORDER BY " + sortBy + sortOrder + ", LTRIM(RTRIM(ap.ApplicationName)) + LTRIM(RTRIM(cu.CustomisationName))";

            orderBy += " OFFSET " + offSet + " ROWS FETCH NEXT " + itemsPerPage + " ROWS ONLY ";

            var courseStatisticsQuery = courseStatisticsSelect + courseStatisticsFromTable + orderBy;

            IEnumerable<CourseStatistics> courseStatistics = connection.Query<CourseStatistics>(
                courseStatisticsQuery,
                new
                {
                    searchString,
                    offSet,
                    itemsPerPage,
                    sortBy,
                    sortDirection,
                    centreId,
                    categoryId,
                    allCentreCourses,
                    hideInLearnerPortal,
                    isActive,
                    categoryName,
                    courseTopic,
                    hasAdminFields
                },
                commandTimeout: 3000
            );

            var courseStatisticsCountQuery = @$"SELECT  COUNT(*) AS Matches " + courseStatisticsFromTable;

            int resultCount = connection.ExecuteScalar<int>(
                courseStatisticsCountQuery,
                new
                {
                    searchString,
                    offSet,
                    itemsPerPage,
                    sortBy,
                    sortDirection,
                    centreId,
                    categoryId,
                    allCentreCourses,
                    hideInLearnerPortal,
                    isActive,
                    categoryName,
                    courseTopic,
                    hasAdminFields
                },
                commandTimeout: 3000
            );
            return (courseStatistics, resultCount);
        }

        public IEnumerable<CourseStatistics> GetNonArchivedCourseStatisticsAtCentreFilteredByCategory(
            int centreId,
            int? categoryId
        )
        {
            return connection.Query<CourseStatistics>(
                @$"{CourseStatisticsQuery} AND ap.ArchivedDate IS NULL",
                new { centreId, categoryId }
            );
        }

        public IEnumerable<DelegateCourseInfo> GetDelegateCoursesInfo(int delegateId)
        {
            return connection.Query<DelegateCourseInfo>(
                $@"{selectDelegateCourseInfoQuery}
                    WHERE pr.CandidateID = @delegateId
                        AND pr.RemovedDate IS NULL
                        AND ap.DefaultContentTypeID <> 4
                    GROUP BY cu.CustomisationID,
                        cu.CustomisationName,
                        ap.ApplicationName,
                        ap.CourseCategoryID,
                        cu.IsAssessed,
                        cu.CentreID,
                        cu.Active,
                        cu.AllCentres,
                        pr.ProgressId,
                        pr.PLLocked,
                        pr.SubmittedTime,
                        pr.CompleteByDate,
                        pr.RemovedDate,
                        pr.Completed,
                        pr.Evaluated,
                        pr.LoginCount,
                        pr.Duration,
                        pr.DiagnosticScore,
                        LTRIM(RTRIM(pr.Answer1)),
                        LTRIM(RTRIM(pr.Answer2)),
                        LTRIM(RTRIM(pr.Answer3)),
                        pr.FirstSubmittedTime,
                        pr.EnrollmentMethodID,
                        uEnrolledBy.FirstName,
                        uEnrolledBy.LastName,
                        aaEnrolledBy.Active,
                        aaSupervisor.ID,
                        uSupervisor.FirstName,
                        uSupervisor.LastName,
                        aaSupervisor.Active,
                        da.ID,
                        da.CandidateNumber,
                        u.FirstName,
                        u.LastName,
                        COALESCE(ucd.Email, u.PrimaryEmail),
                        da.Active,
                        u.HasBeenPromptedForPrn,
                        u.ProfessionalRegistrationNumber,
                        da.CentreID,
                        ap.ArchivedDate",
                new { delegateId }
            );
        }

        public DelegateCourseInfo? GetDelegateCourseInfoByProgressId(int progressId)
        {
            return connection.QuerySingleOrDefault<DelegateCourseInfo>(
                $@"{selectDelegateCourseInfoQuery}
                    WHERE pr.ProgressID = @progressId
                        AND ap.DefaultContentTypeID <> 4
                    GROUP BY cu.CustomisationID,
                        cu.CustomisationName,
                        ap.ApplicationName,
                        ap.CourseCategoryID,
                        cu.IsAssessed,
                        cu.CentreID,
                        cu.Active,
                        cu.AllCentres,
                        pr.ProgressId,
                        pr.PLLocked,
                        pr.SubmittedTime,
                        pr.CompleteByDate,
                        pr.RemovedDate,
                        pr.Completed,
                        pr.Evaluated,
                        pr.LoginCount,
                        pr.Duration,
                        pr.DiagnosticScore,
                        LTRIM(RTRIM(pr.Answer1)),
                        LTRIM(RTRIM(pr.Answer2)),
                        LTRIM(RTRIM(pr.Answer3)),
                        pr.FirstSubmittedTime,
                        pr.EnrollmentMethodID,
                        uEnrolledBy.FirstName,
                        uEnrolledBy.LastName,
                        aaEnrolledBy.Active,
                        aaSupervisor.ID,
                        uSupervisor.FirstName,
                        uSupervisor.LastName,
                        aaSupervisor.Active,
                        da.ID,
                        da.CandidateNumber,
                        u.FirstName,
                        u.LastName,
                        COALESCE(ucd.Email, u.PrimaryEmail),
                        da.Active,
                        u.HasBeenPromptedForPrn,
                        u.ProfessionalRegistrationNumber,
                        da.CentreID,
                        ap.ArchivedDate",
                new { progressId }
            );
        }

        public IEnumerable<DelegateCourseInfo> GetDelegateCourseInfosForCourse(int customisationId, int centreId)
        {
            return connection.Query<DelegateCourseInfo>(
                $@"{selectDelegateCourseInfoQuery}
                    WHERE (cu.CentreID = @centreId OR
                            (cu.AllCentres = 1 AND
                                EXISTS (SELECT CentreApplicationID
                                        FROM CentreApplications cap
                                        WHERE cap.ApplicationID = cu.ApplicationID AND
                                            cap.CentreID = @centreID AND
                                            cap.Active = 1)))
                        AND da.CentreID = @centreId
                        AND pr.CustomisationID = @customisationId
                        AND ap.DefaultContentTypeID <> 4
                    GROUP BY cu.CustomisationID,
                        cu.CustomisationName,
                        ap.ApplicationName,
                        ap.CourseCategoryID,
                        cu.IsAssessed,
                        cu.CentreID,
                        cu.Active,
                        cu.AllCentres,
                        pr.ProgressId,
                        pr.PLLocked,
                        pr.SubmittedTime,
                        pr.CompleteByDate,
                        pr.RemovedDate,
                        pr.Completed,
                        pr.Evaluated,
                        pr.LoginCount,
                        pr.Duration,
                        pr.DiagnosticScore,
                        LTRIM(RTRIM(pr.Answer1)),
                        LTRIM(RTRIM(pr.Answer2)),
                        LTRIM(RTRIM(pr.Answer3)),
                        pr.FirstSubmittedTime,
                        pr.EnrollmentMethodID,
                        uEnrolledBy.FirstName,
                        uEnrolledBy.LastName,
                        aaEnrolledBy.Active,
                        aaSupervisor.ID,
                        uSupervisor.FirstName,
                        uSupervisor.LastName,
                        aaSupervisor.Active,
                        da.ID,
                        da.CandidateNumber,
                        u.FirstName,
                        u.LastName,
                        COALESCE(ucd.Email, u.PrimaryEmail),
                        da.Active,
                        u.HasBeenPromptedForPrn,
                        u.ProfessionalRegistrationNumber,
                        da.CentreID,
                        ap.ArchivedDate",
                new { customisationId, centreId }
            );
        }

        public (IEnumerable<DelegateCourseInfo>, int) GetDelegateCourseInfosPerPageForCourse(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection,
            int customisationId, int centreId, bool? isDelegateActive, bool? isProgressLocked, bool? removed, bool? hasCompleted, string? answer1, string? answer2, string? answer3)
        {
            searchString = searchString == null ? string.Empty : searchString.Trim();
            var selectColumnQuery = $@"SELECT
                cu.CustomisationID AS CustomisationId,
                cu.CustomisationName,
                ap.ApplicationName,
                ap.CourseCategoryID,
                cu.IsAssessed,
                cu.CentreID AS CustomisationCentreId,
                cu.Active AS IsCourseActive,
                cu.AllCentres AS AllCentresCourse,
                pr.ProgressId,
                pr.PLLocked as IsProgressLocked,
                pr.SubmittedTime AS LastUpdated,
                pr.CompleteByDate AS CompleteBy,
                pr.RemovedDate,
                pr.Completed AS Completed,
                pr.Evaluated AS Evaluated,
                pr.LoginCount,
                pr.Duration AS LearningTime,
                pr.DiagnosticScore,
                LTRIM(RTRIM(pr.Answer1)) AS Answer1,
                LTRIM(RTRIM(pr.Answer2)) AS Answer2,
                LTRIM(RTRIM(pr.Answer3)) AS Answer3,
                {DelegateAllAttemptsQuery},
                {DelegateAttemptsPassedQuery},
                pr.FirstSubmittedTime AS Enrolled,
                pr.EnrollmentMethodID AS EnrolmentMethodId,
                uEnrolledBy.FirstName AS EnrolledByForename,
                uEnrolledBy.LastName AS EnrolledBySurname,
                aaEnrolledBy.Active AS EnrolledByAdminActive,
                aaSupervisor.ID AS SupervisorAdminId,
                uSupervisor.FirstName AS SupervisorForename,
                uSupervisor.LastName AS SupervisorSurname,
                aaSupervisor.Active AS SupervisorAdminActive,
                da.ID AS DelegateId,
                da.CandidateNumber AS CandidateNumber,
                u.FirstName AS DelegateFirstName,
                u.LastName AS DelegateLastName,
                COALESCE(ucd.Email, u.PrimaryEmail) AS DelegateEmail,
                da.Active AS IsDelegateActive,
                u.HasBeenPromptedForPrn,
                u.ProfessionalRegistrationNumber,
                da.CentreID AS DelegateCentreId,
                ap.ArchivedDate AS CourseArchivedDate,
                {DelegatePassRateQuery}";

            var fromTableQuery = $@" FROM Customisations cu WITH (NOLOCK)
                INNER JOIN Applications AS ap WITH (NOLOCK) ON ap.ApplicationID = cu.ApplicationID
                INNER JOIN Progress AS pr WITH (NOLOCK) ON pr.CustomisationID = cu.CustomisationID
                LEFT OUTER JOIN AdminAccounts AS aaSupervisor WITH (NOLOCK) ON aaSupervisor.ID = pr.SupervisorAdminId
                LEFT OUTER JOIN Users AS uSupervisor WITH (NOLOCK) ON uSupervisor.ID = aaSupervisor.UserID
                LEFT OUTER JOIN AdminAccounts AS aaEnrolledBy WITH (NOLOCK) ON aaEnrolledBy.ID = pr.EnrolledByAdminID
                LEFT OUTER JOIN Users AS uEnrolledBy WITH (NOLOCK) ON uEnrolledBy.ID = aaEnrolledBy.UserID
                INNER JOIN DelegateAccounts AS da WITH (NOLOCK) ON da.ID = pr.CandidateID
                INNER JOIN Users AS u WITH (NOLOCK) ON u.ID = da.UserID
                LEFT JOIN UserCentreDetails AS ucd WITH (NOLOCK) ON ucd.UserID = da.UserID AND ucd.centreID = da.CentreID

                WHERE (cu.CentreID = @centreId OR
                        (cu.AllCentres = 1 AND
                            EXISTS (SELECT CentreApplicationID
                                    FROM CentreApplications cap
                                    WHERE cap.ApplicationID = cu.ApplicationID AND
                                        cap.CentreID = @centreID AND
                                        cap.Active = 1)))
                AND da.CentreID = @centreId
                AND pr.CustomisationID = @customisationId
                AND ap.DefaultContentTypeID <> 4

                AND ( u.FirstName + ' ' + u.LastName + ' ' + COALESCE(ucd.Email, u.PrimaryEmail) + ' ' + COALESCE(CandidateNumber, '') LIKE N'%' + @searchString + N'%')
                AND ((@isDelegateActive IS NULL) OR (@isDelegateActive = 1 AND (da.Active = 1)) OR (@isDelegateActive = 0 AND (da.Active = 0)))
				AND ((@isProgressLocked IS NULL) OR (@isProgressLocked = 1 AND (pr.PLLocked = 1)) OR (@isProgressLocked = 0 AND (pr.PLLocked = 0)))
				AND ((@removed IS NULL) OR (@removed = 1 AND (pr.RemovedDate IS NOT NULL)) OR (@removed = 0 AND (pr.RemovedDate IS NULL)))
				AND ((@hasCompleted IS NULL) OR (@hasCompleted = 1 AND pr.Completed IS NOT NULL) OR (@hasCompleted = 0 AND pr.Completed IS NULL))

                AND ((@answer1 IS NULL) OR ((@answer1 = 'No option selected' OR @answer1 = 'FREETEXTBLANKVALUE') AND (pr.Answer1 IS NULL OR LTRIM(RTRIM(pr.Answer1)) = '')) 
							OR (@answer1 = 'FREETEXTNOTBLANKVALUE' AND (pr.Answer1 IS NOT NULL OR pr.Answer1 = @answer1)))

				AND ((@answer2 IS NULL) OR ((@answer2 = 'No option selected' OR @answer2 = 'FREETEXTBLANKVALUE') AND (pr.Answer2 IS NULL OR LTRIM(RTRIM(pr.Answer2)) = '')) 
							OR (@answer2 = 'FREETEXTNOTBLANKVALUE' AND (pr.Answer2 IS NOT NULL OR pr.Answer2 = @answer2)))

				AND ((@answer3 IS NULL) OR ((@answer3 = 'No option selected' OR @answer3 = 'FREETEXTBLANKVALUE') AND (pr.Answer3 IS NULL OR LTRIM(RTRIM(pr.Answer3)) = '')) 
							OR (@answer3 = 'FREETEXTNOTBLANKVALUE' AND (pr.Answer3 IS NOT NULL OR pr.Answer3 = @answer3)))
                
                AND COALESCE(ucd.Email, u.PrimaryEmail) LIKE '%_@_%.__%'";

            string orderBy;
            string sortOrder;

            if (sortDirection == "Ascending")
                sortOrder = " ASC ";
            else
                sortOrder = " DESC ";

            if (sortBy == "SearchableName" || sortBy == "FullNameForSearchingSorting")
                orderBy = " ORDER BY LTRIM(u.LastName) " + sortOrder + ", LTRIM(u.FirstName) ";
            else
                orderBy = " ORDER BY  " + sortBy + sortOrder;

            orderBy += " OFFSET " + offSet + " ROWS FETCH NEXT " + itemsPerPage + " ROWS ONLY ";

            var mainSql = selectColumnQuery + fromTableQuery + orderBy;

            IEnumerable<DelegateCourseInfo> delegateUserCard = connection.Query<DelegateCourseInfo>(
                mainSql,
                new
                {
                    searchString,
                    offSet,
                    itemsPerPage,
                    sortBy,
                    sortDirection,
                    customisationId,
                    centreId,
                    isDelegateActive,
                    isProgressLocked,
                    removed,
                    hasCompleted,
                    answer1,
                    answer2,
                    answer3
                },
                commandTimeout: 3000
            );

            var delegateCountQuery = @$"SELECT  COUNT(*) AS Matches " + fromTableQuery;

            int ResultCount = connection.ExecuteScalar<int>(
                delegateCountQuery,
                new
                {
                    searchString,
                    offSet,
                    itemsPerPage,
                    sortBy,
                    sortDirection,
                    customisationId,
                    centreId,
                    isDelegateActive,
                    isProgressLocked,
                    removed,
                    hasCompleted,
                    answer1,
                    answer2,
                    answer3
                },
                commandTimeout: 3000
            );
            return (delegateUserCard, ResultCount);
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
                        {CompletedCountQuery},
                        ap.CourseCategoryID
                    FROM dbo.Customisations AS cu
                    INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = cu.ApplicationID
                    LEFT JOIN dbo.Customisations AS refreshToCu ON refreshToCu.CustomisationID = cu.RefreshToCustomisationId
                    LEFT JOIN dbo.Applications AS refreshToAp ON refreshToAp.ApplicationID = refreshToCu.ApplicationID
                    WHERE
                        (ap.CourseCategoryID = @categoryId OR @categoryId IS NULL)
                        AND cu.CentreID = @centreId
                        AND ap.ArchivedDate IS NULL
                        AND cu.CustomisationID = @customisationId
                        AND ap.DefaultContentTypeID <> 4",
                new { customisationId, centreId, categoryId }
            ).FirstOrDefault();
        }

        public CourseNameInfo? GetCourseNameAndApplication(int customisationId)
        {
            var names = connection.QueryFirstOrDefault<CourseNameInfo>(
                @"SELECT cu.CustomisationName, ap.ApplicationName
                        FROM Customisations cu
                        JOIN Applications ap ON cu.ApplicationId = ap.ApplicationId
                        WHERE cu.CustomisationId = @customisationId
                            AND ap.DefaultContentTypeID <> 4",
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

        public bool GetSelfRegister(int customisationId)
        {
            var selfRegister = connection.QueryFirstOrDefault<bool>(
                @"SELECT SelfRegister
                    FROM Customisations
                    WHERE CustomisationID = @customisationId",
                new { customisationId });

            return selfRegister;
        }

        public IEnumerable<CourseAssessmentDetails> GetCoursesAvailableToCentreByCategory(int centreId, int? categoryId)
        {
            return connection.Query<CourseAssessmentDetails>(
                courseAssessmentDetailsQuery,
                new { centreId, categoryId }
            );
        }

        public IEnumerable<CourseAssessmentDetails> GetNonArchivedCoursesAvailableToCentreByCategory(int centreId, int? categoryId)
        {
            return connection.Query<CourseAssessmentDetails>(
                @$"{courseAssessmentDetailsQuery} AND ap.ArchivedDate IS NULL",
                new { centreId, categoryId }
            );
        }

        public IEnumerable<ApplicationDetails> GetApplicationsAvailableToCentreByCategory(int centreId, int? categoryId)
        {
            return connection.Query<ApplicationDetails>(
                @"SELECT
                        ap.ApplicationID,
                        ap.ApplicationName,
                        ap.PLAssess,
                        ap.DiagAssess,
                        ap.CourseTopicID,
                        cc.CategoryName,
                        ct.CourseTopic
                    FROM Applications AS ap
                    INNER JOIN CourseCategories AS cc ON ap.CourseCategoryId = cc.CourseCategoryId
                    INNER JOIN CourseTopics AS ct ON ap.CourseTopicId = ct.CourseTopicId
                    WHERE ap.ArchivedDate IS NULL
                        AND ap.DefaultContentTypeID <> 4
                        AND (ap.CourseCategoryID = @categoryId OR @categoryId IS NULL)
                        AND EXISTS (SELECT CentreApplicationID FROM CentreApplications
                                    WHERE (CentreID = @centreID AND ApplicationID = ap.ApplicationID))",
                new { centreId, categoryId }
            );
        }

        public IEnumerable<ApplicationDetails> GetApplicationsByBrandId(int brandId)
        {
            return connection.Query<ApplicationDetails>(
                @"SELECT
                        ap.ApplicationID,
                        ap.ApplicationName,
                        ap.PLAssess,
                        ap.DiagAssess,
                        ap.CreatedDate,
                        ap.CourseTopicID,
                        cc.CategoryName,
                        ct.CourseTopic
                    FROM Applications AS ap
                    INNER JOIN CourseCategories AS cc ON ap.CourseCategoryId = cc.CourseCategoryId
                    INNER JOIN CourseTopics AS ct ON ap.CourseTopicId = ct.CourseTopicId
                    WHERE ap.ArchivedDate IS NULL
                        AND ap.Debug = 0
                        AND ap.BrandID = @brandId
                        AND ap.DefaultContentTypeID <> 4",
                new { brandId }
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
                        AND ap.DefaultContentTypeID <> 4",
                new { centreId, categoryId }
            );
        }

        public bool DoesCourseNameExistAtCentre(
            string customisationName,
            int centreId,
            int applicationId,
            int customisationId = 0
        )
        {
            return connection.ExecuteScalar<bool>(
                @"SELECT CASE WHEN EXISTS (
                        SELECT c.CustomisationID
                        FROM dbo.Customisations AS c
                        INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = c.ApplicationID
                        WHERE c.[ApplicationID] = @applicationID
                            AND c.[CentreID] = @centreID
                            AND c.[CustomisationName] = @customisationName
                            AND c.[CustomisationID] != @customisationId
                            AND ap.DefaultContentTypeID <> 4)
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT) END",
                new { customisationName, centreId, applicationId, customisationId }
            );
        }

        public CourseValidationDetails? GetCourseValidationDetails(int customisationId, int centreId)
        {
            return connection.QueryFirstOrDefault<CourseValidationDetails>(
                @"SELECT
                        c.CentreId,
                        a.CourseCategoryId,
                        c.AllCentres,
                        CASE WHEN EXISTS (
                                SELECT CentreApplicationID
                                FROM CentreApplications
                                WHERE (ApplicationID = c.ApplicationID) AND (CentreID = @centreId) AND (Active = 1)
                            )
                            THEN CAST(1 AS BIT)
                            ELSE CAST(0 AS BIT)
                        END AS CentreHasApplication
                    FROM Customisations AS c
                    INNER JOIN Applications AS a on a.ApplicationID = c.ApplicationID
                    WHERE CustomisationID = @customisationId
                        AND a.DefaultContentTypeID <> 4",
                new { customisationId, centreId }
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

        public Dictionary<int, int> GetNumsOfRecentProgressRecordsForBrand(int brandId, DateTime threeMonthsAgo)
        {
            var query = connection.Query(
                @"SELECT
                        Applications.ApplicationID,
                        COUNT(Progress.ProgressID) AS NumRecentProgressRecords
                    FROM Applications
                    INNER JOIN Customisations ON Applications.ApplicationID = Customisations.ApplicationID
                    INNER JOIN Progress ON Customisations.CustomisationID = Progress.CustomisationID
                    WHERE Applications.BrandID = @brandId
                        AND Applications.Debug = 0
                        AND Applications.ArchivedDate IS NULL
                        AND Progress.SubmittedTime > @threeMonthsAgo
                        AND Applications.DefaultContentTypeID <> 4
                    GROUP BY Applications.ApplicationID",
                new
                { brandId, threeMonthsAgo }
            );
            return query.ToDictionary<dynamic?, int, int>(
                entry => entry?.ApplicationID,
                entry => entry?.NumRecentProgressRecords
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
                        AND cu.CustomisationID = @customisationId
                        AND ap.DefaultContentTypeID <> 4",
                new { customisationId, centreId, categoryId }
            ).FirstOrDefault();
        }

        public int CreateNewCentreCourse(Customisation customisation)
        {
            var customisationId = connection.QuerySingle<int>(
                @"INSERT INTO Customisations(
                        CurrentVersion,
                        CentreID,
                        ApplicationID,
                        Active,
                        CustomisationName,
                        Password,
                        SelfRegister,
                        TutCompletionThreshold,
                        IsAssessed,
                        DiagCompletionThreshold,
                        DiagObjSelect,
                        HideInLearnerPortal,
                        NotificationEmails)
                    OUTPUT Inserted.CustomisationID
                    VALUES
                        (1,
                        @CentreId,
                        @ApplicationId,
                        1,
                        @CustomisationName,
                        @Password,
                        @SelfRegister,
                        @TutCompletionThreshold,
                        @IsAssessed,
                        @DiagCompletionThreshold,
                        @DiagObjSelect,
                        @HideInLearnerPortal,
                        @NotificationEmails)",
                new
                {
                    customisation.CentreId,
                    customisation.ApplicationId,
                    customisation.CustomisationName,
                    customisation.Password,
                    customisation.SelfRegister,
                    customisation.TutCompletionThreshold,
                    customisation.IsAssessed,
                    customisation.DiagCompletionThreshold,
                    customisation.DiagObjSelect,
                    customisation.HideInLearnerPortal,
                    customisation.NotificationEmails,
                }
            );

            return customisationId;
        }

        public IEnumerable<CourseDelegateForExport> GetDelegatesOnCourseForExport(int customisationId, int centreId)
        {
            return connection.Query<CourseDelegateForExport>(
                $@"SELECT
                        ap.ApplicationName,
                        cu.CustomisationName,
                        da.ID AS DelegateId,
                        da.CandidateNumber,
                        u.FirstName AS DelegateFirstName,
                        u.LastName AS DelegateLastName,
                        COALESCE(ucd.Email, u.PrimaryEmail) AS DelegateEmail,
                        da.Active AS IsDelegateActive,
                        da.Answer1 AS RegistrationAnswer1,
                        da.Answer2 AS RegistrationAnswer2,
                        da.Answer3 AS RegistrationAnswer3,
                        da.Answer4 AS RegistrationAnswer4,
                        da.Answer5 AS RegistrationAnswer5,
                        da.Answer6 AS RegistrationAnswer6,
                        p.ProgressID,
                        p.PLLocked AS IsProgressLocked,
                        p.SubmittedTime AS LastUpdated,
                        da.DateRegistered AS Enrolled,
                        p.CompleteByDate AS CompleteBy,
                        p.RemovedDate,
                        p.Completed,
                        p.CustomisationId,
                        p.LoginCount,
                        p.Duration,
                        p.DiagnosticScore,
                        p.Answer1,
                        p.Answer2,
                        p.Answer3,
                        {DelegateAllAttemptsQuery},
                        {DelegateAllAttemptsQuery}
                    FROM DelegateAccounts AS da
                    INNER JOIN Users AS u on u.ID = da.UserID
                    INNER JOIN Progress AS p ON p.CandidateID = da.ID
                    LEFT JOIN UserCentreDetails AS ucd ON ucd.UserID = da.UserID AND ucd.centreID = da.centreID
                    INNER JOIN Customisations AS cu ON cu.CustomisationID = p.CustomisationID
                    INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = cu.ApplicationID
                    WHERE da.CentreID = @centreId
                        AND p.CustomisationID = @customisationId
                        AND ap.DefaultContentTypeID <> 4",
                new { customisationId, centreId }
            );
        }

        public int GetCourseDelegatesCountForExport(string searchString, string sortBy, string sortDirection,
            int customisationId, int centreId, bool? isDelegateActive, bool? isProgressLocked, bool? removed, bool? hasCompleted, string? answer1, string? answer2, string? answer3)
        {
            searchString = searchString == null ? string.Empty : searchString.Trim();
            var fromTableQuery = $@" FROM Customisations cu WITH (NOLOCK)
                INNER JOIN Applications AS ap WITH (NOLOCK) ON ap.ApplicationID = cu.ApplicationID
                INNER JOIN Progress AS pr WITH (NOLOCK) ON pr.CustomisationID = cu.CustomisationID
                LEFT OUTER JOIN AdminAccounts AS aaSupervisor WITH (NOLOCK) ON aaSupervisor.ID = pr.SupervisorAdminId
                LEFT OUTER JOIN Users AS uSupervisor WITH (NOLOCK) ON uSupervisor.ID = aaSupervisor.UserID
                LEFT OUTER JOIN AdminAccounts AS aaEnrolledBy WITH (NOLOCK) ON aaEnrolledBy.ID = pr.EnrolledByAdminID
                LEFT OUTER JOIN Users AS uEnrolledBy WITH (NOLOCK) ON uEnrolledBy.ID = aaEnrolledBy.UserID
                INNER JOIN DelegateAccounts AS da WITH (NOLOCK) ON da.ID = pr.CandidateID
                INNER JOIN Users AS u WITH (NOLOCK) ON u.ID = da.UserID
                LEFT JOIN UserCentreDetails AS ucd WITH (NOLOCK) ON ucd.UserID = da.UserID AND ucd.centreID = da.CentreID

                WHERE (cu.CentreID = @centreId OR
                        (cu.AllCentres = 1 AND
                            EXISTS (SELECT CentreApplicationID
                                    FROM CentreApplications cap
                                    WHERE cap.ApplicationID = cu.ApplicationID AND
                                        cap.CentreID = @centreID AND
                                        cap.Active = 1)))
                AND da.CentreID = @centreId
                AND pr.CustomisationID = @customisationId
                AND ap.DefaultContentTypeID <> 4

                AND ( u.FirstName + ' ' + u.LastName + ' ' + COALESCE(ucd.Email, u.PrimaryEmail) + ' ' + COALESCE(CandidateNumber, '') LIKE N'%' + @searchString + N'%')
                AND ((@isDelegateActive IS NULL) OR (@isDelegateActive = 1 AND (da.Active = 1)) OR (@isDelegateActive = 0 AND (da.Active = 0)))
				AND ((@isProgressLocked IS NULL) OR (@isProgressLocked = 1 AND (pr.PLLocked = 1)) OR (@isProgressLocked = 0 AND (pr.PLLocked = 0)))
				AND ((@removed IS NULL) OR (@removed = 1 AND (pr.RemovedDate IS NOT NULL)) OR (@removed = 0 AND (pr.RemovedDate IS NULL)))
				AND ((@hasCompleted IS NULL) OR (@hasCompleted = 1 AND pr.Completed IS NOT NULL) OR (@hasCompleted = 0 AND pr.Completed IS NULL))

                AND ((@answer1 IS NULL) OR ((@answer1 = 'No option selected' OR @answer1 = 'FREETEXTBLANKVALUE') AND (pr.Answer1 IS NULL OR LTRIM(RTRIM(pr.Answer1)) = '')) 
							OR (@answer1 = 'FREETEXTNOTBLANKVALUE' AND (pr.Answer1 IS NOT NULL OR pr.Answer1 = @answer1)))

				AND ((@answer2 IS NULL) OR ((@answer2 = 'No option selected' OR @answer2 = 'FREETEXTBLANKVALUE') AND (pr.Answer2 IS NULL OR LTRIM(RTRIM(pr.Answer2)) = '')) 
							OR (@answer2 = 'FREETEXTNOTBLANKVALUE' AND (pr.Answer2 IS NOT NULL OR pr.Answer2 = @answer2)))

				AND ((@answer3 IS NULL) OR ((@answer3 = 'No option selected' OR @answer3 = 'FREETEXTBLANKVALUE') AND (pr.Answer3 IS NULL OR LTRIM(RTRIM(pr.Answer3)) = '')) 
							OR (@answer3 = 'FREETEXTNOTBLANKVALUE' AND (pr.Answer3 IS NOT NULL OR pr.Answer3 = @answer3)))
                
                AND COALESCE(ucd.Email, u.PrimaryEmail) LIKE '%_@_%.__%'";


            var mainSql = "SELECT COUNT(*) AS TotalRecords " + fromTableQuery;

            return connection.ExecuteScalar<int>(
                mainSql,
                new
                {
                    searchString,
                    sortBy,
                    sortDirection,
                    customisationId,
                    centreId,
                    isDelegateActive,
                    isProgressLocked,
                    removed,
                    hasCompleted,
                    answer1,
                    answer2,
                    answer3
                },
                commandTimeout: 3000
            );
        }


        public IEnumerable<CourseDelegateForExport> GetCourseDelegatesForExport(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection,
            int customisationId, int centreId, bool? isDelegateActive, bool? isProgressLocked, bool? removed, bool? hasCompleted, string? answer1, string? answer2, string? answer3)
        {
            searchString = searchString == null ? string.Empty : searchString.Trim();
            var selectColumnQuery = $@"SELECT
                        ap.ApplicationName,
                        cu.CustomisationName,
                        da.ID AS DelegateId,
                        da.CandidateNumber,
                        u.FirstName AS DelegateFirstName,
                        u.LastName AS DelegateLastName,
                        COALESCE(ucd.Email, u.PrimaryEmail) AS DelegateEmail,
                        da.Active AS IsDelegateActive,
                        da.Answer1 AS RegistrationAnswer1,
                        da.Answer2 AS RegistrationAnswer2,
                        da.Answer3 AS RegistrationAnswer3,
                        da.Answer4 AS RegistrationAnswer4,
                        da.Answer5 AS RegistrationAnswer5,
                        da.Answer6 AS RegistrationAnswer6,
                        pr.ProgressID,
                        pr.PLLocked AS IsProgressLocked,
                        pr.SubmittedTime AS LastUpdated,
                        pr.FirstSubmittedTime AS Enrolled,
                        pr.CompleteByDate AS CompleteBy,
                        pr.RemovedDate,
                        pr.Completed,
                        pr.CustomisationId,
                        pr.LoginCount,
                        pr.Duration,
                        pr.DiagnosticScore,
                        pr.Answer1,
                        pr.Answer2,
                        pr.Answer3,
                        {DelegateAllAttemptsQuery},
                        {DelegateAttemptsPassedQuery},                
                        {DelegatePassRateQuery}";

            var fromTableQuery = $@" FROM Customisations cu WITH (NOLOCK)
                INNER JOIN Applications AS ap WITH (NOLOCK) ON ap.ApplicationID = cu.ApplicationID
                INNER JOIN Progress AS pr WITH (NOLOCK) ON pr.CustomisationID = cu.CustomisationID
                LEFT OUTER JOIN AdminAccounts AS aaSupervisor WITH (NOLOCK) ON aaSupervisor.ID = pr.SupervisorAdminId
                LEFT OUTER JOIN Users AS uSupervisor WITH (NOLOCK) ON uSupervisor.ID = aaSupervisor.UserID
                LEFT OUTER JOIN AdminAccounts AS aaEnrolledBy WITH (NOLOCK) ON aaEnrolledBy.ID = pr.EnrolledByAdminID
                LEFT OUTER JOIN Users AS uEnrolledBy WITH (NOLOCK) ON uEnrolledBy.ID = aaEnrolledBy.UserID
                INNER JOIN DelegateAccounts AS da WITH (NOLOCK) ON da.ID = pr.CandidateID
                INNER JOIN Users AS u WITH (NOLOCK) ON u.ID = da.UserID
                LEFT JOIN UserCentreDetails AS ucd WITH (NOLOCK) ON ucd.UserID = da.UserID AND ucd.centreID = da.CentreID

                WHERE (cu.CentreID = @centreId OR
                        (cu.AllCentres = 1 AND
                            EXISTS (SELECT CentreApplicationID
                                    FROM CentreApplications cap
                                    WHERE cap.ApplicationID = cu.ApplicationID AND
                                        cap.CentreID = @centreID AND
                                        cap.Active = 1)))
                AND da.CentreID = @centreId
                AND pr.CustomisationID = @customisationId
                AND ap.DefaultContentTypeID <> 4

                AND ( u.FirstName + ' ' + u.LastName + ' ' + COALESCE(ucd.Email, u.PrimaryEmail) + ' ' + COALESCE(CandidateNumber, '') LIKE N'%' + @searchString + N'%')
                AND ((@isDelegateActive IS NULL) OR (@isDelegateActive = 1 AND (da.Active = 1)) OR (@isDelegateActive = 0 AND (da.Active = 0)))
				AND ((@isProgressLocked IS NULL) OR (@isProgressLocked = 1 AND (pr.PLLocked = 1)) OR (@isProgressLocked = 0 AND (pr.PLLocked = 0)))
				AND ((@removed IS NULL) OR (@removed = 1 AND (pr.RemovedDate IS NOT NULL)) OR (@removed = 0 AND (pr.RemovedDate IS NULL)))
				AND ((@hasCompleted IS NULL) OR (@hasCompleted = 1 AND pr.Completed IS NOT NULL) OR (@hasCompleted = 0 AND pr.Completed IS NULL))

                AND ((@answer1 IS NULL) OR ((@answer1 = 'No option selected' OR @answer1 = 'FREETEXTBLANKVALUE') AND (pr.Answer1 IS NULL OR LTRIM(RTRIM(pr.Answer1)) = '')) 
							OR (@answer1 = 'FREETEXTNOTBLANKVALUE' AND (pr.Answer1 IS NOT NULL OR pr.Answer1 = @answer1)))

				AND ((@answer2 IS NULL) OR ((@answer2 = 'No option selected' OR @answer2 = 'FREETEXTBLANKVALUE') AND (pr.Answer2 IS NULL OR LTRIM(RTRIM(pr.Answer2)) = '')) 
							OR (@answer2 = 'FREETEXTNOTBLANKVALUE' AND (pr.Answer2 IS NOT NULL OR pr.Answer2 = @answer2)))

				AND ((@answer3 IS NULL) OR ((@answer3 = 'No option selected' OR @answer3 = 'FREETEXTBLANKVALUE') AND (pr.Answer3 IS NULL OR LTRIM(RTRIM(pr.Answer3)) = '')) 
							OR (@answer3 = 'FREETEXTNOTBLANKVALUE' AND (pr.Answer3 IS NOT NULL OR pr.Answer3 = @answer3)))
                
                AND COALESCE(ucd.Email, u.PrimaryEmail) LIKE '%_@_%.__%'";

            string orderBy;
            string sortOrder;

            if (sortDirection == "Ascending")
                sortOrder = " ASC ";
            else
                sortOrder = " DESC ";

            if (sortBy == "SearchableName" || sortBy == "FullNameForSearchingSorting")
                orderBy = " ORDER BY LTRIM(u.LastName) " + sortOrder + ", LTRIM(u.FirstName) ";
            else
                orderBy = " ORDER BY  " + sortBy + sortOrder;

            orderBy += " OFFSET " + offSet + " ROWS FETCH NEXT " + itemsPerPage + " ROWS ONLY ";


            var mainSql = selectColumnQuery + fromTableQuery + orderBy;

            IEnumerable<CourseDelegateForExport> courseDelegates = connection.Query<CourseDelegateForExport>(
                mainSql,
                new
                {
                    searchString,
                    sortBy,
                    sortDirection,
                    customisationId,
                    centreId,
                    isDelegateActive,
                    isProgressLocked,
                    removed,
                    hasCompleted,
                    answer1,
                    answer2,
                    answer3
                },
                commandTimeout: 3000
            );


            return courseDelegates;
        }

        public bool IsCourseCompleted(int candidateId, int customisationId)
        {
            return connection.ExecuteScalar<bool>(
                @"SELECT CASE WHEN EXISTS (
                            SELECT p.Completed
                                FROM  Progress AS p INNER JOIN
                                                Customisations AS cu ON p.CustomisationID = cu.CustomisationID INNER JOIN
                                                Applications AS a ON cu.ApplicationID = a.ApplicationID
                                WHERE  (p.CandidateID = @candidateId) AND p.CustomisationID = @customisationId
                                AND (NOT (p.Completed IS NULL)))
                            THEN CAST(1 AS BIT)
                            ELSE CAST(0 AS BIT) END",
                new { candidateId, customisationId }
            );
        }

        public IEnumerable<Course> GetApplicationsAvailableToCentre(int centreId)
        {
            return connection.Query<Course>(
                @$"SELECT ap.ApplicationID, ap.ApplicationName,
                                     {DelegateCountQuery}, cu.CustomisationID, cu.CustomisationName
                    FROM   Applications AS ap INNER JOIN
                                 CentreApplications AS ca ON ap.ApplicationID = ca.ApplicationID LEFT OUTER JOIN
                                 Customisations AS cu ON ca.ApplicationID = cu.ApplicationID AND ca.CentreID = cu.CentreID AND cu.Active = 1
                    WHERE (ca.Active = 1) AND (ca.CentreID = @centreId)
                    ORDER BY ap.ApplicationName",
                new { centreId }
            );
        }

        public IEnumerable<CourseStatistics> GetDelegateCourseStatisticsAtCentre(string searchString, int centreId, int? categoryId, bool allCentreCourses, bool? hideInLearnerPortal, string isActive, string categoryName, string courseTopic, string hasAdminFields
        )
        {
            string courseStatisticsSelect = @$" SELECT
                        cu.CustomisationID,
                        cu.CentreID,
                        cu.Active,
                        CASE WHEN ap.ArchivedDate IS NOT NULL THEN 0 ELSE cu.Active END AS Active,
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
                        cu.LearningTimeMins AS LearningMinutes,
                        cu.IsAssessed,
                        CASE WHEN ap.ArchivedDate IS NOT NULL THEN 1 ELSE 0 END AS Archived,
                        ((SELECT COUNT(pr.CandidateID)
		                    FROM dbo.Progress AS pr WITH (NOLOCK) 
		                    INNER JOIN dbo.Candidates AS can WITH (NOLOCK) ON can.CandidateID = pr.CandidateID
		                    WHERE pr.CustomisationID = cu.CustomisationID
		                    AND can.CentreID = @centreId
		                    AND RemovedDate IS NULL) - 
		                (SELECT COUNT(pr.CandidateID)
		                    FROM dbo.Progress AS pr WITH (NOLOCK) 
		                    INNER JOIN dbo.Candidates AS can WITH (NOLOCK) ON can.CandidateID = pr.CandidateID
		                    WHERE pr.CustomisationID = cu.CustomisationID AND pr.Completed IS NOT NULL
		                    AND can.CentreID = @centreId)) AS InProgressCount ";
            string courseStatisticsFromTable = @$" FROM dbo.Customisations AS cu WITH (NOLOCK)
                    INNER JOIN dbo.CentreApplications AS ca WITH (NOLOCK) ON ca.ApplicationID = cu.ApplicationID
                    INNER JOIN dbo.Applications AS ap WITH (NOLOCK) ON ap.ApplicationID = ca.ApplicationID
                    INNER JOIN dbo.CourseCategories AS cc WITH (NOLOCK) ON cc.CourseCategoryID = ap.CourseCategoryID
                    INNER JOIN dbo.CourseTopics AS ct WITH (NOLOCK) ON ct.CourseTopicID = ap.CourseTopicId

                    LEFT JOIN CoursePrompts AS cp1 WITH (NOLOCK) 
			            ON cu.CourseField1PromptID = cp1.CoursePromptID
		            LEFT JOIN CoursePrompts AS cp2 WITH (NOLOCK) 
			            ON cu.CourseField2PromptID = cp2.CoursePromptID
		            LEFT JOIN CoursePrompts AS cp3 WITH (NOLOCK) 
			            ON cu.CourseField3PromptID = cp3.CoursePromptID

                    WHERE (ap.CourseCategoryID = @categoryId OR @categoryId IS NULL)
                        AND (cu.CentreID = @centreId OR (cu.AllCentres = 1 AND ca.Active = @allCentreCourses))
                        AND ca.CentreID = @centreId
                        AND ap.DefaultContentTypeID <> 4
                        AND ( ap.ApplicationName + IIF(cu.CustomisationName IS NULL, '', ' - ' + cu.CustomisationName) LIKE N'%' + @searchString + N'%')
                        AND ((@isActive = 'Any') OR (@isActive = 'true' AND (cu.Active = 1 AND ap.ArchivedDate IS NULL)) OR (@isActive = 'false' AND ((cu.Active = 0 OR ap.ArchivedDate IS NOT NULL))))
                        AND ((@categoryName = 'Any') OR (cc.CategoryName = @categoryName))
                        AND ((@courseTopic = 'Any') OR (ct.CourseTopic = @courseTopic))
                        AND ((@hasAdminFields = 'Any') OR (@hasAdminFields = 'true' AND (cp1.CoursePrompt IS NOT NULL OR cp2.CoursePrompt IS NOT NULL OR cp3.CoursePrompt IS NOT NULL))
                                                       OR (@hasAdminFields = 'false' AND (cp1.CoursePrompt IS NULL AND cp2.CoursePrompt IS NULL AND cp3.CoursePrompt IS NULL)))";

            if (hideInLearnerPortal != null)
                courseStatisticsFromTable += " AND cu.HideInLearnerPortal = @hideInLearnerPortal";

            var courseStatisticsQuery = courseStatisticsSelect + courseStatisticsFromTable;

            IEnumerable<CourseStatistics> courseStatistics = connection.Query<CourseStatistics>(
                courseStatisticsQuery,
                new
                {
                    searchString,
                    centreId,
                    categoryId,
                    allCentreCourses,
                    hideInLearnerPortal,
                    isActive,
                    categoryName,
                    courseTopic,
                    hasAdminFields
                },
                commandTimeout: 3000
            );

            return courseStatistics;
        }

        public IEnumerable<DelegateAssessmentStatistics> GetDelegateAssessmentStatisticsAtCentre(string searchString, int centreId, string categoryName, string isActive)
        {
            string assessmentStatisticsSelectQuery = $@"SELECT
                        sa.Name AS Name,
                        cc.CategoryName AS Category,
                        CASE 
                          WHEN sa.SupervisorSelfAssessmentReview = 1 OR sa.SupervisorResultsReview = 1 THEN 1
                          ELSE 0
                        END AS Supervised,
                        (SELECT COUNT(can.ID)
                        FROM dbo.CandidateAssessments AS can WITH (NOLOCK)
                            INNER JOIN Users AS u WITH (NOLOCK) ON u.ID = can.DelegateUserID 
						    LEFT JOIN UserCentreDetails AS ucd WITH (NOLOCK) ON ucd.UserID = u.ID AND ucd.centreID = can.CentreID
                        WHERE can.CentreID = @centreId AND can.SelfAssessmentID = csa.SelfAssessmentID
                            AND can.RemovedDate IS NULL AND COALESCE(ucd.Email, u.PrimaryEmail) LIKE '%_@_%.__%') AS DelegateCount,
                        (Select COUNT(*) FROM
                            (SELECT can.ID FROM dbo.CandidateAssessments AS can WITH (NOLOCK)
                                LEFT JOIN dbo.CandidateAssessmentSupervisors AS cas ON can.ID = cas.CandidateAssessmentID
                                LEFT JOIN dbo.CandidateAssessmentSupervisorVerifications AS casv ON cas.ID = casv.CandidateAssessmentSupervisorID
                                WHERE can.CentreID = @centreId AND can.SelfAssessmentID = CSA.SelfAssessmentID AND can.RemovedDate IS NULL
                                AND (can.SubmittedDate IS NOT NULL OR (casv.SignedOff = 1 AND casv.Verified IS NOT NULL)) GROUP BY can.ID) A
                                ) AS SubmittedSignedOffCount,
                        CC.Active AS Active,
                        sa.ID AS SelfAssessmentId
                        from CentreSelfAssessments AS csa 
                        INNER join SelfAssessments AS sa ON csa.SelfAssessmentID = sa.ID
                        INNER JOIN CourseCategories AS cc ON sa.CategoryID = cc.CourseCategoryID
                        WHERE csa.CentreID= @centreId
                                AND sa.[Name] LIKE '%' + @searchString + '%'
		                        AND ((@categoryName = 'Any') OR (cc.CategoryName = @categoryName))
		                        AND ((@isActive = 'Any') OR (@isActive = 'true' AND  sa.ArchivedDate IS NULL) OR (@isActive = 'false' AND sa.ArchivedDate IS NOT NULL))
                                ";

            IEnumerable<DelegateAssessmentStatistics> delegateAssessmentStatistics = connection.Query<DelegateAssessmentStatistics>(assessmentStatisticsSelectQuery,
                new { searchString, centreId, categoryName, isActive }, commandTimeout: 3000);
            return delegateAssessmentStatistics;
        }
    }
}
