namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;

    public interface ICourseDelegatesDataService
    {
        IEnumerable<CourseDelegate> GetDelegatesOnCourse(int customisationId, int centreId);
    }

    public class CourseDelegatesDataService : ICourseDelegatesDataService
    {
        private readonly IDbConnection connection;

        public CourseDelegatesDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<CourseDelegate> GetDelegatesOnCourse(int customisationId, int centreId)
        {
            return connection.Query<CourseDelegate>(
                @"SELECT
                        c.CandidateID AS DelegateId,
                        c.CandidateNumber,
                        c.FirstName,
                        c.LastName,
                        c.EmailAddress,
                        c.Active,
                        p.ProgressID,
                        p.PLLocked AS Locked,
                        p.SubmittedTime AS LastUpdated,
                        c.DateRegistered AS Enrolled,
                        p.CompleteByDate AS CompleteBy
                    FROM Candidates AS c
                    INNER JOIN Progress AS p ON p.CandidateID = c.CandidateID
                    WHERE c.CentreID = @centreId
                        AND p.CustomisationID = @customisationId",
                new { customisationId, centreId }
            );
        }
    }
}
