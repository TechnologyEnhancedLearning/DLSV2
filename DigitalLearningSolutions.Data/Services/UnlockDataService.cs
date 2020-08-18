namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;

    public interface IUnlockDataService
    {
        UnlockData? GetUnlockData(int progressId);
    }

    public class UnlockDataService : IUnlockDataService
    {
        private readonly IDbConnection connection;

        public UnlockDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public UnlockData? GetUnlockData(int progressId)
        {
            return connection.Query<UnlockData?>(
                @"SELECT TOP (1)
                        candidates.EmailAddress AS DelegateEmail,
                        candidates.FirstName + ' ' + candidates.LastName AS DelegateName,
                        centres.ContactForename,
                        COALESCE (centres.NotifyEmail, COALESCE (centres.ContactEmail, centres.pwEmail)) AS ContactEmail,
                        applications.ApplicationName + ' - ' + customisations.CustomisationName AS CourseName,
                        progress.CustomisationID AS CustomisationId
                    FROM Progress AS progress
                        INNER JOIN Candidates AS candidates
                            ON progress.CandidateID = candidates.CandidateID
                        INNER JOIN Centres AS centres
                            ON candidates.CentreID = centres.CentreID
                        INNER JOIN Customisations AS customisations
                            ON progress.CustomisationID = customisations.CustomisationID
                        INNER JOIN Applications AS applications
                            ON customisations.ApplicationID = applications.ApplicationID
                    WHERE  (progress.ProgressID = @progressID)",
            new { progressId }
            ).FirstOrDefault();
        }
    }
}
