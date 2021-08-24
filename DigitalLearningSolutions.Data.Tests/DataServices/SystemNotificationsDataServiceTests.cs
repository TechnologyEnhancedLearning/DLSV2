namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class SystemNotificationsDataServiceTests
    {
        private SystemNotificationsDataService systemNotificationsDataService = null!;
        private SystemNotificationTestHelper systemNotificationTestHelper = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            systemNotificationsDataService = new SystemNotificationsDataService(connection);
            systemNotificationTestHelper = new SystemNotificationTestHelper(connection);
        }

        [Test]
        public void AcknowledgeSystemNotification_creates_new_record()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Given
                const int notificationId = 1;
                const int adminId = 5;

                // When
                systemNotificationsDataService.AcknowledgeNotification(notificationId, adminId);
                var acknowledgedNotificationIds =
                    systemNotificationTestHelper.GetSystemNotificationAcknowledgementsForAdmin(adminId);

                // Then
                acknowledgedNotificationIds.Should().Contain(notificationId);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetUnacknowledgedSystemNotifications_returns_correct_notifications()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Given
                const int adminId = 5;
                var currentTime = DateTime.UtcNow.Date;
                var neverExpiresNotification = new SystemNotification(
                    100,
                    "Never expires test",
                    "body",
                    null,
                    currentTime,
                    1
                );
                var expiredYesterdayNotification = new SystemNotification(
                    102,
                    "Expired yesterday test",
                    "body",
                    currentTime.AddDays(-1),
                    currentTime,
                    1
                );
                var expiresTomorrowNotification = new SystemNotification(
                    103,
                    "Expires tomorrow test",
                    "body",
                    currentTime.AddDays(1),
                    currentTime,
                    1
                );
                var alreadyAcknowledgedNotification = new SystemNotification(
                    104,
                    "Already acknowledged test",
                    "body",
                    null,
                    currentTime,
                    1
                );
                systemNotificationTestHelper.CreateNewSystemNotification(neverExpiresNotification);
                systemNotificationTestHelper.CreateNewSystemNotification(expiredYesterdayNotification);
                systemNotificationTestHelper.CreateNewSystemNotification(expiresTomorrowNotification);
                systemNotificationTestHelper.CreateNewSystemNotification(alreadyAcknowledgedNotification);
                systemNotificationsDataService.AcknowledgeNotification(104, adminId);

                // When
                var systemNotificationsToShow =
                    systemNotificationsDataService.GetUnacknowledgedSystemNotifications(adminId).ToList();

                // Then
                using (new AssertionScope())
                {
                    systemNotificationsToShow.Should().HaveCount(2);
                    systemNotificationsToShow.Should().ContainEquivalentOf(neverExpiresNotification);
                    systemNotificationsToShow.Should().ContainEquivalentOf(expiresTomorrowNotification);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }
    }
}
