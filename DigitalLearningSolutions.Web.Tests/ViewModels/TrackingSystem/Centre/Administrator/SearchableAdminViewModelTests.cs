namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Administrator
{
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using FluentAssertions;
    using NUnit.Framework;

    public class SearchableAdminViewModelTests
    {
        
        [Test]
        public void SearchableAdmin_centre_manager_should_not_be_able_to_deactivate_their_own_account()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(id: 1, isCentreAdmin: true);
            var loggedInAdminUser = UserTestHelper.GetDefaultAdminUser(id: 1, isCentreAdmin: true);

            // When
            var model = new SearchableAdminViewModel(
                adminUser,
                loggedInAdminUser
            );

            // Then
            model.CanShowDeactivateAdminButton.Should().BeFalse();
        }

        public void SearchableAdmin_centre_manager_should_not_be_able_to_deactivate_other_centre_manager()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(id: 1, isCentreAdmin: true);
            var loggedInAdminUser = UserTestHelper.GetDefaultAdminUser(id: 1, isCentreAdmin: true);

            // When
            var model = new SearchableAdminViewModel(
                adminUser,
                loggedInAdminUser
            );

            // Then
            model.CanShowDeactivateAdminButton.Should().BeFalse();
        }

        [Test]
        public void SearchableAdmin_centre_manager_should_not_be_able_to_deactivate_super_admin_account()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(id: 1, isCentreAdmin: true);
            var loggedInAdminUser = UserTestHelper.GetDefaultAdminUser(id: 1, isUserAdmin: true);

            // When
            var model = new SearchableAdminViewModel(
                adminUser,
                loggedInAdminUser
            );

            // Then
            model.CanShowDeactivateAdminButton.Should().BeFalse();
        }

        [Test]
        public void SearchableAdmin_super_admin_should_not_be_able_to_deactivate_their_own_account()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(id: 1, isUserAdmin: true);
            var loggedInAdminUser = UserTestHelper.GetDefaultAdminUser(id: 1, isUserAdmin: true);

            // When
            var model = new SearchableAdminViewModel(
                adminUser,
                loggedInAdminUser
            );

            // Then
            model.CanShowDeactivateAdminButton.Should().BeFalse();
        }

        [Test]
        public void SearchableAdmin_super_admin_should_be_able_to_deactivate_other_super_admin()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(id: 1, isUserAdmin: true);
            var loggedInAdminUser = UserTestHelper.GetDefaultAdminUser(id: 2, isUserAdmin: true);

            // When
            var model = new SearchableAdminViewModel(
                adminUser,
                loggedInAdminUser
            );

            // Then
            model.CanShowDeactivateAdminButton.Should().BeTrue();
        }
    }
}
