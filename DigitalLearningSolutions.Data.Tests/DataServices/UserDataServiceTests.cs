﻿namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Mappers;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public class UserDataServiceTests
    {
        private IUserDataService userDataService = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MapperHelper.SetUpFluentMapper();
        }

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            userDataService = new UserDataService(connection);
        }

        [Test]
        public void GetAdminUserByUsername_Returns_admin_user()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();

            // When
            var returnedAdminUser = userDataService.GetAdminUserByUsername("Username");

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
        }

        [Test]
        public void GetDelegateUsersByUsername_Returns_delegate_user()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();

            //When
            var returnedDelegateUsers = userDataService.GetDelegateUsersByUsername("SV1234");

            // Then
            returnedDelegateUsers.FirstOrDefault().Should().BeEquivalentTo(expectedDelegateUser);
        }

        [Test]
        public void GetAllDelegateUsersByUsername_Returns_delegate_user()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();

            //When
            var returnedDelegateUsers = userDataService.GetAllDelegateUsersByUsername("SV1234");

            // Then
            returnedDelegateUsers.FirstOrDefault().Should().BeEquivalentTo(expectedDelegateUser);
        }

        [Test]
        public void GetAllDelegateUsersByUsername_includes_inactive_users()
        {
            //When
            var returnedDelegateUsers = userDataService.GetAllDelegateUsersByUsername("OS35");

            // Then
            returnedDelegateUsers.FirstOrDefault()!.Id.Should().Be(89094);
        }

        [Test]
        public void GetAllDelegateUsersByUsername_search_includes_CandidateNumber()
        {
            //When
            var returnedDelegateUsers = userDataService.GetAllDelegateUsersByUsername("ND107");

            // Then
            returnedDelegateUsers.FirstOrDefault()!.Id.Should().Be(78051);
        }

        [Test]
        public void GetAllDelegateUsersByUsername_search_includes_EmailAddress()
        {
            //When
            var returnedDelegateUsers = userDataService.GetAllDelegateUsersByUsername("saudnhb@.5lpyk");

            // Then
            returnedDelegateUsers.FirstOrDefault()!.Id.Should().Be(78051);
        }

        [Test]
        public void GetAllDelegateUsersByUsername_searches_AliasID()
        {
            //When
            var returnedDelegateUsers = userDataService.GetAllDelegateUsersByUsername("aldn y");

            // Then
            returnedDelegateUsers.FirstOrDefault()!.Id.Should().Be(78051);
        }

        [Test]
        public void GetAdminUserById_Returns_admin_user()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();

            // When
            var returnedAdminUser = userDataService.GetAdminUserById(7);

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
        }

        [Test]
        public void GetDelegateUserById_Returns_delegate_users()
        {
            // Given
            var expectedDelegateUsers = UserTestHelper.GetDefaultDelegateUser(
                dateRegistered: DateTime.Parse("2010-09-22 06:52:09.080"),
                jobGroupName: "Nursing / midwifery"
            );

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
        public void GetAdminUserByEmailAddress_Returns_admin_user()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();

            // When
            var returnedAdminUser = userDataService.GetAdminUserByEmailAddress("test@gmail.com");

            // Then
            using (new AssertionScope())
            {
                returnedAdminUser.Should().NotBeNull();
                returnedAdminUser!.Id.Should().Be(expectedAdminUser.Id);
                returnedAdminUser!.FirstName.Should().BeEquivalentTo(expectedAdminUser.FirstName);
                returnedAdminUser!.LastName.Should().BeEquivalentTo(expectedAdminUser.LastName);
                returnedAdminUser!.EmailAddress.Should().BeEquivalentTo(expectedAdminUser.EmailAddress);
                returnedAdminUser!.Password.Should().BeEquivalentTo(expectedAdminUser.Password);
                returnedAdminUser!.ResetPasswordId.Should().Be(expectedAdminUser.ResetPasswordId);
            }
        }

        [Test]
        public void GetDelegateUsersByEmailAddress_Returns_delegate_user()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();

            //When
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
        public void UpdateAdminUser_updates_user()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                var firstName = "TestFirstName";
                var lastName = "TestLastName";
                var email = "test@email.com";

                // When
                userDataService.UpdateAdminUser(firstName, lastName, email, null, 7);
                var updatedUser = userDataService.GetAdminUserById(7);

                // Then
                updatedUser.FirstName.Should().BeEquivalentTo(firstName);
                updatedUser.LastName.Should().BeEquivalentTo(lastName);
                updatedUser.EmailAddress.Should().BeEquivalentTo(email);
            }
            finally
            {
                transaction.Dispose();
            }
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

                // When
                userDataService.UpdateDelegateUsers(firstName, lastName, email, null, new[] { 2, 3 });
                var updatedUser = userDataService.GetDelegateUserById(2);
                var secondUpdatedUser = userDataService.GetDelegateUserById(3);

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
        public void UpdateDelegateUserCentrePrompts_updates_user()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                var jobGroupId = 1;
                var jobGroupName = "Nursing / midwifery";
                var answer1 = "Answer1";
                var answer2 = "Answer2";
                var answer3 = "Answer3";
                var answer4 = "Answer4";
                var answer5 = "Answer5";
                var answer6 = "Answer6";

                // When
                userDataService.UpdateDelegateUserCentrePrompts(
                    2,
                    jobGroupId,
                    answer1,
                    answer2,
                    answer3,
                    answer4,
                    answer5,
                    answer6
                );
                var updatedUser = userDataService.GetDelegateUserById(2);

                // Then
                using (new AssertionScope())
                {
                    updatedUser.JobGroupName.Should().BeEquivalentTo(jobGroupName);
                    updatedUser.Answer1.Should().BeEquivalentTo(answer1);
                    updatedUser.Answer2.Should().BeEquivalentTo(answer2);
                    updatedUser.Answer3.Should().BeEquivalentTo(answer3);
                    updatedUser.Answer4.Should().BeEquivalentTo(answer4);
                    updatedUser.Answer5.Should().BeEquivalentTo(answer5);
                    updatedUser.Answer6.Should().BeEquivalentTo(answer6);
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
                var delegate1 = userDataService.GetDelegateUserById(16);
                var delegate2 = userDataService.GetDelegateUserById(28);

                delegate1.Approved.Should().BeFalse();
                delegate2.Approved.Should().BeFalse();

                // When
                userDataService.ApproveDelegateUsers(ids);

                var delegate1AfterUpdate = userDataService.GetDelegateUserById(16);
                var delegate2AfterUpdate = userDataService.GetDelegateUserById(28);

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
        public void GetDelegateCountWithAnswerForPrompt_returns_expected_count()
        {
            // When
            var count = userDataService.GetDelegateCountWithAnswerForPrompt(101, 1);

            // Then
            count.Should().Be(124);
        }

        [Test]
        public void GetAdminUsersByCentreId_gets_all_admins_at_centre()
        {
            // Given
            var expectedAdminIds = new List<int> { 7, 1408, 2464 };

            // When
            var admins = userDataService.GetAdminUsersByCentreId(2);
            var returnedIds = admins.Select(a => a.Id);

            // Then
            Assert.That(returnedIds.SequenceEqual(expectedAdminIds));
        }

        [Test]
        public void GetAdminUsersByCentreId_populates_correct_properties_on_admin()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();

            // When
            var admin = userDataService.GetAdminUsersByCentreId(2).Single(a => a.Id == 7);

            // Then
            admin.Should().BeEquivalentTo(expectedAdminUser);
        }

        [Test]
        public void GetNumberOfActiveAdminsAtCentre_returns_expected_count()
        {
            // When
            var count = userDataService.GetNumberOfActiveAdminsAtCentre(2);

            // Then
            count.Should().Be(3);
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
        public void GetDelegateUserCardsByCentreId_populates_DelegateUser_fields_correctly()
        {
            // Given
            var expected = UserTestHelper.GetDefaultDelegateUser(
                dateRegistered: DateTime.Parse("2010-09-22 06:52:09.080"),
                jobGroupName: "Nursing / midwifery"
            );

            // When
            var userCards = userDataService.GetDelegateUserCardsByCentreId(2);

            // Then
            var userCard = userCards.Single(user => user.Id == 2);
            userCard.Should().BeEquivalentTo(expected);
            userCard.Active.Should().BeTrue();
            userCard.SelfReg.Should().BeFalse();
            userCard.ExternalReg.Should().BeFalse();
            userCard.AdminId.Should().BeNull();
            userCard.AliasId.Should().BeNull();
            userCard.JobGroupId.Should().Be(1);
        }

        [Test]
        public void GetDelegateUserCardsByCentreId_populates_DelegateUserCard_admin_fields_correctly()
        {
            // When
            var userCards = userDataService.GetDelegateUserCardsByCentreId(279);

            // Then
            var userCard = userCards.Single(user => user.Id == 97055);
            userCard.Active.Should().BeTrue();
            userCard.SelfReg.Should().BeTrue();
            userCard.ExternalReg.Should().BeFalse();
            userCard.AdminId.Should().Be(74);
            userCard.AliasId.Should().Be("");
            userCard.JobGroupId.Should().Be(6);
        }
    }
}
