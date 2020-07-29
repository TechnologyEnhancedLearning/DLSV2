namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;

    public interface ICourseService
    {
        IEnumerable<CurrentCourse> GetCurrentCourses(int CandidateID);
        IEnumerable<Course> GetCompletedCourses();
        IEnumerable<Course> GetAvailableCourses();
        void SetCompleteByDate(int progressId, int candidateId, DateTime? completeByDate);
        void RemoveCurrentCourse(int progressId, int candidateId);
    }

    public class CourseService : ICourseService
    {
        private readonly IDbConnection connection;

        public CourseService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<CurrentCourse> GetCurrentCourses(int CandidateID)
        {
            return connection.Query<CurrentCourse>("GetCurrentCoursesForCandidate_V2", new { CandidateID }, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<Course> GetCompletedCourses()
        {
            return connection.Query<Course>(@"
                SELECT ApplicationID AS Id, ApplicationName AS Name FROM Applications WHERE CreatedById = 466
            ");
        }

        public IEnumerable<Course> GetAvailableCourses()
        {
            return connection.Query<Course>(@"
                SELECT ApplicationID AS Id, ApplicationName AS Name FROM Applications WHERE CreatedById = 2223
            ");
        }

        public void SetCompleteByDate(int progressId, int candidateId, DateTime? completeByDate)
        {
            connection.Execute(
                @"UPDATE Progress
                        SET CompleteByDate = @date
                        WHERE ProgressID = @progressId
                          AND CandidateID = @candidateId",
                new { date = completeByDate, progressId, candidateId }
                );
        }

        public void RemoveCurrentCourse(int progressId, int candidateId)
        {
            connection.Execute(
            @"UPDATE Progress
                    SET RemovedDate = getUTCDate(),
                        RemovalMethodID = 1
                    WHERE ProgressID = @progressId
                      AND CandidateID = @candidateId
                ",
            new { progressId, candidateId }
            );
        }
    }
}
