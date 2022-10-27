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
        [TestCase("email@test.com", 40005)]
        [TestCase("ES2", 106599)]
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
            result.Should().Be(36702);
        }

        [Test]
        public void GetUserAccountById_returns_expected_user_account()
        {
            // Given
            var defaultUser = UserTestHelper.GetDefaultUserAccount();

            // When
            var result = userDataService.GetUserAccountById(2);
            result.EmailVerified = defaultUser.EmailVerified;
            result.DetailsLastChecked = defaultUser.DetailsLastChecked;

            // Then
            result.Should().BeEquivalentTo(
                UserTestHelper.GetDefaultUserAccount()
            );
        }

        [Test]
        public void GetUserAccountByPrimaryEmail_returns_expected_user_account()
        {
            // Given
            var defaultUser = UserTestHelper.GetDefaultUserAccount();

            // When
            var result = userDataService.GetUserAccountByPrimaryEmail("test@gmail.com");
            result.EmailVerified = defaultUser.EmailVerified;
            result.DetailsLastChecked = defaultUser.DetailsLastChecked;

            // Then
            result.Should().BeEquivalentTo(
                defaultUser
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

        [Test]
        public void SetPrimaryEmailAndActivate_sets_primary_email_and_activates_user()
        {
            using var transaction = new TransactionScope();

            // Given
            const int userId = 2;
            const string primaryEmail = "primary@email.com";
            connection.Execute(@"UPDATE Users SET Active = 0 WHERE ID = @userId", new { userId });

            // When
            userDataService.SetPrimaryEmailAndActivate(userId, primaryEmail);

            // Then
            var result = userDataService.GetUserAccountById(userId);
            result!.PrimaryEmail.Should().Be(primaryEmail);
            result.Active.Should().BeTrue();
        }

        [Test]
        public void DeleteUser_deletes_expected_user()
        {
            using var transaction = new TransactionScope();

            // Given

            // Create a new user with no delegate or admin accounts so that it can be deleted without failing
            // a referential integrity check in the database.
            var userId = connection.QuerySingle<int>(
                @"INSERT INTO Users
                (
                    PrimaryEmail,
                    PasswordHash,
                    FirstName,
                    LastName,
                    JobGroupID,
                    Active,
                    FailedLoginCount,
                    HasBeenPromptedForPrn,
                    HasDismissedLhLoginWarning
                )
                OUTPUT Inserted.ID
                VALUES
                ('DeleteUser_deletes_expected_user@email.com', 'password', 'test', 'user', 1, 1, 0, 1, 1)"
            );

            var user = userDataService.GetUserAccountById(userId);

            // When
            userDataService.DeleteUser(userId);

            // Then
            var result = userDataService.GetUserAccountById(userId);

            result.Should().BeNull();
            user.Should().NotBeNull();
        }

        [Test]
        public void GetPrimaryEmailVerificationDetails_returns_expected_value()
        {
            using var transaction = new TransactionScope();

            // Given
            const string email = "unverified@email.com";
            const string code = "code";
            var createdDate = new DateTime(2022, 1, 1);

            var userId = GivenEmailVerificationHashLinkedToUser(email, code, createdDate);

            // When
            var result = userDataService.GetPrimaryEmailVerificationDetails(code);

            // Then
            result!.UserId.Should().Be(userId);
            result.Email.Should().Be(email);
            result.EmailVerificationHash.Should().Be(code);
            result.EmailVerified.Should().BeNull();
            result.EmailVerificationHashCreatedDate.Should().Be(createdDate);
            result.CentreIdIfEmailIsForUnapprovedDelegate.Should().Be(null);
        }

        [Test]
        public void SetPrimaryEmailVerified_sets_EmailVerified_and_EmailVerificationHashId()
        {
            using var transaction = new TransactionScope();

            // Given
            const string email = "unverified@email.com";
            const string code = "code";
            var createdDate = new DateTime(2022, 1, 1);
            var verifiedDate = new DateTime(2022, 1, 3);

            var userId = GivenEmailVerificationHashLinkedToUser(email, code, createdDate);

            // When
            userDataService.SetPrimaryEmailVerified(userId, email, verifiedDate);

            // Then
            var (emailVerified, emailVerificationHashId) = connection.QuerySingle<(DateTime?, int?)>(
                @"SELECT EmailVerified, EmailVerificationHashID FROM Users WHERE ID = @userId",
                new { userId }
            );

            emailVerified.Should().BeSameDateAs(verifiedDate);
            emailVerificationHashId.Should().BeNull();
        }

        [Test]
        public void
            SetPrimaryEmailVerified_does_not_set_EmailVerified_or_EmailVerificationHashId_if_userId_and_email_do_not_match()
        {
            using var transaction = new TransactionScope();

            // Given
            const int userId = 1;
            const string email = "SetPrimaryEmailVerified@email.com";
            var oldVerifiedDate = new DateTime(2022, 1, 1);
            var newVerifiedDate = new DateTime(2022, 1, 3);

            var oldEmailVerificationHashId = connection.QuerySingle<int>(
                @"INSERT INTO EmailVerificationHashes (EmailVerificationHash, CreatedDate) OUTPUT Inserted.ID VALUES ('code', CURRENT_TIMESTAMP);"
            );

            connection.Execute(
                @"UPDATE Users SET PrimaryEmail = @email, EmailVerified = @oldVerifiedDate, EmailVerificationHashID = @oldEmailVerificationHashId WHERE ID = @userId",
                new { userId, email, oldVerifiedDate, oldEmailVerificationHashId }
            );

            // When
            userDataService.SetPrimaryEmailVerified(userId, "different@email.com", newVerifiedDate);

            // Then
            var (emailVerified, emailVerificationHashId) = connection.QuerySingle<(DateTime?, int?)>(
                @"SELECT EmailVerified, EmailVerificationHashID FROM Users WHERE ID = @userId",
                new { userId }
            );

            emailVerified.Should().BeSameDateAs(oldVerifiedDate);
            emailVerificationHashId.Should().Be(oldEmailVerificationHashId);
        }

        private int GivenEmailVerificationHashLinkedToUser(
            string email,
            string hash,
            DateTime createdDate
        )
        {
            var emailVerificationHashesId = connection.QuerySingle<int>(
                @"INSERT INTO EmailVerificationHashes (EmailVerificationHash, CreatedDate) OUTPUT Inserted.ID VALUES (@hash, @createdDate);",
                new { hash, createdDate }
            );

            return connection.QuerySingle<int>(
                @"INSERT INTO Users (
                        FirstName,
                        LastName,
                        PrimaryEmail,
                        PasswordHash,
                        Active,
                        JobGroupID,
                        EmailVerificationHashID
                    )
                    OUTPUT Inserted.ID
                    VALUES ('Unverified', 'Email', @email, 'password', 1, 1, @emailVerificationHashesId)",
                new { email, emailVerificationHashesId }
            );
        }
    }
}
