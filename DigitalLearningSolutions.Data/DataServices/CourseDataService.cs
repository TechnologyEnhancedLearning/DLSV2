namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.Extensions.Logging;

    public interface ICourseDataService
    {
        IEnumerable<CurrentCourse> GetCurrentCourses(int candidateId);

        IEnumerable<CompletedCourse> GetCompletedCourses(int candidateId);

        IEnumerable<AvailableCourse> GetAvailableCourses(int candidateId, int? centreId);

        IEnumerable<AvailableCourse> GetAvailableCourses(int candidateId, int? centreId, int categoryId);

        void SetCompleteByDate(int progressId, int candidateId, DateTime? completeByDate);

        void RemoveCurrentCourse(int progressId, int candidateId, RemovalMethod removalMethod);

        void EnrolOnSelfAssessment(int selfAssessmentId, int candidateId);

        int EnrolSelfAssessment(int selfAssessmentId, int candidateId);

        int GetNumberOfActiveCoursesAtCentreFilteredByCategory(int centreId, int? categoryId);

        IEnumerable<CourseStatistics> GetCourseStatisticsAtCentreFilteredByCategory(int centreId, int? categoryId);

        IEnumerable<CourseStatistics> GetNonArchivedCourseStatisticsAtCentreFilteredByCategory(int centreId, int? categoryId);

        IEnumerable<DelegateCourseInfo> GetDelegateCoursesInfo(int delegateId);

        DelegateCourseInfo? GetDelegateCourseInfoByProgressId(int progressId);

        CourseNameInfo? GetCourseNameAndApplication(int customisationId);

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

        int EnrolOnActivitySelfAssessment(int selfAssessmentId, int candidateId, int supervisorId, string adminEmail,
            int selfAssessmentSupervisorRoleId, DateTime completeByDate);
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

        private const string DelegateAllAttemptsQuery =
            @"(SELECT COUNT(aa.AssessAttemptID)
                FROM dbo.AssessAttempts AS aa
                INNER JOIN dbo.DelegateAccounts AS dacc ON dacc.ID = aa.CandidateID
                WHERE aa.CustomisationID = cu.CustomisationID AND aa.[Status] IS NOT NULL
                AND dacc.ID = da.ID) AS AllAttempts";

        private const string DelegateAttemptsPassedQuery =
            @"(SELECT COUNT(aa.AssessAttemptID)
                FROM dbo.AssessAttempts AS aa
                INNER JOIN dbo.DelegateAccounts AS dacc ON dacc.ID = aa.CandidateID
                WHERE aa.CustomisationID = cu.CustomisationID AND aa.[Status] = 1
                AND dacc.ID = da.ID) AS AttemptsPassed";

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
                        cu.IsAssessed
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
                @"GetActiveAvailableCustomisationsForCentreFiltered_V5",
                new { candidateId, centreId },
                commandType: CommandType.StoredProcedure
            );
        }

        public IEnumerable<AvailableCourse> GetAvailableCourses(int candidateId, int? centreId, int categoryId)
        {
            return connection.Query<AvailableCourse>(
                @"GetActiveAvailableCustomisationsForCentreFiltered_V6",
                new { candidateId, centreId, categoryId },
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
            int selfAssessmentSupervisorRoleId, DateTime completeByDate)
        {
            IClockUtility clockUtility = new ClockUtility();
            DateTime startedDate = clockUtility.UtcNow;
            DateTime lastAccessed = startedDate;
            dynamic completeByDateDynamic = "";
            if (completeByDate.Year > 1753)
            {
                completeByDateDynamic = completeByDate;
            }
            var candidateAssessmentId = (int)connection.ExecuteScalar(
                @"SELECT COALESCE
                 ((SELECT ID
                  FROM    CandidateAssessments
                  WHERE (SelfAssessmentID = @selfAssessmentId) AND (CandidateID = @candidateId) AND (RemovedDate IS NULL) AND (CompletedDate IS NULL)), 0) AS ID",
                new { selfAssessmentId, candidateId }
            );

            if (candidateAssessmentId == 0)
            {
                candidateAssessmentId = connection.QuerySingle<int>(
                    @"INSERT INTO [dbo].[CandidateAssessments]
                           ([CandidateID]
                           ,[SelfAssessmentID]
                           ,[StartedDate]
                           ,[LastAccessed]
                           ,[CompleteByDate] )
                    OUTPUT INSERTED.Id
                     VALUES
                           (@candidateId,
                           @selfAssessmentId,
                           @startedDate,
                           @lastAccessed,
                           @completeByDateDynamic);",
                    new { selfAssessmentId, candidateId, startedDate, lastAccessed, completeByDateDynamic }
                );
            }

            int supervisorDelegateId = (int)connection.ExecuteScalar(
                    @"SELECT COALESCE
                 ((SELECT TOP 1 ID FROM SupervisorDelegates WHERE SupervisorAdminID = @supervisorId AND CandidateID = @candidateId), 0) AS ID",
                    new { supervisorId, candidateId }
                );
            if (supervisorDelegateId == 0 && supervisorId > 0)
            {
                supervisorDelegateId = connection.QuerySingle<int>(@"INSERT INTO SupervisorDelegates (SupervisorAdminID, DelegateEmail, CandidateID, SupervisorEmail, AddedByDelegate)
                    OUTPUT INSERTED.Id
                    SELECT @supervisorId, EmailAddress, @candidateId, @adminEmail, 0
                        FROM Candidates
                        WHERE CandidateID = @candidateId", new { supervisorId, candidateId, adminEmail });
            }

            if (candidateAssessmentId > 0 && supervisorDelegateId > 0 && selfAssessmentSupervisorRoleId > 0)
            {
                int numberOfAffectedRows = connection.Execute(
                    @"INSERT INTO CandidateAssessmentSupervisors (CandidateAssessmentID, SupervisorDelegateId, SelfAssessmentSupervisorRoleID)
                        VALUES (@candidateAssessmentId, @supervisorDelegateId, @selfAssessmentSupervisorRoleId)",
                    new { candidateAssessmentId, supervisorDelegateId, selfAssessmentSupervisorRoleId }
                );
            }

            if (candidateAssessmentId < 1)
            {
                logger.LogWarning(
                    "Not enrolled delegate on self assessment as db insert failed. " +
                    $"Self assessment id: {selfAssessmentId}, candidate id: {candidateId}"
                );
            }

            return candidateAssessmentId;
        }

        public void EnrolOnSelfAssessment(int selfAssessmentId, int candidateId)
        {
            var enrolmentExists = (int)connection.ExecuteScalar(
                @"SELECT COALESCE
                 ((SELECT ID
                  FROM    CandidateAssessments
                  WHERE (SelfAssessmentID = @selfAssessmentId) AND (CandidateID = @candidateId) AND (RemovedDate IS NULL) AND (CompletedDate IS NULL)), 0) AS ID",
                new { selfAssessmentId, candidateId }
            );

            if (enrolmentExists == 0)
            {
                enrolmentExists = connection.Execute(
                    @"INSERT INTO [dbo].[CandidateAssessments]
                           ([CandidateID]
                           ,[SelfAssessmentID])
                     VALUES
                           (@candidateId,
                           @selfAssessmentId)",
                    new { selfAssessmentId, candidateId }
                );
            }

            if (enrolmentExists < 1)
            {
                logger.LogWarning(
                    "Not enrolled delegate on self assessment as db insert failed. " +
                    $"Self assessment id: {selfAssessmentId}, candidate id: {candidateId}"
                );
            }
        }

        public int EnrolSelfAssessment(int selfAssessmentId, int candidateId)
        {
            var enrolmentExists = connection.QuerySingle<int>(
                @"SELECT COALESCE
                 ((SELECT ID
                  FROM    CandidateAssessments
                  WHERE (SelfAssessmentID = @selfAssessmentId) AND (CandidateID = @candidateId) AND (RemovedDate IS NULL) AND (CompletedDate IS NULL)), 0) AS ID",
                new { selfAssessmentId, candidateId }
            );

            if (enrolmentExists == 0)
            {
                enrolmentExists = connection.Execute(
                    @"INSERT INTO [dbo].[CandidateAssessments]
                           ([CandidateID]
                           ,[SelfAssessmentID])
                     OUTPUT Inserted.ID
                     VALUES
                           (@candidateId,
                           @selfAssessmentId)",
                    new { selfAssessmentId, candidateId }
                );
            }

            if (enrolmentExists < 1)
            {
                logger.LogWarning(
                    "Not enrolled delegate on self assessment as db insert failed. " +
                    $"Self assessment id: {selfAssessmentId}, candidate id: {candidateId}"
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
                        AND ap.DefaultContentTypeID <> 4",
                new { delegateId }
            );
        }

        public DelegateCourseInfo? GetDelegateCourseInfoByProgressId(int progressId)
        {
            return connection.QuerySingleOrDefault<DelegateCourseInfo>(
                $@"{selectDelegateCourseInfoQuery}
                    WHERE pr.ProgressID = @progressId
                        AND ap.DefaultContentTypeID <> 4",
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
                        AND ap.DefaultContentTypeID <> 4",
                new { customisationId, centreId }
            );
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
                entry => entry.ApplicationID,
                entry => entry.NumRecentProgressRecords
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
    }
}
