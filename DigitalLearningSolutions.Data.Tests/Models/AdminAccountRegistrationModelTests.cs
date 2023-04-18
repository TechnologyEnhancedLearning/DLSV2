namespace DigitalLearningSolutions.Data.Tests.Models
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class AdminAccountRegistrationModelTests
    {
        [Test]
        public void GetNotificationRoles_returns_full_list_with_all_roles()
        {
            // Given
            var expectedRoles = new List<int> { 1, 2, 3, 4, 6, 7, 8 };
            var adminRegistrationModel = new AdminAccountRegistrationModel(
                1,
                null,
                1,
                0,
                true,
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
        public void GetNotificationRoles_returns_1_2_for_new_centre_manager()
        {
            // Given
            var expectedRoles = new List<int> { 1, 2 };
            var adminRegistrationModel = RegistrationModelTestHelper.GetDefaultCentreManagerAccountRegistrationModel();

            // When
            var result = adminRegistrationModel.GetNotificationRoles();

            // Then
            result.Should().BeEquivalentTo(expectedRoles);
        }
    }
}
