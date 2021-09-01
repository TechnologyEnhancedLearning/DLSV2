namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;

    public interface ISystemNotificationsDataService
    {
        public IEnumerable<SystemNotification> GetUnacknowledgedSystemNotifications(int adminId);

        public void AcknowledgeNotification(int notificationId, int adminId);
    }

    public class SystemNotificationsDataService : ISystemNotificationsDataService
    {
        private readonly IDbConnection connection;

        public SystemNotificationsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<SystemNotification> GetUnacknowledgedSystemNotifications(int adminId)
        {
            return connection.Query<SystemNotification>(
                @"SELECT
                        SANotificationID AS SystemNotificationId,
                        SubjectLine,
                        BodyHtml,
                        ExpiryDate,
                        DateAdded,
                        TargetUserRoleID
                    FROM dbo.SANotifications
                    WHERE SANotificationID NOT IN
                        (SELECT SANotificationID FROM dbo.SANotificationAcknowledgements WHERE AdminUserID = @adminId)
                        AND (ExpiryDate IS NULL OR ExpiryDate > GETUTCDATE())
                    ORDER BY DateAdded DESC",
                new { adminId }
            );
        }

        public void AcknowledgeNotification(int notificationId, int adminId)
        {
            connection.Execute(
                @"INSERT INTO dbo.SANotificationAcknowledgements (SANotificationID, AdminUserID)
                    VALUES (@notificationId, @adminId)",
                new { notificationId, adminId }
            );
        }
    }
}
