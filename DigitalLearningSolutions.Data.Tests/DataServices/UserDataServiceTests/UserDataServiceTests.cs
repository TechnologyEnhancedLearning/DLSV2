namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
{
    using System;
    using System.Transactions;
    using Dapper;
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
        public void GetUserAccountByPrimaryEmail_returns_expected_user_account()
        {
            // When
            var result = userDataService.GetUserAccountByPrimaryEmail("test@gmail.com");

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
        public void SetPrimaryEmailAndActivate_sets_primary_email_and_activates_user()
        {
            using var transaction = new TransactionScope();

            // Given
            const int userId = 2;
            const string primaryEmail = "primary@email.com";

            // When
            connection.Execute(@"UPDATE Users SET Active = 0 WHERE ID = @userId", new { userId });
            userDataService.SetPrimaryEmailAndActivate(userId, primaryEmail);

            // Then
            var result = userDataService.GetUserAccountById(userId);
            result!.PrimaryEmail.Should().Be(primaryEmail);
            result.Active.Should().BeTrue();
        }

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
