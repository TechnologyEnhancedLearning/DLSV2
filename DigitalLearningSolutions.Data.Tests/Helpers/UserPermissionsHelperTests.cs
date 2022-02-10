namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class UserPermissionsHelperTests
    {
        
        [Test]
        public void LoggedInAdminCanDeactivateUser_centre_manager_should_not_be_able_to_deactivate_their_own_account()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(id: 1, isCentreManager: true);
            var loggedInAdminUser = UserTestHelper.GetDefaultAdminUser(id: 1, isCentreManager: true);

            // When
            var result = UserPermissionsHelper.LoggedInAdminCanDeactivateUser(adminUser, loggedInAdminUser);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void LoggedInAdminCanDeactivateUser_centre_manager_should_not_be_able_to_deactivate_other_centre_manager()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(id: 1, isCentreManager: true, isUserAdmin: false);
            var loggedInAdminUser = UserTestHelper.GetDefaultAdminUser(id: 2, isCentreManager: true, isUserAdmin: false);

            // When
            var result = UserPermissionsHelper.LoggedInAdminCanDeactivateUser(adminUser, loggedInAdminUser);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void LoggedInAdminCanDeactivateUser_centre_manager_should_not_be_able_to_deactivate_super_admin_account()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(id: 1, isUserAdmin: true);
            var loggedInAdminUser = UserTestHelper.GetDefaultAdminUser(id: 2, isCentreManager: true, isUserAdmin: false);


            // When
            var result = UserPermissionsHelper.LoggedInAdminCanDeactivateUser(adminUser, loggedInAdminUser);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void LoggedInAdminCanDeactivateUser_super_admin_should_not_be_able_to_deactivate_their_own_account()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(id: 1, isUserAdmin: true);
            var loggedInAdminUser = UserTestHelper.GetDefaultAdminUser(id: 1, isUserAdmin: true);

            // When
            var result = UserPermissionsHelper.LoggedInAdminCanDeactivateUser(adminUser, loggedInAdminUser);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void LoggedInAdminCanDeactivateUser_super_admin_should_be_able_to_deactivate_other_super_admin()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(id: 1, isUserAdmin: true);
            var loggedInAdminUser = UserTestHelper.GetDefaultAdminUser(id: 2, isUserAdmin: true);

            // When
            var result = UserPermissionsHelper.LoggedInAdminCanDeactivateUser(adminUser, loggedInAdminUser);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void LoggedInAdminCanDeactivateUser_super_admin_should_be_able_to_deactivate_centre_manager()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(id: 1, isCentreManager: true);
            var loggedInAdminUser = UserTestHelper.GetDefaultAdminUser(id: 2, isUserAdmin: true);

            // When
            var result = UserPermissionsHelper.LoggedInAdminCanDeactivateUser(adminUser, loggedInAdminUser);

            // Then
            result.Should().BeTrue();
        }
    }
}
