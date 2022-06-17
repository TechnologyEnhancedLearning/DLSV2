namespace DigitalLearningSolutions.Data.Tests.Models
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class AdminRegistrationModelTests
    {
        [Test]
        public void AdminRegistrationModel_with_isCmsAdmin_true_populates_expected_fields()
        {
            // Given
            var adminRegistrationModel =
                RegistrationModelTestHelper.GetDefaultAdminRegistrationModel(isCmsAdmin: true, isCmsManager: false);

            // Then
            adminRegistrationModel.IsContentManager.Should().BeTrue();
            adminRegistrationModel.ImportOnly.Should().BeTrue();
        }

        [Test]
        public void AdminRegistrationModel_with_isCmsManager_true_populates_expected_fields()
        {
            // Given
            var adminRegistrationModel =
                RegistrationModelTestHelper.GetDefaultAdminRegistrationModel(isCmsAdmin: false, isCmsManager: true);

            // Then
            adminRegistrationModel.IsContentManager.Should().BeTrue();
            adminRegistrationModel.ImportOnly.Should().BeFalse();
        }

        [Test]
        public void AdminRegistrationModel_without_isCmsManager_or_isCmsAdmin_populates_expected_fields()
        {
            // Given
            var adminRegistrationModel =
                RegistrationModelTestHelper.GetDefaultAdminRegistrationModel(isCmsAdmin: false, isCmsManager: false);

            // Then
            adminRegistrationModel.IsContentManager.Should().BeFalse();
            adminRegistrationModel.ImportOnly.Should().BeFalse();
        }

        [Test]
        public void GetNotificationRoles_returns_full_list_with_all_roles()
        {
            // Given
            var expectedRoles = new List<int> { 1, 2, 3, 4, 6, 7, 8 };
            var adminRegistrationModel = new AdminRegistrationModel(
                "Test",
                "Name",
                "test@email.com",
                null,
                1,
                null,
                true,
                true,
                "PRN",
                1,
                null,
                true,
                true,
                true,
                true,
                true,
                true,
                true,
                true
            );

            // When
            var result = adminRegistrationModel.GetNotificationRoles();

            // Then
            result.Should().BeEquivalentTo(expectedRoles);
        }

        [Test]
        public void GetNotifcationRoles_returns_1_2_for_new_centre_manager()
        {
            // Given
            var expectedRoles = new List<int> { 1, 2 };
            var adminRegistrationModel = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();

            // When
            var result = adminRegistrationModel.GetNotificationRoles();

            // Then
            result.Should().BeEquivalentTo(expectedRoles);
        }
    }
}
