namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Courses;
    using Microsoft.Extensions.Logging;

    public interface ICourseService
    {
        IEnumerable<CurrentCourse> GetCurrentCourses(int candidateId);
        IEnumerable<CompletedCourse> GetCompletedCourses(int candidateId);
        IEnumerable<AvailableCourse> GetAvailableCourses(int candidateId, int? centreId);
        void SetCompleteByDate(int progressId, int candidateId, DateTime? completeByDate);
        void RemoveCurrentCourse(int progressId, int candidateId);
        void EnrolOnSelfAssessment(int selfAssessmentId, int candidateId);
    }

    public class CourseService : ICourseService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<CourseService> logger;

        public CourseService(IDbConnection connection, ILogger<CourseService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public IEnumerable<CurrentCourse> GetCurrentCourses(int candidateId)
        {
            return connection.Query<CurrentCourse>("GetCurrentCoursesForCandidate_V2", new { candidateId }, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<CompletedCourse> GetCompletedCourses(int candidateId)
        {
            return connection.Query<CompletedCourse>("GetCompletedCoursesForCandidate", new { candidateId }, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<AvailableCourse> GetAvailableCourses(int candidateId, int? centreId)
        {
            return connection.Query<AvailableCourse>(
                @"GetActiveAvailableCustomisationsForCentreFiltered_V5", new { candidateId, centreId },
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
    }
}
