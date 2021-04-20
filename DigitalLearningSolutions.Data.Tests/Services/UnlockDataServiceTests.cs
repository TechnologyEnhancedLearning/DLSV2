﻿namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using NUnit.Framework;
    using FluentAssertions;

    public class UnlockDataServiceTests
    {
        private NotificationDataService unlockDataService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            unlockDataService = new NotificationDataService(connection);
        }

        [Test]
        public void Get_unlock_data_returns_correct_results()
        {
            // When
            const int progressId = 173218;
            var result = unlockDataService.GetUnlockData(progressId);

            // Then
            var expectedUnlockData = new UnlockData
            {
                DelegateEmail = "email@test.com",
                DelegateName = "Firstname Test",
                ContactForename = "xxxxx",
                ContactEmail = "notify@test.com",
                CourseName = "Office 2013 Essentials for the Workplace - Erin Test 01",
                CustomisationId = 15853
            };
            result.Should().BeEquivalentTo(expectedUnlockData);
        }
    }
}
