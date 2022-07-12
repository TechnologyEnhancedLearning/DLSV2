namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using DigitalLearningSolutions.Data.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class PrnStringTests
    {
        [Test]
        public void GetPrnDisplayString_returns_the_prn_when_the_delegate_has_been_prompted_and_has_provided_a_prn()
        {
            // Given
            const string? prn = "12345";

            // When
            var result = PrnHelper.GetPrnDisplayString(true, prn);

            // Then
            result.Should().Be(prn);
        }

        [Test]
        public void
            GetPrnDisplayString_returns_Not_professionally_registered_when_the_delegate_has_been_prompted_and_has_not_provided_a_prn()
        {
            // When
            var result = PrnHelper.GetPrnDisplayString(true, null);

            // Then
            result.Should().Be("Not professionally registered");
        }

        [Test]
        public void GetPrnDisplayString_returns_Not_yet_provided_when_the_delegate_has_not_been_prompted_for_a_prn()
        {
            // When
            var result = PrnHelper.GetPrnDisplayString(false, null);

            // Then
            result.Should().Be("Not yet provided");
        }
    }
}
