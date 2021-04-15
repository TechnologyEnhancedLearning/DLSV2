namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class UserServiceTests
    {
        private IUserService userService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            userService = new UserService(connection);
        }

        [Test]
        public void GetAdminUserByUsername_Returns_admin_user()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();

            //When
            var returnedAdminUser = userService.GetAdminUserByUsername("Username");
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
            var returnedDelegateUsers = userService.GetDelegateUsersByUsername("SV1234");
            returnedDelegateUsers.First().Id = expectedDelegateUsers.Id;

            // Then
            returnedDelegateUsers.FirstOrDefault().Should().BeEquivalentTo(expectedDelegateUsers);
        }

        [Test]
        public void GetUsersByUsername_Returns_admin_user_and_delegate_users()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();
            var expectedDelegateUsers = UserTestHelper.GetDefaultDelegateUser();

            //When
            var (returnedAdminUser, _) = userService.GetUsersByUsername("Username");
            var (_, returnedDelegateUsers) = userService.GetUsersByUsername("SV1234");
            returnedAdminUser.Id = expectedAdminUser.Id;
            returnedDelegateUsers.First().Id = expectedDelegateUsers.Id;

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
            returnedDelegateUsers.FirstOrDefault().Should().BeEquivalentTo(expectedDelegateUsers);
        }
    }
}
