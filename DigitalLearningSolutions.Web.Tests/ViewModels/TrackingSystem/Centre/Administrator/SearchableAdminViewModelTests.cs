namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Administrator
{
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using FluentAssertions;
    using NUnit.Framework;

    public class SearchableAdminViewModelTests
    {
        [Test]
        public void SearchableAdmin_show_deactivate_admin_button_for_super_admin()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(2, firstName: "a", lastName: "Surname");

            // When
            var model = new SearchableAdminViewModel(
                adminUser,
                true,
                1
            );

            // Then
            model.CanShowDeactivateAdminButton.Should().BeTrue();
        }

        [Test]
        public void SearchableAdmin_do_not_show_deactivate_admin_button_if_user_do_not_have_super_admin_access()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(firstName: "a", lastName: "Surname");

            // When
            var model = new SearchableAdminViewModel(
                adminUser,
                false,
                1
            );

            // Then
            model.CanShowDeactivateAdminButton.Should().BeFalse();
        }

        [Test]
        public void SearchableAdmin_do_not_show_deactivate_admin_button_for_logged_in_admin_user_card()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(1, firstName: "a", lastName: "Surname");

            // When
            var model = new SearchableAdminViewModel(
                adminUser,
                false,
                1
            );

            // Then
            model.CanShowDeactivateAdminButton.Should().BeFalse();
        }
    }
}
