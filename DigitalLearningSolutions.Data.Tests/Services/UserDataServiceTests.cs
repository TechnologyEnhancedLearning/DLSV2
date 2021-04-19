﻿namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FluentAssertions;
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

            //When
            var returnedAdminUser = userDataService.GetAdminUserByUsername("Username");
            returnedAdminUser.Id = expectedAdminUser.Id;

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
        }

        [Test]
        public void GetDelegateUsersByUsername_Returns_delegate_users()
        {
            // Given
            var expectedDelegateUsers = UserTestHelper.GetDefaultDelegateUser();

            //When
            var returnedDelegateUsers = userDataService.GetDelegateUsersByUsername("SV1234");
            returnedDelegateUsers.First().Id = expectedDelegateUsers.Id;

            // Then
            returnedDelegateUsers.FirstOrDefault().Should().BeEquivalentTo(expectedDelegateUsers);
        }

        [Test]
        public void GetAdminUserById_Returns_admin_user()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();

            // When
            var returnedAdminUser = userDataService.GetAdminUserById(4259);
            returnedAdminUser.Id = expectedAdminUser.Id;

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
        }

        [Test]
        public void GetDelegateUserById_Returns_delegate_users()
        {
            // Given
            var expectedDelegateUsers = UserTestHelper.GetDefaultDelegateUser();

            //When
            var returnedDelegateUser = userDataService.GetDelegateUserById(299639);
            returnedDelegateUser.Id = expectedDelegateUsers.Id;

            // Then
            returnedDelegateUser.Should().BeEquivalentTo(expectedDelegateUsers);
        }
    }
}
