namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class UserServiceTests
    {
        private IUserService userService;
        private IUserDataService userDataService;

        [SetUp]
        public void Setup()
        {
            userDataService = A.Fake<IUserDataService>();
            userService = new UserService(userDataService);
        }

        [Test]
        public void GetUsersByUsername_Returns_admin_user_and_delegate_users()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();
            var expectedDelegateUsers = UserTestHelper.GetDefaultDelegateUser();
            A.CallTo(() => userDataService.GetAdminUserByUsername(A<string>._)).Returns(UserTestHelper.GetDefaultAdminUser());
            A.CallTo(() => userDataService.GetDelegateUsersByUsername(A<string>._)).Returns(new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() });

            //When
            var (returnedAdminUser, _) = userService.GetUsersByUsername("Username");
            var (_, returnedDelegateUsers) = userService.GetUsersByUsername("SV1234");
            returnedAdminUser.Id = expectedAdminUser.Id;
            returnedDelegateUsers.First().Id = expectedDelegateUsers.Id;

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
            returnedDelegateUsers.FirstOrDefault().Should().BeEquivalentTo(expectedDelegateUsers);
        }

        [Test]
        public void GetUsersById_Returns_admin_user_and_delegate_user()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).Returns(UserTestHelper.GetDefaultAdminUser());
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).Returns(UserTestHelper.GetDefaultDelegateUser());

            //When
            var (returnedAdminUser, _) = userService.GetUsersById("1", "2");
            var (_, returnedDelegateUser) = userService.GetUsersById("1", "2");

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
            returnedDelegateUser.Should().BeEquivalentTo(expectedDelegateUser);
        }

        [Test]
        public void GetUsersById_Returns_admin_user()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).Returns(UserTestHelper.GetDefaultAdminUser());

            //When
            var (returnedAdminUser, _) = userService.GetUsersById("1", null);
            var (_, returnedDelegateUser) = userService.GetUsersById("1", null);

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
            returnedDelegateUser.Should().BeNull();
        }

        [Test]
        public void GetUsersById_Returns_delegate_user()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).Returns(UserTestHelper.GetDefaultDelegateUser());

            //When
            var (returnedAdminUser, _) = userService.GetUsersById(null, "2");
            var (_, returnedDelegateUser) = userService.GetUsersById(null, "2");

            // Then
            returnedAdminUser.Should().BeNull();
            returnedDelegateUser.Should().BeEquivalentTo(expectedDelegateUser);
        }

        [Test]
        public void GetUsersById_Returns_nulls_with_unexpected_input()
        {
            // When
            var (returnedAdminUser, _) = userService.GetUsersById("can't int.Parse this string", "can't int.Parse this string");
            var (_, returnedDelegateUser) = userService.GetUsersById("can't int.Parse this string", "can't int.Parse this string");

            // Then
            returnedAdminUser.Should().BeNull();
            returnedDelegateUser.Should().BeNull();
        }
    }
}
