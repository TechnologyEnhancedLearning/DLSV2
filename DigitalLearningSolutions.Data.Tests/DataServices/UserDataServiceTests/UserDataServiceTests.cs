﻿namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Mappers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public partial class UserDataServiceTests
    {
        private SqlConnection connection = null!;
        private IUserDataService userDataService = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MapperHelper.SetUpFluentMapper();
        }

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            userDataService = new UserDataService(connection);
        }

        [Test]
        [TestCase("email@test.com", 61188)]
        [TestCase("ES2", 1)]
        public void GetUserIdFromUsername_returns_expected_id_for_username(string username, int expectedUserId)
        {
            // When
            var result = userDataService.GetUserIdFromUsername(username);

            // Then
            result.Should().Be(expectedUserId);
        }

        [Test]
        public void GetUserAccountById_returns_expected_user_account()
        {
            // When
            var result = userDataService.GetUserAccountById(2);

            // Then
            result.Should().BeEquivalentTo(UserTestHelper.GetDefaultUserAccount());
        }
    }
}
