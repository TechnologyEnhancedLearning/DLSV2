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
        public void GetDelegateUsersByUsername_Returns_delegate_user()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();

            // When
            var returnedDelegateUsers = userDataService.GetDelegateUsersByUsername("SV1234");

            // Then
            returnedDelegateUsers.FirstOrDefault().Should().BeEquivalentTo(expectedDelegateUser);
        }

        [Test]
        public void GetAllDelegateUsersByUsername_Returns_delegate_user()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();

            // When
            var returnedDelegateUsers = userDataService.GetAllDelegateUsersByUsername("SV1234");

            // Then
            returnedDelegateUsers.FirstOrDefault().Should().BeEquivalentTo(expectedDelegateUser);
        }

        [Test]
        public void GetAllDelegateUsersByUsername_includes_inactive_users()
        {
            // When
            var returnedDelegateUsers = userDataService.GetAllDelegateUsersByUsername("OS35");

            // Then
            returnedDelegateUsers.FirstOrDefault()!.Id.Should().Be(89094);
        }

        [Test]
        public void GetAllDelegateUsersByUsername_search_includes_CandidateNumber()
        {
            // When
            var returnedDelegateUsers = userDataService.GetAllDelegateUsersByUsername("ND107");

            // Then
            returnedDelegateUsers.FirstOrDefault()!.Id.Should().Be(78051);
        }

        [Test]
        public void GetAllDelegateUsersByUsername_search_includes_EmailAddress()
        {
            // When
            var returnedDelegateUsers = userDataService.GetAllDelegateUsersByUsername("saudnhb@.5lpyk");

            // Then
            returnedDelegateUsers.FirstOrDefault()!.Id.Should().Be(78051);
        }

        [Test]
        public void GetAllDelegateUsersByUsername_searches_AliasID()
        {
            // When
            var returnedDelegateUsers = userDataService.GetAllDelegateUsersByUsername("aldn y");

            // Then
            returnedDelegateUsers.FirstOrDefault()!.Id.Should().Be(78051);
        }

        [Test]
        public void GetDelegateUsersByEmailAddress_Returns_delegate_user()
        {
            using (new TransactionScope())
            {
                using (new AssertionScope())
                {
                    // Given
                    var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser(resetPasswordId: 1);
                    connection.Execute("UPDATE Candidates SET ResetPasswordID = 1 WHERE CandidateID = 2");

                    // When
                    var returnedDelegateUsers = userDataService.GetDelegateUsersByEmailAddress("email@test.com");

                    // Then
                    returnedDelegateUsers.FirstOrDefault().Should().NotBeNull();
                    returnedDelegateUsers.First().Id.Should().Be(expectedDelegateUser.Id);
                    returnedDelegateUsers.First().CandidateNumber.Should().BeEquivalentTo
                        (expectedDelegateUser.CandidateNumber);
                    returnedDelegateUsers.First().CentreId.Should().Be(expectedDelegateUser.CentreId);
                    returnedDelegateUsers.First().CentreName.Should().BeEquivalentTo(expectedDelegateUser.CentreName);
                    returnedDelegateUsers.First().CentreActive.Should().Be(expectedDelegateUser.CentreActive);
                    returnedDelegateUsers.First().EmailAddress.Should()
                        .BeEquivalentTo(expectedDelegateUser.EmailAddress);
                    returnedDelegateUsers.First().FirstName.Should().BeEquivalentTo(expectedDelegateUser.FirstName);
                    returnedDelegateUsers.First().LastName.Should().BeEquivalentTo(expectedDelegateUser.LastName);
                    returnedDelegateUsers.First().Password.Should().BeEquivalentTo(expectedDelegateUser.Password);
                    returnedDelegateUsers.First().Approved.Should().Be(expectedDelegateUser.Approved);
                    returnedDelegateUsers.First().ResetPasswordId.Should().Be(expectedDelegateUser.ResetPasswordId);
                }
            }
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
        public void UpdateDelegateUsers_updates_users()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const string firstName = "TestFirstName";
                const string lastName = "TestLastName";
                const string email = "test@email.com";
                const string professionalRegNumber = "test-1234";

                // When
                userDataService.UpdateDelegateUsers(firstName, lastName, email, null, professionalRegNumber, true, new[] { 2, 3 });
                var updatedUser = userDataService.GetDelegateUserById(2)!;
                var secondUpdatedUser = userDataService.GetDelegateUserById(3)!;

                // Then
                updatedUser.FirstName.Should().BeEquivalentTo(firstName);
                updatedUser.LastName.Should().BeEquivalentTo(lastName);
                updatedUser.EmailAddress.Should().BeEquivalentTo(email);

                secondUpdatedUser.FirstName.Should().BeEquivalentTo(firstName);
                secondUpdatedUser.LastName.Should().BeEquivalentTo(lastName);
                secondUpdatedUser.EmailAddress.Should().BeEquivalentTo(email);
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
        public void RemoveDelegateUser_deletes_delegate_user()
        {
            using var transaction = new TransactionScope();

            // Given
            var id = 610;
            userDataService.GetDelegateUserById(id).Should().NotBeNull();

            // When
            userDataService.RemoveDelegateUser(id);

            // Then
            userDataService.GetDelegateUserById(id).Should().BeNull();
        }

        [Test]
        public void RemoveDelegateUser_cannot_remove_delegate_user_with_started_session()
        {
            using var transaction = new TransactionScope();

            // Given
            var id = 16;
            userDataService.GetDelegateUserById(id).Should().NotBeNull();

            // When
            Action action = () => userDataService.RemoveDelegateUser(id);

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
        public void GetDelegateUserByAliasId_Returns_delegate_users()
        {
            // Given
            var expectedDelegateUser = userDataService.GetDelegateUserById(1);

            // When
            var returnedDelegateUser = userDataService.GetDelegateUserByAliasId("ohi@lt.vgmwekac", 101);

            // Then
            returnedDelegateUser.Should().BeEquivalentTo(expectedDelegateUser);
        }

        [Test]
        public void GetDelegateUserByCandidateNumber_Returns_delegate_user()
        {
            // Given
            var expectedDelegateUsers = UserTestHelper.GetDefaultDelegateUser();

            // When
            var returnedDelegateUser = userDataService.GetDelegateUserByCandidateNumber("SV1234", 2);

            // Then
            returnedDelegateUser.Should().BeEquivalentTo(expectedDelegateUsers);
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
        public void GetDelegateUsersByAliasId_returns_expected_delegates()
        {
            // Given
            const string alias = "1086";
            var expectedIds = new[]
            {
                17867,
                19258,
                202870,
                203415,
                165982,
                166032,
                166033,
                166052,
                169397,
                170540,
                170562,
                170737,
            };

            // When
            var result = userDataService.GetDelegateUsersByAliasId(alias).ToList();

            // Then
            result.Count.Should().Be(12);
            result.Select(d => d.Id).Should().BeEquivalentTo(expectedIds);
        }

        [Test]
        public void UpdateDelegate_updates_delegate()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const int delegateId = 11;
                const string firstName = "TestFirstName";
                const string lastName = "TestLastName";
                const string email = "test@email.com";
                const int jobGroupId = 1;
                const bool active = true;
                const string answer1 = "answer1";
                const string answer2 = "answer2";
                const string answer3 = "answer3";
                const string answer4 = "answer4";
                const string answer5 = "answer5";
                const string answer6 = "answer6";
                const string alias = "alias";

                // When
                userDataService.UpdateDelegate(
                    delegateId,
                    firstName,
                    lastName,
                    jobGroupId,
                    active,
                    answer1,
                    answer2,
                    answer3,
                    answer4,
                    answer5,
                    answer6,
                    alias,
                    email
                );
                var delegateUser = userDataService.GetDelegateUserById(delegateId)!;

                // Then
                using (new AssertionScope())
                {
                    delegateUser.FirstName.Should().Be(firstName);
                    delegateUser.LastName.Should().Be(lastName);
                    delegateUser.EmailAddress.Should().Be(email);
                    delegateUser.JobGroupId.Should().Be(jobGroupId);
                    delegateUser.Active.Should().Be(active);
                    delegateUser.Answer1.Should().Be(answer1);
                    delegateUser.Answer2.Should().Be(answer2);
                    delegateUser.Answer3.Should().Be(answer3);
                    delegateUser.Answer4.Should().Be(answer4);
                    delegateUser.Answer5.Should().Be(answer5);
                    delegateUser.Answer6.Should().Be(answer6);
                    delegateUser.AliasId.Should().Be(alias);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void UpdateDelegateAccountDetails_updates_users()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const string firstName = "TestFirstName";
                const string lastName = "TestLastName";
                const string email = "test@email.com";

                // When
                userDataService.UpdateDelegateAccountDetails(firstName, lastName, email, new[] { 2, 3 });
                var updatedUser = userDataService.GetDelegateUserById(2)!;
                var secondUpdatedUser = userDataService.GetDelegateUserById(3)!;

                // Then
                using (new AssertionScope())
                {
                    updatedUser.FirstName.Should().BeEquivalentTo(firstName);
                    updatedUser.LastName.Should().BeEquivalentTo(lastName);
                    updatedUser.EmailAddress.Should().BeEquivalentTo(email);

                    secondUpdatedUser.FirstName.Should().BeEquivalentTo(firstName);
                    secondUpdatedUser.LastName.Should().BeEquivalentTo(lastName);
                    secondUpdatedUser.EmailAddress.Should().BeEquivalentTo(email);
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
    }
}
