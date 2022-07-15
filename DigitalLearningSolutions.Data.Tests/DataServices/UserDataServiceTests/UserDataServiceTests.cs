namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
{
    using System;
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
        public void GetUserIdFromDelegateId_returns_expected_userId_for_delegateId()
        {
            // When
            var result = userDataService.GetUserIdFromDelegateId(1);

            // Then
            result.Should().Be(48157);
        }

        [Test]
        public void GetUserAccountById_returns_expected_user_account()
        {
            // When
            var result = userDataService.GetUserAccountById(2);

            // Then
            result.Should().BeEquivalentTo(
                UserTestHelper.GetDefaultUserAccount(emailVerified: DateTime.Parse("2022-04-27 16:28:55.247"))
            );
        }

        [Test]
        public void GetUserAccountByEmailAddress_returns_expected_user_account()
        {
            // When
            var result = userDataService.GetUserAccountByEmailAddress("test@gmail.com");

            // Then
            result.Should().BeEquivalentTo(
                UserTestHelper.GetDefaultUserAccount(emailVerified: DateTime.Parse("2022-04-27 16:28:55.247"))
            );
        }

        [Test]
        public void GetUserIdByAdminId_returns_expected_user_id()
        {
            // When
            var result = userDataService.GetUserIdByAdminId(7);

            // Then
            result.Should().Be(2);
        }

        [Test]
        [TestCase("test@gmail.com", true)]
        [TestCase("not_an_email_in_the_database", false)]
        public void PrimaryEmailIsInUse_returns_expected_value(string email, bool expectedResult)
        {
            // When
            var result = userDataService.PrimaryEmailIsInUse(email);

            // Then
            result.Should().Be(expectedResult);
        }

        [Test]
        [TestCase("test@gmail.com", -1, true)]
        [TestCase("test@gmail.com", 2, false, TestName = "User id matches email")]
        [TestCase("not_an_email_in_the_database", 2, false)]
        public void PrimaryEmailIsInUseByOtherUser_returns_expected_value(string email, int userId, bool expectedResult)
        {
            // When
            var result = userDataService.PrimaryEmailIsInUseByOtherUser(email, userId);

            // Then
            result.Should().Be(expectedResult);
        }
    }
}
