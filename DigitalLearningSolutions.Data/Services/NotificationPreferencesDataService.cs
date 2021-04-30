namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;

    public interface INotificationPreferencesDataService
    {
        IEnumerable<NotificationPreference> GetNotificationPreferencesForAdmin(int? adminId);
        IEnumerable<NotificationPreference> GetNotificationPreferencesForDelegate(int? delegateId);
        void SetNotificationPreferencesForAdmin(int? adminId, IEnumerable<int> notificationIds);
        void SetNotificationPreferencesForDelegate(int? delegateId, IEnumerable<int> notificationIds);
    }

    public class NotificationPreferencesDataService : INotificationPreferencesDataService
    {
        private readonly IDbConnection connection;

        public NotificationPreferencesDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<NotificationPreference> GetNotificationPreferencesForAdmin(int? adminId)
        {
            if (!adminId.HasValue)
            {
                return new List<NotificationPreference>();
            }

            return connection.Query<NotificationPreference>(
                @"DECLARE @CentreAdmin BIT, @IsCentreManager BIT, @ContentManager BIT, @ContentCreator BIT, @Supervisor BIT, @Trainer BIT

                    SELECT @CentreAdmin = CentreAdmin, @IsCentreManager = IsCentreManager, @ContentManager = ContentManager, @ContentCreator = ContentCreator, @Supervisor = Supervisor, @Trainer = Trainer
	                    FROM AdminUsers
	                    WHERE AdminID = @adminId

                    DECLARE @Whitespace VARCHAR(3)
                    SET @Whitespace = CHAR(10)+CHAR(13)+CHAR(32)

                    SELECT DISTINCT n.NotificationID, TRIM(@Whitespace FROM n.NotificationName) AS NotificationName, n.Description, CASE WHEN nu.NotificationUserID IS NOT NULL THEN 1 ELSE 0 END AS accepted
	                    FROM Notifications AS n
	                    JOIN NotificationRoles AS nr ON nr.NotificationID = n.NotificationID
	                    LEFT JOIN NotificationUsers AS nu ON nu.NotificationID = n.NotificationID AND nu.AdminUserID = @adminId
	                    WHERE (nr.RoleID = 1 AND @CentreAdmin = 1)
	                    OR (nr.RoleID = 2 AND @IsCentreManager = 1)
	                    OR (nr.RoleID = 3 AND @ContentManager = 1)
	                    OR (nr.RoleID = 4 AND @ContentCreator = 1)
	                    OR (nr.RoleID = 6 AND @Supervisor = 1)
	                    OR (nr.RoleID = 7 AND @Trainer = 1)
                        ORDER BY n.NotificationID",
                new { adminId }
            );
        }

        public IEnumerable<NotificationPreference> GetNotificationPreferencesForDelegate(int? delegateId)
        {
            if (!delegateId.HasValue)
            {
                return new List<NotificationPreference>();
            }

            return connection.Query<NotificationPreference>(
                @"DECLARE @Whitespace VARCHAR(3)
                    SET @Whitespace = CHAR(10)+CHAR(13)+CHAR(32)

                    SELECT DISTINCT n.NotificationID, TRIM(@Whitespace FROM n.NotificationName) AS NotificationName, n.Description, CASE WHEN nu.NotificationUserID IS NOT NULL THEN 1 ELSE 0 END AS accepted
	                    FROM Notifications AS n
	                    JOIN NotificationRoles AS nr ON nr.NotificationID = n.NotificationID
	                    LEFT JOIN NotificationUsers AS nu ON nu.NotificationID = n.NotificationID AND nu.CandidateID = @delegateId
	                    WHERE nr.RoleID = 5
                        ORDER BY n.NotificationID",
                new { delegateId }
            );
        }

        public void SetNotificationPreferencesForAdmin(int? adminId, IEnumerable<int> notificationIds)
        {
            if (!adminId.HasValue) return;

            connection.Execute(
                @"DELETE FROM NotificationUsers
                    WHERE AdminUserId = @adminId",
                new { adminId }
            );

            var notificationIdsWithAdminId = notificationIds.Select(notificationId => new { adminId, notificationId });

            connection.Execute(
                @"INSERT INTO NotificationUsers (NotificationId, AdminUserId)
                VALUES (@notificationId, @adminId)",
                notificationIdsWithAdminId
            );
        }

        public void SetNotificationPreferencesForDelegate(int? delegateId, IEnumerable<int> notificationIds)
        {
            if (!delegateId.HasValue) return;

            connection.Execute(
                @"DELETE FROM NotificationUsers
                    WHERE CandidateId = @delegateId",
                new { delegateId }
            );

            var notificationIdsWithDelegateId = notificationIds.Select(notificationId => new { delegateId, notificationId });

            connection.Execute(
                @"INSERT INTO NotificationUsers (NotificationId, CandidateId)
                VALUES (@notificationId, @delegateId)",
                notificationIdsWithDelegateId
            );
        }
    }
}
