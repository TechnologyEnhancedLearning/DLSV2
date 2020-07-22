namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;

    public interface ICourseService
    {
        IEnumerable<CurrentCourse> GetCurrentCourses(int CandidateID);
        IEnumerable<Course> GetCompletedCourses();
        IEnumerable<Course> GetAvailableCourses();
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
            return connection.Query<CurrentCourse>($@"
                EXEC GetCurrentCoursesForCandidate_V2 @CandidateId = '{CandidateID}'
            ");
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
    }
}
