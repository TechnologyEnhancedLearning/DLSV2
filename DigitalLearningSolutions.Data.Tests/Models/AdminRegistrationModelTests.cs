namespace DigitalLearningSolutions.Data.Tests.Models
{
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
    }
}
