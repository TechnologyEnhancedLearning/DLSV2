﻿namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class UserDataServiceTests
    {
        private IUserDataService userDataService;

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
            var expectedDelegateUsers = UserTestHelper.GetDefaultDelegateUser(jobGroupName: "Nursing / midwifery");

            //When
            var returnedDelegateUser = userDataService.GetDelegateUserById(2);

            // Then
            returnedDelegateUser.Should().BeEquivalentTo(expectedDelegateUsers);
        }

        [Test]
        public void UpdateAdminUser_updates_user()
        {
            using (var transaction = new TransactionScope())
            {
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
        }

        [Test]
        public void UpdateDelegateUsers_updates_users()
        {
            using (var transaction = new TransactionScope())
            {
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
        }

        [Test]
        public void UpdateDelegateUserCentrePrompts_updates_user()
        {
            using (var transaction = new TransactionScope())
            {
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
                    userDataService.UpdateDelegateUserCentrePrompts
                        (2, jobGroupId, answer1, answer2, answer3, answer4, answer5, answer6);
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
        }
    }
}
