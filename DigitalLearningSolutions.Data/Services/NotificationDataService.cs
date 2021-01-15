namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Frameworks;

    public interface INotificationDataService
    {
        UnlockData? GetUnlockData(int progressId);
        CollaboratorNotification GetCollaboratorNotification(int adminId, int frameworkId, int invitedByAdminId);
    }

    public class NotificationDataService : INotificationDataService
    {
        private readonly IDbConnection connection;

        public NotificationDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public CollaboratorNotification GetCollaboratorNotification(int adminId, int frameworkId, int invitedByAdminId)
        {
            return connection.Query<CollaboratorNotification>(

                @"SELECT fc.FrameworkID, fc.AdminID, fc.CanModify, au.Email, au.Forename, au.Surname, CASE WHEN fc.CanModify = 1 THEN 'Contributor' ELSE 'Reviewer' END AS FrameworkRole, f.FrameworkName,
                    (SELECT Forename + ' ' + Surname AS Expr1
                         FROM    AdminUsers AS au1
                         WHERE (AdminID = @invitedByAdminId)) AS InvitedByName,
                    (SELECT Email
                        FROM    AdminUsers AS au2
                        WHERE (AdminID = @invitedByAdminId)) AS InvitedByEmail
                    FROM   FrameworkCollaborators AS fc INNER JOIN
                        AdminUsers AS au ON fc.AdminID = au.AdminID INNER JOIN
                        Frameworks AS f ON fc.FrameworkID = f.ID
                    WHERE (fc.FrameworkID = @frameworkId) AND (fc.AdminID = @adminId)",
                new { invitedByAdminId, frameworkId, adminId }
                ).FirstOrDefault();
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
