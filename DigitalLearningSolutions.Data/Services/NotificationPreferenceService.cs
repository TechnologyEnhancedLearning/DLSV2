namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;

    public interface INotificationPreferenceService
    {
        IEnumerable<NotificationPreference> GetNotificationPreferencesForAdmin(int adminId);
        IEnumerable<NotificationPreference> GetNotificationPreferencesForDelegate(int delegateId);
    }

    public class NotificationPreferenceService : INotificationPreferenceService
    {
        private readonly IDbConnection connection;

        public NotificationPreferenceService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<NotificationPreference> GetNotificationPreferencesForAdmin(int adminId)
        {
            return connection.Query<NotificationPreference>(
                @"DECLARE @CentreAdmin BIT, @IsCentreManager BIT, @ContentManager BIT, @ContentCreator BIT, @Supervisor BIT, @Trainer BIT

                    SELECT @CentreAdmin = CentreAdmin, @IsCentreManager = IsCentreManager, @ContentManager = ContentManager, @ContentCreator = ContentCreator, @Supervisor = Supervisor, @Trainer = Trainer
	                    FROM AdminUsers
	                    WHERE AdminID = @adminId

                    SELECT DISTINCT n.NotificationName, n.Description, CASE WHEN nu.NotificationUserID IS NOT NULL THEN 1 ELSE 0 END AS accepted
	                    FROM Notifications AS n
	                    JOIN NotificationRoles AS nr ON nr.NotificationID = n.NotificationID
	                    LEFT JOIN NotificationUsers AS nu ON nu.NotificationID = n.NotificationID AND nu.AdminUserID = @adminId
	                    WHERE (nr.RoleID = 1 AND @CentreAdmin = 1)
	                    OR (nr.RoleID = 2 AND @IsCentreManager = 1)
	                    OR (nr.RoleID = 3 AND @ContentManager = 1)
	                    OR (nr.RoleID = 4 AND @ContentCreator = 1)
	                    OR (nr.RoleID = 6 AND @Supervisor = 1)
	                    OR (nr.RoleID = 7 AND @Trainer = 1)",
                new { adminId }
            );
        }

        public IEnumerable<NotificationPreference> GetNotificationPreferencesForDelegate(int delegateId)
        {
            return connection.Query<NotificationPreference>(
                @"SELECT DISTINCT n.NotificationName, n.Description, CASE WHEN nu.NotificationUserID IS NOT NULL THEN 1 ELSE 0 END AS accepted
	                    FROM Notifications AS n
	                    JOIN NotificationRoles AS nr ON nr.NotificationID = n.NotificationID
	                    LEFT JOIN NotificationUsers AS nu ON nu.NotificationID = n.NotificationID AND nu.CandidateID = @delegateId
	                    WHERE nr.RoleID = 5",
                new { delegateId }
            );
        }
    }
}
