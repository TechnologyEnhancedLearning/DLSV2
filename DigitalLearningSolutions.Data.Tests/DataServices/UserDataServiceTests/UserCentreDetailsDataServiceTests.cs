namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
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
            userDataService.SetCentreEmail(8, 374, email, null);
            var result =
                connection.QuerySingleOrDefault<string>(@"SELECT Email FROM UserCentreDetails WHERE UserID = 8");
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
            var result =
                connection.QuerySingleOrDefault<DateTime?>(
                    @"SELECT EmailVerified FROM UserCentreDetails WHERE UserID = 8"
                );
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
        public void GetAllActiveCentreEmailsForUser_returns_centre_email_list()
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
            const string candidateNumber = "AAAAA";

            var delegateOnlyCentreName = connection.QuerySingleOrDefault<string>(
                @"SELECT CentreName FROM Centres WHERE CentreID = @delegateOnlyCentreId",
                new { delegateOnlyCentreId }
            );

            var adminOnlyCentreName = connection.QuerySingleOrDefault<string>(
                @"SELECT CentreName FROM Centres WHERE CentreID = @adminOnlyCentreId",
                new { adminOnlyCentreId }
            );

            var adminAndDelegateCentreName = connection.QuerySingleOrDefault<string>(
                @"SELECT CentreName FROM Centres WHERE CentreID = @adminAndDelegateCentreId",
                new { adminAndDelegateCentreId }
            );

            var nullCentreEmailCentreName = connection.QuerySingleOrDefault<string>(
                @"SELECT CentreName FROM Centres WHERE CentreID = @nullCentreEmailCentreId",
                new { nullCentreEmailCentreId }
            );

            connection.Execute(
                @"INSERT INTO AdminAccounts (UserID, CentreID, Active) VALUES
                    (@userId, @adminOnlyCentreId, 1),
                    (@userId, @nullCentreEmailCentreId, 1)",
                new { userId, adminOnlyCentreId, nullCentreEmailCentreId }
            );

            connection.Execute(
                @"INSERT INTO DelegateAccounts (UserId, CentreId, DateRegistered, CandidateNumber, Active) VALUES
                    (@userId, @delegateOnlyCentreId, GETDATE(), @candidateNumber, 1)",
                new { userId, delegateOnlyCentreId, candidateNumber }
            );

            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email) VALUES
                    (@userId, @delegateOnlyCentreId, @delegateOnlyCentreEmail),
                    (@userId, @adminOnlyCentreId, @adminOnlyCentreEmail),
                    (@userId, @adminAndDelegateCentreId, @adminAndDelegateCentreEmail)",
                new
                {
                    userId,
                    delegateOnlyCentreId, delegateOnlyCentreEmail,
                    adminOnlyCentreId, adminOnlyCentreEmail,
                    adminAndDelegateCentreId, adminAndDelegateCentreEmail,
                }
            );

            // When
            var result = userDataService.GetAllActiveCentreEmailsForUser(userId).ToList();

            // Then
            result.Count.Should().Be(4);
            result.Should().ContainEquivalentOf((delegateOnlyCentreId, delegateOnlyCentreName, delegateOnlyCentreEmail));
            result.Should().ContainEquivalentOf((adminOnlyCentreId, adminOnlyCentreName, adminOnlyCentreEmail));
            result.Should().ContainEquivalentOf(
                (adminAndDelegateCentreId, adminAndDelegateCentreName, adminAndDelegateCentreEmail)
            );
            result.Should().ContainEquivalentOf((nullCentreEmailCentreId, nullCentreEmailCentreName, (string?)null));
        }

        [Test]
        public void GetAllActiveCentreEmailsForUser_does_not_return_emails_for_inactive_admin_accounts()
        {
            using var transaction = new TransactionScope();

            // Given
            const int centreId = 3;
            const string email = "inactive_admin@email.com";

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
                ('inactive_admin_primary@email.com', 'password', 'test', 'user', 1, 1, 0, 1, 1)"
            );

            connection.Execute(
                @"INSERT INTO AdminAccounts (UserID, CentreID, Active) VALUES (@userId, @centreId, 0)",
                new { userId, centreId }
            );

            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email) VALUES (@userId, @centreId, @email)",
                new { userId, centreId, email }
            );

            // When
            var result = userDataService.GetAllActiveCentreEmailsForUser(userId).ToList();

            // Then
            result.Count.Should().Be(0);
        }

        [Test]
        public void GetAllActiveCentreEmailsForUser_does_not_return_emails_for_inactive_delegate_accounts()
        {
            using var transaction = new TransactionScope();

            // Given
            const int centreId = 3;
            const string email = "inactive_delegate@email.com";

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
                ('inactive_delegate_primary@email.com', 'password', 'test', 'user', 1, 1, 0, 1, 1)"
            );

            connection.Execute(
                @"INSERT INTO DelegateAccounts (UserID, CentreID, Active, DateRegistered, CandidateNumber) VALUES (@userId, @centreId, 0, GETDATE(), 'TU255')",
                new { userId, centreId }
            );

            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email) VALUES (@userId, @centreId, @email)",
                new { userId, centreId, email }
            );

            // When
            var result = userDataService.GetAllActiveCentreEmailsForUser(userId).ToList();

            // Then
            result.Count.Should().Be(0);
        }

        [Test]
        public void GetAllActiveCentreEmailsForUser_returns_empty_list_when_user_has_no_centre_accounts()
        {
            using var transaction = new TransactionScope();

            // Given
            const int userId = 777;
            const int adminId = 805;

            connection.Execute(@"DELETE FROM DelegateAccounts WHERE UserID = @userId", new { userId });
            
            connection.Execute(@"DELETE FROM TicketComments WHERE TicketId IN (SELECT TicketId FROM Tickets WHERE AdminUserId = @adminId)", new { adminId });

            connection.Execute(@"DELETE FROM Tickets WHERE AdminUserID = @adminId", new { adminId });

            connection.Execute(@"DELETE FROM AdminSessions WHERE AdminId = @adminId", new { adminId });

            connection.Execute(@"DELETE FROM AdminAccounts WHERE UserID = @userId", new { userId });

            // When
            var result = userDataService.GetAllActiveCentreEmailsForUser(userId);

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
            var centreName = connection.QuerySingleOrDefault<string>(
                @"SELECT CentreName FROM Centres WHERE CentreID = @centreId",
                new { centreId }
            );

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
            var emailVerificationHashId = connection.QuerySingle<int>(
                @"INSERT INTO [dbo].[EmailVerificationHashes]
                        ([EmailVerificationHash]
                        ,[CreatedDate])
                    OUTPUT Inserted.ID
                    VALUES (N'hash', GETDATE())");

            var adminId = connection.QuerySingle<int>(
                @"INSERT INTO [dbo].[UserCentreDetails]
                           ([UserID]
                           ,[CentreID]
                           ,[Email]
                           ,[EmailVerified]
                           ,[EmailVerificationHashID])
                    OUTPUT Inserted.ID
                    VALUES
                           (1, 101, N'test@example.com', CURRENT_TIMESTAMP, @emailVerificationHashId)", new {emailVerificationHashId});

            const int userIdForUserCentreDetailsAfterUpdate = 2;
            
            var delegateEntity = userDataService.GetDelegateByCandidateNumber("KW969")!;
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

        [Test]
        public void GetCentreEmailVerificationDetails_returns_expected_value()
        {
            using var transaction = new TransactionScope();

            // Given
            const int userId = 1;
            const int centreId = 101;
            const string email = "unverified@email.com";
            const string code = "code";
            const bool delegateIsApproved = true;
            var createdDate = new DateTime(2022, 1, 1);

            GivenEmailVerificationHashLinkedToUserCentreDetails(
                userId,
                centreId,
                email,
                code,
                createdDate,
                delegateIsApproved
            );

            // When
            var result = userDataService.GetCentreEmailVerificationDetails(code);

            // Then
            result.Single().CentreIdIfEmailIsForUnapprovedDelegate.Should().Be(null);
        }

        [Test]
        public void GetCentreEmailVerificationDetails_returns_expected_value_for_centre_id_if_delegate_is_unapproved()
        {
            using var transaction = new TransactionScope();

            // Given
            const int userId = 1;
            const int centreId = 101;
            const string email = "unverified@email.com";
            const string code = "code";
            var createdDate = new DateTime(2022, 1, 1);
            const bool delegateIsApproved = false;

            GivenEmailVerificationHashLinkedToUserCentreDetails(
                userId,
                centreId,
                email,
                code,
                createdDate,
                delegateIsApproved
            );

            // When
            var result = userDataService.GetCentreEmailVerificationDetails(code);

            // Then
            result.Single().CentreIdIfEmailIsForUnapprovedDelegate.Should().Be(centreId);
        }

        [Test]
        public void SetCentreEmailVerified_sets_EmailVerified_and_EmailVerificationHashId()
        {
            using var transaction = new TransactionScope();

            // Given
            const int userId = 1;
            const int centreId = 2;
            const string email = "unverified@email.com";
            const string code = "code";
            var createdDate = new DateTime(2022, 1, 1);
            var verifiedDate = new DateTime(2022, 1, 3);
            const bool delegateIsApproved = true;

            GivenEmailVerificationHashLinkedToUserCentreDetails(
                userId,
                centreId,
                email,
                code,
                createdDate,
                delegateIsApproved
            );

            // When
            userDataService.SetCentreEmailVerified(userId, email, verifiedDate);

            // Then
            var (emailVerifiedAfterUpdate, emailVerificationHashIdAfterUpdate) =
                connection.QuerySingle<(DateTime?, int?)>(
                    @"SELECT EmailVerified, EmailVerificationHashID FROM UserCentreDetails WHERE UserID = @userId AND Email = @email",
                    new { userId, email }
                );

            emailVerifiedAfterUpdate.Should().BeSameDateAs(verifiedDate);
            emailVerificationHashIdAfterUpdate.Should().BeNull();
        }

        [Test]
        public void
            SetCentreEmailVerified_does_not_set_EmailVerified_and_EmailVerificationHashId_if_userId_and_email_do_not_match()
        {
            using var transaction = new TransactionScope();

            // Given
            const int userId = 1;
            const int centreId = 2;
            const string email = "SetCentreEmailVerified@email.com";
            var oldVerifiedDate = new DateTime(2022, 1, 1);
            var newVerifiedDate = new DateTime(2022, 1, 3);

            var oldEmailVerificationHashId = connection.QuerySingle<int>(
                @"INSERT INTO EmailVerificationHashes (EmailVerificationHash, CreatedDate) OUTPUT Inserted.ID VALUES ('code', CURRENT_TIMESTAMP);"
            );

            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email, EmailVerified, EmailVerificationHashID)
                    VALUES (@userId, @centreId, @email, @oldVerifiedDate, @oldEmailVerificationHashId)",
                new { userId, centreId, email, oldVerifiedDate, oldEmailVerificationHashId }
            );

            // When
            userDataService.SetCentreEmailVerified(userId, "different@email.com", newVerifiedDate);

            // Then
            var (emailVerified, emailVerificationHashId) =
                connection.QuerySingle<(DateTime?, int?)>(
                    @"SELECT EmailVerified, EmailVerificationHashID FROM UserCentreDetails WHERE UserID = @userId AND Email = @email",
                    new { userId, email }
                );

            emailVerified.Should().BeSameDateAs(oldVerifiedDate);
            emailVerificationHashId.Should().Be(oldEmailVerificationHashId);
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

        private void GivenEmailVerificationHashLinkedToUserCentreDetails(
            int userId,
            int centreId,
            string email,
            string hash,
            DateTime createdDate,
            bool delegateIsApproved
        )
        {
            var emailVerificationHashesId = connection.QuerySingle<int>(
                @"INSERT INTO EmailVerificationHashes (EmailVerificationHash, CreatedDate) OUTPUT Inserted.ID VALUES (@hash, @createdDate);",
                new { hash, createdDate }
            );

            connection.Execute(
                @"UPDATE DelegateAccounts SET Approved = @delegateIsApproved Where UserID = @userId AND CentreID = @centreId;",
                new { delegateIsApproved, userId, centreId }
            );

            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email, EmailVerificationHashID)
                    VALUES (@userId, @centreId, @email, @emailVerificationHashesId)",
                new { userId, centreId, email, emailVerificationHashesId }
            );
        }
    }
}
