namespace DigitalLearningSolutions.Api.Tests.Authentication
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Api.Authentication;
    using FluentAssertions;
    using NUnit.Framework;

    public class LearningHubClaimsHelperTests
    {
        [Test]
        public void GetLearningHubAuthId_returns_value_of_name_identifier_claim()
        {
            // Given
            var user = CreatePrincipal(new Claim(ClaimTypes.NameIdentifier, "123"));

            // When
            var result = user.GetLearningHubAuthId();

            // Then
            result.Should().Be(123);
        }

        [Test]
        public void GetLearningHubAuthId_falls_back_to_sub_claim()
        {
            // Given
            var user = CreatePrincipal(new Claim("sub", "456"));

            // When
            var result = user.GetLearningHubAuthId();

            // Then
            result.Should().Be(456);
        }

        [Test]
        public void GetLearningHubAuthId_prefers_name_identifier_over_sub()
        {
            // Given
            var user = CreatePrincipal(
                new Claim("sub", "456"),
                new Claim(ClaimTypes.NameIdentifier, "123")
            );

            // When
            var result = user.GetLearningHubAuthId();

            // Then
            result.Should().Be(123);
        }

        [Test]
        public void GetLearningHubAuthId_returns_null_when_claim_is_missing()
        {
            // Given
            var user = CreatePrincipal(new Claim(ClaimTypes.Email, "someone@example.com"));

            // When
            var result = user.GetLearningHubAuthId();

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetLearningHubAuthId_returns_null_when_claim_is_not_an_integer()
        {
            // Given
            var user = CreatePrincipal(new Claim(ClaimTypes.NameIdentifier, "not-a-number"));

            // When
            var result = user.GetLearningHubAuthId();

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetLearningHubAuthId_returns_null_for_null_principal()
        {
            // Given
            ClaimsPrincipal? user = null;

            // When
            var result = user.GetLearningHubAuthId();

            // Then
            result.Should().BeNull();
        }

        private static ClaimsPrincipal CreatePrincipal(params Claim[] claims)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        }
    }
}
