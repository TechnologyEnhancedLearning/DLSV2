namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System;
    using System.Collections.Generic;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.Data.SqlClient;

    public class SystemNotificationTestHelper
    {
        private readonly SqlConnection connection;
        private static readonly IClockUtility ClockUtility = new ClockUtility();

        public SystemNotificationTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<int> GetSystemNotificationAcknowledgementsForAdmin(int adminId)
        {
            return connection.Query<int>(
                @"SELECT SANotificationId
                        FROM SANotificationAcknowledgements
                        WHERE AdminUserID = @adminId",
                new { adminId }
            );
        }

        public void CreateNewSystemNotification(SystemNotification notification)
        {
            connection.Execute(
                @"SET IDENTITY_INSERT dbo.SANotifications ON
                    INSERT INTO SANotifications (SANotificationID, SubjectLine, BodyHtml, ExpiryDate, DateAdded, TargetUserRoleID)
                    VALUES (@systemNotificationId, @subjectLine, @bodyHtml, @expiryDate, @dateAdded, @targetUserRoleId)
                    SET IDENTITY_INSERT dbo.SANotifications OFF",
                new
                {
                    notification.SystemNotificationId,
                    notification.SubjectLine,
                    notification.BodyHtml,
                    notification.ExpiryDate,
                    notification.DateAdded,
                    notification.TargetUserRoleId
                }
            );
        }

        public static SystemNotification GetDefaultSystemNotification(
            int id = 1,
            string subject = "test subject",
            string bodyHtml = "<p>test body</p>",
            DateTime? expiryDate = null,
            DateTime? dateAdded = null,
            int targetUserRoleId = 1
        )
        {
            return new SystemNotification(
                id,
                subject,
                bodyHtml,
                expiryDate,
                dateAdded ?? ClockUtility.UtcNow,
                targetUserRoleId
            );
        }
    }
}
