﻿namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
{
    using System;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using FluentAssertions;
    using NUnit.Framework;

    public partial class UserDataServiceTests
    {
        [Test]
        [TestCase(true, null, 1)]
        [TestCase(true, "new@admin.email", 1)]
        [TestCase(false, null, 0)]
        [TestCase(false, "new@admin.email", 1)]
        public void SetCentreEmail_sets_email_if_not_empty(bool detailsExist, string? email, int entriesCount)
        {
            using var transaction = new TransactionScope();

            // Given
            if (detailsExist)
            {
                connection.Execute(
                    @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)
                        VALUES (8, 374, 'sample@admin.email')"
                );
            }

            // When
            userDataService.SetCentreEmail(8, 374, email);
            var result = connection.Query<string?>(@"SELECT Email FROM UserCentreDetails WHERE UserID = 8")
                .SingleOrDefault();
            var count = connection.Query<int>(@"SELECT COUNT(*) FROM UserCentreDetails WHERE UserID = 8");

            // Then
            result.Should().BeEquivalentTo(email);
            count.Should().Equal(entriesCount);
        }

        [Test]
        [TestCase(true, null, 1)]
        [TestCase(true, "new@admin.email", 1)]
        [TestCase(false, null, 0)]
        [TestCase(false, "new@admin.email", 1)]
        public void SetCentreEmail_sets_emailVerified_if_provided_and_email_not_empty(
            bool detailsExist,
            string? email,
            int entriesCount
        )
        {
            using var transaction = new TransactionScope();

            // Given
            var emailVerified = new DateTime(2022, 2, 2);
            if (detailsExist)
            {
                connection.Execute(
                    @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)
                        VALUES (8, 374, 'sample@admin.email')"
                );
            }

            // When
            userDataService.SetCentreEmail(8, 374, email, emailVerified);
            var result = connection.Query<DateTime?>(@"SELECT EmailVerified FROM UserCentreDetails WHERE UserID = 8")
                .SingleOrDefault();
            var count = connection.Query<int>(@"SELECT COUNT(*) FROM UserCentreDetails WHERE UserID = 8");

            // Then
            result.Should().Be(email == null ? (DateTime?)null : emailVerified);
            count.Should().Equal(entriesCount);
        }

        [Test]
        public void GetUnverifiedCentreEmailsForUser_returns_unverified_emails_for_active_centres()
        {
            using var transaction = new TransactionScope();

            // Given
            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email, EmailVerified)
                        VALUES (8, 374, 'verified@centre.email', '2022-06-17 17:06:22')"
            );
            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)
                        VALUES (8, 375, 'unverified@centre.email')"
            );
            connection.Execute(
                @"INSERT INTO UserCentreDetails(UserID, CentreID, Email)
                        VALUES (8, 378, 'unverified@inactive_centre.email')"
            );

            // When
            var result = userDataService.GetUnverifiedCentreEmailsForUser(8).ToList();

            // Then
            result.Should().HaveCount(1);
            result[0].centreEmail.Should().BeEquivalentTo("unverified@centre.email");
        }

        [Test]
        [TestCase("centre@email.com", true)]
        [TestCase("not_an_email_in_the_database", false)]
        public void CentreSpecificEmailIsInUseAtCentre_returns_expected_value(string email, bool expectedResult)
        {
            using var transaction = new TransactionScope();

            // Given
            const int centreId = 2;

            if (expectedResult)
            {
                connection.Execute(
                    @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)
                    VALUES (1, @centreId, @email)",
                    new { centreId, email }
                );
            }

            // When
            var result = userDataService.CentreSpecificEmailIsInUseAtCentre(email, centreId);

            // Then
            result.Should().Be(expectedResult);
        }

        [Test]
        public void CentreSpecificEmailIsInUseAtCentre_returns_false_when_email_is_in_use_at_different_centre()
        {
            using var transaction = new TransactionScope();

            // Given
            const string email = "centre@email.com";

            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)
                VALUES (1, 2, @email)",
                new { email }
            );

            // When
            var result = userDataService.CentreSpecificEmailIsInUseAtCentre(email, 3);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        [TestCase("centre@email.com", true)]
        [TestCase("not_an_email_in_the_database", false)]
        public void CentreSpecificEmailIsInUseAtCentreByOtherUser_returns_expected_value(
            string email,
            bool expectedResult
        )
        {
            using var transaction = new TransactionScope();

            // Given
            const int userId = 1;
            const int centreId = 2;

            if (expectedResult)
            {
                connection.Execute(
                    @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)
                    VALUES (@userId, @centreId, @email)",
                    new { userId, centreId, email }
                );
            }

            // When
            var result = userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(email, centreId, 2);

            // Then
            result.Should().Be(expectedResult);
        }

        [Test]
        public void
            CentreSpecificEmailIsInUseAtCentreByOtherUser_returns_false_when_email_is_in_use_at_different_centre()
        {
            using var transaction = new TransactionScope();

            // Given
            const string email = "centre@email.com";

            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)
                VALUES (1, 2, @email)",
                new { email }
            );

            // When
            var result = userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(email, 3, 2);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void CentreSpecificEmailIsInUseAtCentreByOtherUser_returns_false_when_email_is_in_use_by_same_user()
        {
            using var transaction = new TransactionScope();

            // Given
            const string email = "centre@email.com";
            const int userId = 1;
            const int centreId = 2;

            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)
                VALUES (@userId, @centreId, @email)",
                new { userId, centreId, email }
            );

            // When
            var result = userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(email, centreId, userId);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void GetAllCentreEmailsForUser_returns_centre_email_list()
        {
            using var transaction = new TransactionScope();

            // Given
            const int userId = 1;
            const int delegateOnlyCentreId = 2;
            const int adminOnlyCentreId = 3;
            const int adminAndDelegateCentreId = 101;
            const int nullCentreEmailCentreId = 4;
            const string delegateOnlyCentreEmail = "centre2@email.com";
            const string adminOnlyCentreEmail = "centre3@email.com";
            const string adminAndDelegateCentreEmail = "centre101@email.com";
            var delegateOnlyCentreName = connection.Query<string>(
                @"SELECT CentreName FROM Centres WHERE CentreID = @delegateOnlyCentreId",
                new { delegateOnlyCentreId }
            ).SingleOrDefault();
            var adminOnlyCentreName = connection.Query<string>(
                @"SELECT CentreName FROM Centres WHERE CentreID = @adminOnlyCentreId",
                new { adminOnlyCentreId }
            ).SingleOrDefault();
            var adminAndDelegateCentreName = connection.Query<string>(
                @"SELECT CentreName FROM Centres WHERE CentreID = @adminAndDelegateCentreId",
                new { adminAndDelegateCentreId }
            ).SingleOrDefault();
            var nullCentreEmailCentreName = connection.Query<string>(
                @"SELECT CentreName FROM Centres WHERE CentreID = @nullCentreEmailCentreId",
                new { nullCentreEmailCentreId }
            ).SingleOrDefault();

            connection.Execute(
                @"INSERT INTO AdminAccounts (UserID, CentreID) VALUES (@userId, @adminOnlyCentreId)",
                new { userId, adminOnlyCentreId }
            );
            connection.Execute(
                @"INSERT INTO AdminAccounts (UserID, CentreID) VALUES (@userId, @nullCentreEmailCentreId)",
                new { userId, nullCentreEmailCentreId }
            );
            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)VALUES (@userId, @delegateOnlyCentreId, @delegateOnlyCentreEmail)",
                new { userId, delegateOnlyCentreId, delegateOnlyCentreEmail }
            );

            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)
                VALUES (@userId, @adminOnlyCentreId, @adminOnlyCentreEmail)",
                new { userId, adminOnlyCentreId, adminOnlyCentreEmail }
            );

            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)
                VALUES (@userId, @adminAndDelegateCentreId, @adminAndDelegateCentreEmail)",
                new { userId, adminAndDelegateCentreId, adminAndDelegateCentreEmail }
            );

            // When
            var result = userDataService.GetAllCentreEmailsForUser(1).ToList();

            // Then
            result.Count.Should().Be(4);
            result.Should()
                .ContainEquivalentOf((delegateOnlyCentreId, delegateOnlyCentreName, delegateOnlyCentreEmail));
            result.Should().ContainEquivalentOf((adminOnlyCentreId, adminOnlyCentreName, adminOnlyCentreEmail));
            result.Should().ContainEquivalentOf(
                (adminAndDelegateCentreId, adminAndDelegateCentreName, adminAndDelegateCentreEmail)
            );
            result.Should().ContainEquivalentOf((nullCentreEmailCentreId, nullCentreEmailCentreName, (string?)null));
        }

        [Test]
        public void GetAllCentreEmailsForUser_returns_empty_list_when_user_has_no_centre_accounts()
        {
            using var transaction = new TransactionScope();

            // Given
            const int userId = 777;
            connection.Execute(@"DELETE FROM DelegateAccounts WHERE UserID = @userId", new { userId });
            connection.Execute(@"DELETE FROM AdminAccounts WHERE UserID = @userId", new { userId });

            // When
            var result = userDataService.GetAllCentreEmailsForUser(userId);

            // Then
            result.Should().BeEmpty();
        }

        [Test]
        public void
            GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair_returns_null_if_user_does_not_exist()
        {
            using var transaction = new TransactionScope();

            // When
            var result =
                userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    "centre@email.com",
                    "hash"
                );

            // Then
            result.Should().Be((null, null, null));
        }

        [Test]
        public void GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair_returns_user_id_if_it_exists()
        {
            using var transaction = new TransactionScope();

            // Given
            const string email = "centre@email.com";
            const string confirmationHash = "hash";
            const int centreId = 3;
            const int userId = 1;
            var centreName = connection.Query<string>(
                @"SELECT CentreName FROM Centres WHERE CentreID = @centreId",
                new { centreId }
            ).SingleOrDefault();

            GivenUnclaimedUserExists(userId, centreId, email, confirmationHash);

            // When
            var result =
                userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    email,
                    confirmationHash
                );

            // Then
            result.Should().Be((userId, centreId, centreName));
        }

        [Test]
        public void LinkUserCentreDetailsToNewUser_updates_UserId_in_claimed_UserCentreDetails()
        {
            using var transaction = new TransactionScope();

            // Given
            const int userIdForUserCentreDetailsAfterUpdate = 2;

            var delegateEntity = userDataService.GetDelegateByCandidateNumber("CLAIMABLEUSER1")!;
            var currentUserIdForUserCentreDetails = delegateEntity.UserAccount.Id;
            var centreId = delegateEntity.DelegateAccount.CentreId;
            var userCentreDetailsId = delegateEntity.UserCentreDetails!.Id;
            var email = delegateEntity.UserCentreDetails.Email;

            var newUser = userDataService.GetUserAccountById(userIdForUserCentreDetailsAfterUpdate);

            var newUserUserCentreDetailsBeforeUpdate = connection.Query<(int, string)>(
                @"SELECT CentreID, Email FROM UserCentreDetails
                    WHERE UserID = @userIdForUserCentreDetailsAfterUpdate",
                new { userIdForUserCentreDetailsAfterUpdate }
            );

            // When
            userDataService.LinkUserCentreDetailsToNewUser(
                currentUserIdForUserCentreDetails,
                userIdForUserCentreDetailsAfterUpdate,
                centreId
            );

            // Then
            newUser.Should().NotBeNull();

            newUserUserCentreDetailsBeforeUpdate.Should()
                .NotContain(row => row.Item1 == centreId && row.Item2 == email);

            var updatedUserCentreDetails = connection.QuerySingle<(int, int, string)>(
                @"SELECT UserID, CentreID, Email FROM UserCentreDetails
                        WHERE ID = @userCentreDetailsId",
                new { userCentreDetailsId }
            );

            updatedUserCentreDetails.Item1.Should().Be(userIdForUserCentreDetailsAfterUpdate);
            updatedUserCentreDetails.Item2.Should().Be(centreId);
            updatedUserCentreDetails.Item3.Should().Be(email);
        }

        private void GivenUnclaimedUserExists(int userId, int centreId, string email, string hash)
        {
            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email) VALUES (@userId, @centreId, @email)",
                new { userId, centreId, email }
            );

            connection.Execute(
                @"INSERT INTO DelegateAccounts
                            (UserID, CentreID, RegistrationConfirmationHash, DateRegistered, CandidateNumber)
                        VALUES (@userId, @centreId, @hash, GETDATE(), 'CN1001')",
                new { userId, centreId, hash }
            );
        }
    }
}
