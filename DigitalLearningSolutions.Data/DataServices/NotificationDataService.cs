﻿namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Notifications;

    public interface INotificationDataService
    {
        UnlockData? GetUnlockData(int progressId);

        ProgressCompletionData? GetProgressCompletionData(int progressId, int candidateId, int customisationId);

        IEnumerable<NotificationRecipient> GetAdminRecipientsForCentreNotification(int centreId, int notificationId);

        IEnumerable<int> GetRoleBasedNotifications(int isCentreManager, int isContentManager, int isContentCreator);

        void SubscribeDefaultNotifications(int candidateId);
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
                        candidates.CandidateID AS DelegateId,
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
                @"SELECT centres.CentreID, applications.ApplicationName + ' - ' + customisations.CustomisationName AS CourseName,
                                 (SELECT TOP (1) au.AdminID
                                 FROM    AdminUsers AS au INNER JOIN
                                              Progress AS p ON au.AdminID = p.EnrolledByAdminID INNER JOIN
                                              NotificationUsers AS nu ON au.AdminID = nu.AdminUserID
                                 WHERE (nu.NotificationID = 6) AND (p.ProgressID = @progressId) AND (au.Active = 1)) AS AdminId, customisations.NotificationEmails AS CourseNotificationEmail,
                                 (SELECT MAX(SessionID) AS Expr1
                                 FROM    Sessions
                                 WHERE (CandidateID = @candidateId) AND (CustomisationID = @customisationId)) AS SessionID, CAST(CASE WHEN nu1.NotificationID IS NOT NULL THEN 1 ELSE 0 END AS bit) AS DelegateSubscribedToCompletionNotification
                    FROM   Progress AS progress INNER JOIN
                             Candidates AS candidates ON progress.CandidateID = candidates.CandidateID INNER JOIN
                             Centres AS centres ON candidates.CentreID = centres.CentreID INNER JOIN
                             Customisations AS customisations ON progress.CustomisationID = customisations.CustomisationID INNER JOIN
                             Applications AS applications ON customisations.ApplicationID = applications.ApplicationID LEFT OUTER JOIN
                             NotificationUsers AS nu1 ON nu1.CandidateID = candidates.CandidateID AND nu1.NotificationID = 11
                    WHERE (progress.ProgressID = @progressId) AND (applications.ArchivedDate IS NULL) AND (applications.DefaultContentTypeID <> 4)",
                new { progressId, candidateId, customisationId }
            );
        }

        public IEnumerable<NotificationRecipient> GetAdminRecipientsForCentreNotification(int centreId, int notificationId)
        {
            var recipients = connection.Query<NotificationRecipient>(

                @"SELECT au.Forename as FirstName, au.Surname as LastName, au.Email
                    FROM     NotificationUsers AS nu INNER JOIN
                         AdminUsers AS au ON nu.AdminUserID = au.AdminID AND au.Active = 1
                    WHERE  (nu.NotificationID = @notificationId) AND (au.CentreID = @centreId)",
                new { notificationId, centreId }
                );
            return recipients;
        }

        public IEnumerable<int> GetRoleBasedNotifications(int isCentreManager, int isContentManager, int isContentCreator)
        {
            return connection.Query<int>(
                    @"SELECT NR.NotificationID from NotificationRoles AS NR INNER JOIN Notifications AS N ON NR.NotificationID = N.NotificationID WHERE ((@isCentreManager = 1 AND NR.RoleID = 2) OR (@isContentManager = 1 AND NR.RoleID = 3) OR (@isContentCreator = 1 AND NR.RoleID = 4) ) AND N.AutoOptIn = 1",
                    new { isCentreManager, isContentManager, isContentCreator }
                ).AsEnumerable();
        }

        public void SubscribeDefaultNotifications(int candidateId)
        {
            connection.Execute(
            @"INSERT INTO NotificationUsers (
                    NotificationID,CandidateID)
                    (SELECT notificationroles.NotificationID,@candidateId FROM NotificationRoles AS notificationroles
                    INNER JOIN Notifications AS notifications ON notificationroles.NotificationID = notifications.NotificationID
                    WHERE notificationroles.RoleID = 5 AND notifications.AutoOptIn=1)", new { candidateId });
        }

    }
}
