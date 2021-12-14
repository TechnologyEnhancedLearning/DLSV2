namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
{
    using System;
    using System.Linq;
    using System.Transactions;
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
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();

            // When
            var returnedDelegateUsers = userDataService.GetDelegateUsersByEmailAddress("email@test.com");

            // Then
            using (new AssertionScope())
            {
                returnedDelegateUsers.FirstOrDefault().Should().NotBeNull();
                returnedDelegateUsers.First().Id.Should().Be(expectedDelegateUser.Id);
                returnedDelegateUsers.First().CandidateNumber.Should().BeEquivalentTo
                    (expectedDelegateUser.CandidateNumber);
                returnedDelegateUsers.First().CentreId.Should().Be(expectedDelegateUser.CentreId);
                returnedDelegateUsers.First().CentreName.Should().BeEquivalentTo(expectedDelegateUser.CentreName);
                returnedDelegateUsers.First().CentreActive.Should().Be(expectedDelegateUser.CentreActive);
                returnedDelegateUsers.First().EmailAddress.Should().BeEquivalentTo(expectedDelegateUser.EmailAddress);
                returnedDelegateUsers.First().FirstName.Should().BeEquivalentTo(expectedDelegateUser.FirstName);
                returnedDelegateUsers.First().LastName.Should().BeEquivalentTo(expectedDelegateUser.LastName);
                returnedDelegateUsers.First().Password.Should().BeEquivalentTo(expectedDelegateUser.Password);
                returnedDelegateUsers.First().Approved.Should().Be(expectedDelegateUser.Approved);
                returnedDelegateUsers.First().ResetPasswordId.Should().Be(expectedDelegateUser.ResetPasswordId);
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
                var firstName = "TestFirstName";
                var lastName = "TestLastName";
                var email = "test@email.com";
                var professionalRegNumber = "test-1234";

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

            // given
            userDataService.GetDelegateUserById(1)!.Active.Should().BeTrue();

            // when
            userDataService.DeactivateDelegateUser(1);

            // then
            userDataService.GetDelegateUserById(1)!.Active.Should().BeFalse();
        }
    }
}
