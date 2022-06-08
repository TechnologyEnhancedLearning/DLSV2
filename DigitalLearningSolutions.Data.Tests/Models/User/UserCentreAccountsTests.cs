namespace DigitalLearningSolutions.Data.Tests.Models.User
{
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class UserCentreAccountsTests
    {
        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        [TestCase(null, true, true)]
        [TestCase(null, false, false)]
        [TestCase(true, null, true)]
        [TestCase(false, null, false)]
        [TestCase(null, null, false)]
        public void IsActive_returns_expected_value(bool? activeAdmin, bool? activeDelegate, bool expectedResult)
        {
            // When
            var result = new UserCentreAccounts(
                2,
                activeAdmin == null ? null : UserTestHelper.GetDefaultAdminAccount(active: activeAdmin.Value),
                activeDelegate == null ? null : UserTestHelper.GetDefaultDelegateAccount(active: activeDelegate.Value)
            );

            // Then
            result.IsActive.Should().Be(expectedResult);
        }
    }
}
