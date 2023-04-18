namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public class NotificationDataServiceTests
    {
        private SqlConnection connection = null!;
        private NotificationDataService notificationDataService = null!;

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            notificationDataService = new NotificationDataService(connection);
        }

        [Test]
        public void Get_unlock_data_returns_correct_results()
        {
            // When
            const int progressId = 173218;
            var result = notificationDataService.GetUnlockData(progressId);

            // Then
            var expectedUnlockData = new UnlockData
            {
                DelegateId = 1,
                ContactForename = "xxxxx",
                ContactEmail = "e@1htrnkisv.wa",
                CourseName = "Office 2013 Essentials for the Workplace - Erin Test 01",
                CustomisationId = 15853,
            };
            result.Should().BeEquivalentTo(expectedUnlockData);
        }

        [Test]
        public void GetProgressCompletionData_returns_data_correctly()
        {
            // Given
            using var transaction = new TransactionScope();

            const int customisationId = 236;
            const string courseNotificationEmail = "test@email.com";
            connection.Execute(
                @"UPDATE Customisations
                        SET NotificationEmails = @courseNotificationEmail
                        WHERE CustomisationID = @customisationId",
                new { customisationId, courseNotificationEmail }
            );

            var progressCompletionData = new ProgressCompletionData
            {
                CentreId = 237,
                CourseName = "Entry Level - Win XP, Office 2003/07 OLD - Standard",
                AdminId = null,
                CourseNotificationEmail = courseNotificationEmail,
                SessionId = 429,
            };

            // When
            var result = notificationDataService.GetProgressCompletionData(100, 103, customisationId);

            // Then
            result.Should().BeEquivalentTo(progressCompletionData);
        }

        [Test]
        public void GetProgressCompletionData_returns_admin_id_when_there_is_one()
        {
            // Given
            var progressCompletionData = new ProgressCompletionData
            {
                CentreId = 374,
                CourseName = "Level 3 - Microsoft Word 2010  - LH TEST NEW COURSE PUBLISHED",
                AdminId = 4106,
                CourseNotificationEmail = null,
                SessionId = 0,
            };

            // When
            var result = notificationDataService.GetProgressCompletionData(276626, 293518, 27312);

            // Then
            result.Should().BeEquivalentTo(progressCompletionData);
        }
    }
}
