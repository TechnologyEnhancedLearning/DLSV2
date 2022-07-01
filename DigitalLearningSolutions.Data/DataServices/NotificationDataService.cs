namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;

    public interface INotificationDataService
    {
        UnlockData? GetUnlockData(int progressId);

        ProgressCompletionData? GetProgressCompletionData(int progressId, int candidateId, int customisationId);
    }

    public class NotificationDataService : INotificationDataService
    {
        private readonly IDbConnection connection;

        public NotificationDataService(IDbConnection connection)
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
                    WHERE  (progress.ProgressID = @progressID)
                        AND applications.DefaultContentTypeID <> 4",
                new { progressId }
            ).FirstOrDefault();
        }

        public ProgressCompletionData? GetProgressCompletionData(int progressId, int candidateId, int customisationId)
        {
            return connection.QuerySingle<ProgressCompletionData?>(
                @"SELECT
                        centres.CentreID,
                        applications.ApplicationName + ' - ' + customisations.CustomisationName AS CourseName,
                        (SELECT TOP (1) au.Email
                            FROM AdminUsers AS au
                            INNER JOIN Progress AS p ON au.AdminID = p.EnrolledByAdminID
                            INNER JOIN NotificationUsers AS nu ON au.AdminID = nu.AdminUserID
                        WHERE nu.NotificationID = 6
                            AND p.ProgressID = @progressId
                            AND au.Active = 1) AS AdminEmail,
                        customisations.NotificationEmails AS CourseNotificationEmail,
                        (SELECT MAX(SessionID)
                            FROM Sessions
                            WHERE CandidateID = @candidateId
                            AND CustomisationID = @customisationId) AS SessionID
                    FROM Progress AS progress
                        INNER JOIN Candidates AS candidates
                            ON progress.CandidateID = candidates.CandidateID
                        INNER JOIN Centres AS centres
                            ON candidates.CentreID = centres.CentreID
                        INNER JOIN Customisations AS customisations
                            ON progress.CustomisationID = customisations.CustomisationID
                        INNER JOIN Applications AS applications
                            ON customisations.ApplicationID = applications.ApplicationID
                    WHERE (progress.ProgressID = @progressId) AND applications.ArchivedDate IS NULL
                        AND applications.DefaultContentTypeID <> 4",
                new { progressId, candidateId, customisationId }
            );
        }
    }
}
