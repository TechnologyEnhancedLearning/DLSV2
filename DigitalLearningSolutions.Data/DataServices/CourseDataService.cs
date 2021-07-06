namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
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
    }

    public class CourseDataService : ICourseDataService
    {
        private const string DelegateCountQuery =
            @"(SELECT COUNT(CandidateID)
                FROM dbo.Progress AS pr
                WHERE pr.CustomisationID = cu.CustomisationID) AS DelegateCount";

        private const string CompletedCountQuery =
            @"(SELECT COUNT(CandidateID)
                FROM dbo.Progress AS pr
                WHERE pr.CustomisationID = cu.CustomisationID AND pr.Completed IS NOT NULL) AS CompletedCount";

        private const string AllAttemptsQuery =
            @"(SELECT COUNT(AssessAttemptID)
                FROM dbo.AssessAttempts AS aa
                WHERE aa.CustomisationID = cu.CustomisationID AND aa.[Status] IS NOT NULL) AS AllAttempts";

        private const string AttemptsPassedQuery =
            @"(SELECT COUNT(AssessAttemptID)
                FROM dbo.AssessAttempts AS aa
                WHERE aa.CustomisationID = cu.CustomisationID AND aa.[Status] = 1) AS AttemptsPassed";

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
                        ap.ArchivedDate,
                        ap.ApplicationName,
                        cu.CustomisationName,
                        {DelegateCountQuery},
                        {CompletedCountQuery},
                        {AllAttemptsQuery},
                        {AttemptsPassedQuery}
                    FROM dbo.Customisations AS cu
                    INNER JOIN dbo.CentreApplications AS ca ON ca.ApplicationID = cu.ApplicationID
                    INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = ca.ApplicationID
                    WHERE (ap.CourseCategoryID = @categoryId OR @categoryId = 0) 
                        AND (cu.CentreID = @centreId OR (cu.AllCentres = 1 AND ca.Active = 1))
                        AND ca.CentreID = @centreId",
                new { centreId, categoryId }
            );
        }
    }
}
