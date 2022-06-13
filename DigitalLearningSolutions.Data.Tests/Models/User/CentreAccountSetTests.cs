namespace DigitalLearningSolutions.Data.Tests.Models.User
{
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class CentreAccountSetTests
    {
        [Test]
        // Note: the value of approvedDelegate has no effect when activeDelegate is null
        [TestCase(true, true, true, true)]
        [TestCase(true, false, true, true)]
        [TestCase(true, true, false, true)]
        [TestCase(true, false, false, true)]
        [TestCase(true, null, false, true)]
        [TestCase(false, true, true, true)]
        [TestCase(false, false, true, false)]
        [TestCase(false, true, false, false)]
        [TestCase(false, false, false, false)]
        [TestCase(false, null, false, false)]
        [TestCase(null, true, true, true)]
        [TestCase(null, false, true, false)]
        [TestCase(null, true, false, false)]
        [TestCase(null, false, false, false)]
        [TestCase(null, null, false, false)]
        public void CanLogInToCentre_returns_expected_value_when_centre_is_active(
            bool? activeAdmin,
            bool? activeDelegate,
            bool approvedDelegate,
            bool expectedResult
        )
        {
            // When
            var result = new CentreAccountSet(
                2,
                activeAdmin == null ? null : UserTestHelper.GetDefaultAdminAccount(active: activeAdmin.Value),
                activeDelegate == null
                    ? null
                    : UserTestHelper.GetDefaultDelegateAccount(active: activeDelegate.Value, approved: approvedDelegate)
            );

            // Then
            result.CanLogInToCentre.Should().Be(expectedResult);
        }

        [Test]
        // Note: the value of approvedDelegate has no effect when activeDelegate is null
        [TestCase(true, true, true, true)]
        [TestCase(true, false, true, false)]
        [TestCase(true, true, false, false)]
        [TestCase(true, false, false, false)]
        [TestCase(true, null, false, true)]
        [TestCase(false, true, true, false)]
        [TestCase(false, false, true, false)]
        [TestCase(false, true, false, false)]
        [TestCase(false, false, false, false)]
        [TestCase(false, null, false, false)]
        [TestCase(null, true, true, true)]
        [TestCase(null, false, true, false)]
        [TestCase(null, true, false, false)]
        [TestCase(null, false, false, false)]
        [TestCase(null, null, false, false)]
        public void CanLogDirectlyInToCentre_returns_expected_value_when_centre_is_active(
            bool? activeAdmin,
            bool? activeDelegate,
            bool approvedDelegate,
            bool expectedResult
        )
        {
            // When
            var result = new CentreAccountSet(
                2,
                activeAdmin == null ? null : UserTestHelper.GetDefaultAdminAccount(active: activeAdmin.Value),
                activeDelegate == null
                    ? null
                    : UserTestHelper.GetDefaultDelegateAccount(active: activeDelegate.Value, approved: approvedDelegate)
            );

            // Then
            result.CanLogDirectlyInToCentre.Should().Be(expectedResult);
        }
    }
}
