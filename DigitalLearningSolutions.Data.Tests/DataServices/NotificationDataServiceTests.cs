namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class NotificationDataServiceTests
    {
        private NotificationDataService notificationDataService = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
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
                DelegateEmail = "hcta@egviomklw.",
                DelegateName = "xxxxx xxxxxxxxx",
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
            var progressCompletionData = new ProgressCompletionData
            {
                CentreId = 237,
                CourseName = "Entry Level - Win XP, Office 2003/07 OLD - Standard",
                AdminEmail = null,
                SessionId = 429,
            };

            // when
            var result = notificationDataService.GetProgressCompletionData(100, 103, 236);

            // then
            result.Should().BeEquivalentTo(progressCompletionData);
        }

        [Test]
        public void GetProgressCompletionData_returns_admin_email_when_there_is_one()
        {
            // Given
            var progressCompletionData = new ProgressCompletionData
            {
                CentreId = 374,
                CourseName = "Level 3 - Microsoft Word 2010  - LH TEST NEW COURSE PUBLISHED",
                AdminEmail = "hcoayru@lmgein.",
                SessionId = 0,
            };

            // when
            var result = notificationDataService.GetProgressCompletionData(276626, 293518, 27312);

            // then
            result.Should().BeEquivalentTo(progressCompletionData);
        }
    }
}
