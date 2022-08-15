namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
{
    using System;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public partial class UserDataServiceTests
    {
        [Test]
        public void GetDelegateById_returns_delegate_with_null_user_centre_details()
        {
            // Given
            var expectedDelegateEntity = UserTestHelper.GetDefaultDelegateEntity();

            // When
            var returnedDelegateEntity = userDataService.GetDelegateById(2);

            // Then
            returnedDelegateEntity.Should().BeEquivalentTo(expectedDelegateEntity);
        }

        [Test]
        public void GetDelegateById_returns_delegate_with_correct_user_centre_details()
        {
            using var transaction = new TransactionScope();

            // Given
            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)
                    VALUES (61188, 2, 'centre@email.com')"
            );
            // We set userCentreDetailsId here so that UserTestHelper.GetDefaultDelegateEntity returns an
            // DelegateEntity with a non-null UserCentreDetails
            var expectedUserCentreDetails = UserTestHelper.GetDefaultDelegateEntity(
                userCentreDetailsId: 1,
                centreSpecificEmail: "centre@email.com",
                centreSpecificEmailVerified: null
            ).UserCentreDetails;

            // When
            var returnedDelegateEntity = userDataService.GetDelegateById(2);

            // Then
            returnedDelegateEntity!.UserCentreDetails.Should().NotBeNull();
            returnedDelegateEntity.UserCentreDetails!.UserId.Should().Be(expectedUserCentreDetails!.UserId);
            returnedDelegateEntity.UserCentreDetails.CentreId.Should().Be(expectedUserCentreDetails.CentreId);
            returnedDelegateEntity.UserCentreDetails.Email.Should().Be(expectedUserCentreDetails.Email);
            returnedDelegateEntity.UserCentreDetails.EmailVerified.Should().Be(expectedUserCentreDetails.EmailVerified);
        }

        [Test]
        public void GetDelegateById_returns_delegate_with_correct_adminId()
        {
            using var transaction = new TransactionScope();

            // Given
            var adminId = connection.QuerySingle<int>(
                @"INSERT INTO AdminAccounts (
                           UserID,
                           CentreID,
                           Active)
                    OUTPUT Inserted.ID
                    VALUES (61188, 2, 1)"
            );

            // When
            var returnedDelegateEntity = userDataService.GetDelegateById(2);

            // Then
            returnedDelegateEntity!.AdminId.Should().Be(adminId);
        }

        [Test]
        public void GetDelegateById_does_not_return_id_for_inactive_admin()
        {
            using var transaction = new TransactionScope();

            // Given
            var adminId = connection.QuerySingle<int>(
                @"INSERT INTO AdminAccounts (
                           UserID,
                           CentreID,
                           Active)
                    OUTPUT Inserted.ID
                    VALUES (61188, 2, 0)"
            );

            // When
            var returnedDelegateEntity = userDataService.GetDelegateById(2);

            // Then
            returnedDelegateEntity!.AdminId.Should().BeNull();
        }

        [Test]
        public void GetDelegateByCandidateNumber_returns_delegate_with_null_user_centre_details()
        {
            // Given
            var expectedDelegateEntity = UserTestHelper.GetDefaultDelegateEntity();

            // When
            var returnedDelegateEntity = userDataService.GetDelegateByCandidateNumber("SV1234");

            // Then
            returnedDelegateEntity.Should().BeEquivalentTo(expectedDelegateEntity);
        }

        [Test]
        public void GetDelegateByCandidateNumber_returns_delegate_with_correct_user_centre_details()
        {
            using var transaction = new TransactionScope();

            // Given
            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)
                    VALUES (61188, 2, 'centre@email.com')"
            );
            var expectedUserCentreDetails = UserTestHelper.GetDefaultDelegateEntity(
                userCentreDetailsId: 1,
                centreSpecificEmail: "centre@email.com",
                centreSpecificEmailVerified: null
            ).UserCentreDetails;

            // When
            var returnedDelegateEntity = userDataService.GetDelegateByCandidateNumber("SV1234");

            // Then
            returnedDelegateEntity!.UserCentreDetails.Should().NotBeNull();
            returnedDelegateEntity.UserCentreDetails!.UserId.Should().Be(expectedUserCentreDetails!.UserId);
            returnedDelegateEntity.UserCentreDetails.CentreId.Should().Be(expectedUserCentreDetails.CentreId);
            returnedDelegateEntity.UserCentreDetails.Email.Should().Be(expectedUserCentreDetails.Email);
            returnedDelegateEntity.UserCentreDetails.EmailVerified.Should().Be(expectedUserCentreDetails.EmailVerified);
        }

        [Test]
        public void GetUnapprovedDelegatesByCentreId_returns_correct_delegate_users()
        {
            // When
            var returnedDelegateEntities = userDataService.GetUnapprovedDelegatesByCentreId(101).ToList();

            // Then
            returnedDelegateEntities.Count.Should().Be(4);
            returnedDelegateEntities.Select(d => d.DelegateAccount.Id).Should()
                .BeEquivalentTo(new[] { 28, 16, 115768, 297514 });
        }

        [Test]
        public void GetDelegateUserById_Returns_delegate_users()
        {
            // Given
            var expectedDelegateUsers = UserTestHelper.GetDefaultDelegateUser();

            // When
            var returnedDelegateUser = userDataService.GetDelegateUserById(2);

            // Then
            returnedDelegateUser.Should().BeEquivalentTo(expectedDelegateUsers);
        }

        [Test]
        public void GetUnapprovedDelegateUsersByCentreId_returns_correct_delegate_users()
        {
            // When
            var returnedDelegateUsers = userDataService.GetUnapprovedDelegateUsersByCentreId(101);

            // Then
            returnedDelegateUsers.Count.Should().Be(4);
            returnedDelegateUsers.Select(d => d.Id).Should().BeEquivalentTo(new[] { 28, 16, 115768, 297514 });
        }

        [Test]
        public void UpdateUser_updates_user()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const string firstName = "TestFirstName";
                const string lastName = "TestLastName";
                const string email = "test@email.com";
                const string professionalRegNumber = "test-1234";
                const int jobGroupId = 1;

                // When
                userDataService.UpdateUser(
                    firstName,
                    lastName,
                    email,
                    null,
                    professionalRegNumber,
                    true,
                    jobGroupId,
                    DateTime.Now,
                    null,
                    61188,
                    true
                );
                var updatedUser = userDataService.GetDelegateUserById(2)!;

                // Then
                using (new AssertionScope())
                {
                    updatedUser.FirstName.Should().BeEquivalentTo(firstName);
                    updatedUser.LastName.Should().BeEquivalentTo(lastName);
                    updatedUser.EmailAddress.Should().BeEquivalentTo(email);
                    updatedUser.ProfessionalRegistrationNumber.Should().BeEquivalentTo(professionalRegNumber);
                    updatedUser.HasBeenPromptedForPrn.Should().BeTrue();
                    updatedUser.JobGroupId.Should().Be(jobGroupId);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void ApproveDelegateUsers_approves_delegate_users()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                var ids = new[] { 16, 28 };
                var delegate1 = userDataService.GetDelegateUserById(16)!;
                var delegate2 = userDataService.GetDelegateUserById(28)!;

                delegate1.Approved.Should().BeFalse();
                delegate2.Approved.Should().BeFalse();

                // When
                userDataService.ApproveDelegateUsers(ids);

                var delegate1AfterUpdate = userDataService.GetDelegateUserById(16)!;
                var delegate2AfterUpdate = userDataService.GetDelegateUserById(28)!;

                // Then
                delegate1AfterUpdate.Approved.Should().BeTrue();
                delegate2AfterUpdate.Approved.Should().BeTrue();
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void RemoveDelegateAccount_deletes_delegate_account()
        {
            using var transaction = new TransactionScope();

            // Given
            var id = 610;
            userDataService.GetDelegateUserById(id).Should().NotBeNull();

            // When
            userDataService.RemoveDelegateAccount(id);

            // Then
            userDataService.GetDelegateUserById(id).Should().BeNull();
        }

        [Test]
        public void RemoveDelegateAccount_cannot_remove_delegate_account_with_started_session()
        {
            using var transaction = new TransactionScope();

            // Given
            var id = 16;
            userDataService.GetDelegateUserById(id).Should().NotBeNull();

            // When
            Action action = () => userDataService.RemoveDelegateAccount(id);

            // Then
            action.Should().Throw<SqlException>();
        }

        [Test]
        public void GetNumberOfActiveApprovedDelegatesAtCentre_returns_expected_count()
        {
            // When
            var count = userDataService.GetNumberOfApprovedDelegatesAtCentre(2);

            // Then
            count.Should().Be(3420);
        }

        [Test]
        public void DeactivateDelegateUser_deactivates_delegate_user()
        {
            using var transaction = new TransactionScope();

            // Given
            userDataService.GetDelegateUserById(1)!.Active.Should().BeTrue();

            // When
            userDataService.DeactivateDelegateUser(1);

            // Then
            userDataService.GetDelegateUserById(1)!.Active.Should().BeFalse();
        }

        [Test]
        public void UpdateDelegateAccount_updates_delegate()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const int delegateId = 11;
                const bool active = true;
                const string answer1 = "answer1";
                const string answer2 = "answer2";
                const string answer3 = "answer3";
                const string answer4 = "answer4";
                const string answer5 = "answer5";
                const string answer6 = "answer6";

                // When
                userDataService.UpdateDelegateAccount(
                    delegateId,
                    active,
                    answer1,
                    answer2,
                    answer3,
                    answer4,
                    answer5,
                    answer6
                );
                var delegateUser = userDataService.GetDelegateUserById(delegateId)!;

                // Then
                using (new AssertionScope())
                {
                    delegateUser.Active.Should().Be(active);
                    delegateUser.Answer1.Should().Be(answer1);
                    delegateUser.Answer2.Should().Be(answer2);
                    delegateUser.Answer3.Should().Be(answer3);
                    delegateUser.Answer4.Should().Be(answer4);
                    delegateUser.Answer5.Should().Be(answer5);
                    delegateUser.Answer6.Should().Be(answer6);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void UpdateUserDetails_updates_user()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const string firstName = "TestFirstName";
                const string lastName = "TestLastName";
                const string email = "test@email.com";
                const int jobGroupId = 1;

                // When
                userDataService.UpdateUserDetails(firstName, lastName, email, jobGroupId, 61188);
                var updatedUser = userDataService.GetDelegateUserById(2)!;

                // Then
                using (new AssertionScope())
                {
                    updatedUser.FirstName.Should().BeEquivalentTo(firstName);
                    updatedUser.LastName.Should().BeEquivalentTo(lastName);
                    updatedUser.EmailAddress.Should().BeEquivalentTo(email);
                    updatedUser.JobGroupId.Should().Be(jobGroupId);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void ActivateDelegateUser_activates_delegate_user()
        {
            using var transaction = new TransactionScope();

            // Given
            const int delegateUserId = 29;

            // When
            userDataService.ActivateDelegateUser(delegateUserId);

            // Then
            userDataService.GetDelegateUserById(delegateUserId)!.Active.Should().BeTrue();
        }

        [Test]
        public void GetDelegatesNotRegisteredForGroupByGroupId_returns_expected_number_of_delegates()
        {
            // When
            var result = userDataService.GetDelegatesNotRegisteredForGroupByGroupId(5, 101);

            // Then
            result.Should().HaveCount(106);
        }

        [Test]
        public void SetDelegateUserLearningHubAuthId_correctly_sets_delegates_learningHubAuthId()
        {
            using var transaction = new TransactionScope();

            // Given
            const int delegateId = 3;
            const int learningHubAuthId = 1234;

            // When
            userDataService.SetDelegateUserLearningHubAuthId(delegateId, learningHubAuthId);

            // Then
            var result = userDataService.GetDelegateUserLearningHubAuthId(delegateId);

            result.Should().NotBeNull()
                .And.Subject.Should().Be(learningHubAuthId);
        }

        [Test]
        public void GetDelegateUserLearningHubAuthId_returns_null_delegate_learningHubAuthId()
        {
            using var transaction = new TransactionScope();

            // Given
            const int delegateId = 3;

            // When
            var result = userDataService.GetDelegateUserLearningHubAuthId(delegateId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void UpdateDelegateLhLoginWarningDismissalStatus_changes_delegate_dismissal_status()
        {
            using var transaction = new TransactionScope();

            // When
            userDataService.UpdateDelegateLhLoginWarningDismissalStatus(2, true);

            // Then
            var updatedUser = userDataService.GetDelegateUserById(2)!;
            updatedUser.HasDismissedLhLoginWarning.Should().BeTrue();
        }

        [Test]
        public void UpdateDelegateProfessionalRegistrationNumber_sets_ProfessionalRegistrationNumber()
        {
            // Given
            const int delegateId = 2;
            const string prn = "PRN123";

            using var transaction = new TransactionScope();

            // When
            userDataService.UpdateDelegateProfessionalRegistrationNumber(delegateId, prn, true);

            // Then
            var updatedUser = userDataService.GetDelegateUserById(delegateId)!;
            updatedUser.ProfessionalRegistrationNumber.Should().Be(prn);
            updatedUser.HasBeenPromptedForPrn.Should().BeTrue();
        }

        [Test]
        public void GetDelegateAccountsByUserId_returns_expected_accounts()
        {
            // When
            var result = userDataService.GetDelegateAccountsByUserId(61188).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.Single().Should().BeEquivalentTo(UserTestHelper.GetDefaultDelegateAccount());
            }
        }

        [Test]
        public void GetDelegateAccountById_returns_expected_account()
        {
            // When
            var result = userDataService.GetDelegateAccountById(2);

            // Then
            result.Should().BeEquivalentTo(UserTestHelper.GetDefaultDelegateAccount());
        }

        [Test]
        [TestCase(null)]
        [TestCase("hash")]
        public void SetRegistrationConfirmationHash_sets_hash(string? hash)
        {
            using var transaction = new TransactionScope();

            // Given
            const int userId = 1;
            const int centreId = 101;

            // When
            userDataService.SetRegistrationConfirmationHash(userId, centreId, hash);

            // Then
            var result = connection.Query<string>(
                @"SELECT RegistrationConfirmationHash
                    FROM DelegateAccounts
                    WHERE UserID = @userId AND CentreID = @centreId",
                new { userId, centreId }
            ).SingleOrDefault();
            result.Should().Be(hash);
        }

        [Test]
        public void LinkDelegateAccountToNewUser_updates_UserId_and_sets_RegistrationConfirmationHash_to_null()
        {
            using var transaction = new TransactionScope();

            // Given
            const int userIdForDelegateAccountAfterUpdate = 2;

            var delegateEntity = userDataService.GetDelegateByCandidateNumber("CLAIMABLEUSER1")!;
            var currentUserIdForDelegateAccount = delegateEntity.UserAccount.Id;
            var delegateId = delegateEntity.DelegateAccount.Id;
            var centreId = delegateEntity.DelegateAccount.CentreId;

            var newUserDelegateAccountsBeforeUpdate =
                userDataService.GetDelegateAccountsByUserId(userIdForDelegateAccountAfterUpdate);

            // When
            userDataService.LinkDelegateAccountToNewUser(
                currentUserIdForDelegateAccount,
                userIdForDelegateAccountAfterUpdate,
                centreId
            );

            // Then
            newUserDelegateAccountsBeforeUpdate.Should()
                .NotContain(delegateAccount => delegateAccount.CentreId == centreId);

            var updatedDelegateEntity = userDataService.GetDelegateById(delegateId)!;

            updatedDelegateEntity.UserAccount.Id.Should().Be(userIdForDelegateAccountAfterUpdate);
            updatedDelegateEntity.DelegateAccount.RegistrationConfirmationHash.Should().Be(null);
        }
    }
}
