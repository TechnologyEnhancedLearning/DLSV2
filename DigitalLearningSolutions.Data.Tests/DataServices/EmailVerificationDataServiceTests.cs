namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public class EmailVerificationDataServiceTests
    {
        private SqlConnection connection = null!;
        private IEmailVerificationDataService emailVerificationDataService = null!;

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            emailVerificationDataService = new EmailVerificationDataService(connection);
        }

        [Test]
        public void CreateEmailVerificationHash_creates_hash()
        {
            using var transaction = new TransactionScope();

            // Given
            const string hash = "hash";
            var currentTime = DateTime.UtcNow;

            // When
            var result = emailVerificationDataService.CreateEmailVerificationHash(hash, currentTime);
            var hashId = connection.Query<int>(
                @"SELECT ID FROM EmailVerificationHashes WHERE EmailVerificationHash = @hash",
                new { hash }
            ).SingleOrDefault();

            // Then
            result.Should().Be(hashId);
        }

        [Test]
        public void UpdateEmailVerificationHashIdForPrimaryEmail_updates_hashId()
        {
            using var transaction = new TransactionScope();

            // Given
            const int userId = 2;
            const string hash = "hash";
            var currentTime = DateTime.UtcNow;
            var hashId = emailVerificationDataService.CreateEmailVerificationHash(hash, currentTime);

            var hashIdBeforeUpdate = connection.QuerySingle<int?>(
                "SELECT EmailVerificationHashID FROM Users WHERE ID = @userId",
                new { userId }
            );

            // When
            emailVerificationDataService.UpdateEmailVerificationHashIdForPrimaryEmail(userId, "test@gmail.com", hashId);
            var result = connection.Query<int>(
                @"SELECT EmailVerificationHashID FROM Users WHERE ID = @userId",
                new { userId }
            ).SingleOrDefault();

            // Then
            hashIdBeforeUpdate.Should().NotBe(hashId);
            result.Should().Be(hashId);
        }

        [Test]
        public void UpdateEmailVerificationHashIdForCentreEmails_updates_hasId()
        {
            using var transaction = new TransactionScope();

            // Given
            const int userId = 2;
            const int centreId = 2;
            const string hash = "hash";
            const string email = "test@gmail.com";
            var currentTime = DateTime.UtcNow;
            var hashId = emailVerificationDataService.CreateEmailVerificationHash(hash, currentTime);
            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email) VALUES (@userId, @centreId, @email)",
                new { userId, centreId, email }
            );

            // When
            emailVerificationDataService.UpdateEmailVerificationHashIdForCentreEmails(userId, email, hashId);
            var result = connection.Query<int>(
                @"SELECT EmailVerificationHashID
                        FROM UserCentreDetails
                        WHERE UserID = @userId AND CentreID = @centreId",
                new { userId, centreId }
            ).SingleOrDefault();

            // Then
            result.Should().Be(hashId);
        }

        [Test]
        [TestCase(false, false, false)]
        [TestCase(false, true, true)]
        [TestCase(true, false, true)]
        [TestCase(true, true, true)]
        public void AccountEmailIsVerifiedForUser_returns_expected_value(
            bool primaryEmailIsVerified,
            bool centreEmailIsVerified,
            bool expectedResult
        )
        {
            using var transaction = new TransactionScope();

            // Given
            const int userId = 2;
            const int centreId = 2;
            const string email = "test@gmail.com";
            var currentTime = DateTime.UtcNow;

            if (primaryEmailIsVerified)
            {
                connection.Execute(
                    @"UPDATE Users SET EmailVerified = @currentTime WHERE ID = @userId",
                    new { currentTime, userId }
                );
            }
            else
            {
                connection.Execute(
                    @"UPDATE Users SET EmailVerified = NULL WHERE ID = @userId",
                    new { userId }
                );
            }

            if (centreEmailIsVerified)
            {
                connection.Execute(
                    @"INSERT INTO UserCentreDetails (UserID, CentreID, Email, EmailVerified)
                        VALUES (@userId, @centreId, @email, @currentTime)",
                    new { userId, centreId, email, currentTime }
                );
            }

            // When
            var result = emailVerificationDataService.AccountEmailIsVerifiedForUser(userId, email);

            // Then
            result.Should().Be(expectedResult);
        }
    }
}
