namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;

    public interface ICourseDelegatesDataService
    {
        IEnumerable<CourseDelegate> GetDelegatesOnCourse(int customisationId, int centreId);

        IEnumerable<CourseDelegateForExport> GetDelegatesOnCourseForExport(int customisationId, int centreId);
    }

    public class CourseDelegatesDataService : ICourseDelegatesDataService
    {
        private const string AllAttemptsQuery =
            @"(SELECT COUNT(aa.AssessAttemptID)
                FROM dbo.AssessAttempts AS aa
                INNER JOIN dbo.Candidates AS can ON can.CandidateID = aa.CandidateID
                WHERE aa.CustomisationID = cu.CustomisationID AND aa.[Status] IS NOT NULL
                AND can.CentreID = @centreId AND can.CandidateId = c.CandidateId) AS AllAttempts";

        private const string AttemptsPassedQuery =
            @"(SELECT COUNT(aa.AssessAttemptID)
                FROM dbo.AssessAttempts AS aa
                INNER JOIN dbo.Candidates AS can ON can.CandidateID = aa.CandidateID
                WHERE aa.CustomisationID = cu.CustomisationID AND aa.[Status] = 1
                AND can.CentreID = @centreId AND can.CandidateId = c.CandidateId) AS AttemptsPassed";

        private readonly IDbConnection connection;

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
                        c.HasBeenPromptedForPrn,
                        c.ProfessionalRegistrationNumber,
                        p.ProgressID,
                        p.PLLocked AS Locked,
                        p.SubmittedTime AS LastUpdated,
                        c.DateRegistered AS Enrolled,
                        p.CompleteByDate,
                        p.RemovedDate,
                        p.Completed,
                        p.CustomisationId,
                        TRIM(p.Answer1) AS Answer1,
                        TRIM(p.Answer2) AS Answer2,
                        TRIM(p.Answer3) AS Answer3,
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

        public IEnumerable<CourseDelegateForExport> GetDelegatesOnCourseForExport(int customisationId, int centreId)
        {
            return connection.Query<CourseDelegateForExport>(
                $@"SELECT
                        c.CandidateID AS DelegateId,
                        c.CandidateNumber,
                        c.FirstName,
                        c.LastName,
                        c.EmailAddress,
                        c.Active,
                        c.Answer1,
                        c.Answer2,
                        c.Answer3,
                        c.Answer4,
                        c.Answer5,
                        c.Answer6,
                        p.ProgressID,
                        p.PLLocked AS Locked,
                        p.SubmittedTime AS LastUpdated,
                        c.DateRegistered AS Enrolled,
                        p.CompleteByDate,
                        p.RemovedDate,
                        p.Completed,
                        p.CustomisationId,
                        p.LoginCount,
                        p.Duration,
                        p.DiagnosticScore,
                        p.Answer1 AS CourseAnswer1,
                        p.Answer2 AS CourseAnswer2,
                        p.Answer3 AS CourseAnswer3,
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
