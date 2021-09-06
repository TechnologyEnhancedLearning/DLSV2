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

        public CourseDelegatesDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<CourseDelegate> GetDelegatesOnCourse(int customisationId, int centreId)
        {
            return connection.Query<CourseDelegate>(
                $@"SELECT
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
                        p.CompleteByDate AS CompleteBy,
                        p.RemovedDate,
                        p.Completed,
                        {AllAttemptsQuery},
                        {AttemptsPassedQuery}
                    FROM Candidates AS c
                    INNER JOIN Progress AS p ON p.CandidateID = c.CandidateID
                    INNER JOIN Customisations cu ON cu.CustomisationID = p.CustomisationID
                    WHERE c.CentreID = @centreId
                        AND p.CustomisationID = @customisationId",
                new { customisationId, centreId }
            );
        }
    }
}
