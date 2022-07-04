namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using DigitalLearningSolutions.Data.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class CentreEmailHelperTests
    {
        [Test]
        [TestCase(null, "primary@test.com", "primary@test.com")]
        [TestCase("centre@test.com", "primary@test.com", "centre@test.com")]
        public void GetEmailForCentreNotifications_returns_expected_value(string? centreEmail, string primaryEmail, string expectedResult)
        {
            // When
            var result = CentreEmailHelper.GetEmailForCentreNotifications(primaryEmail, centreEmail);

            // Then
            result.Should().Be(expectedResult);
        }
    }
}
