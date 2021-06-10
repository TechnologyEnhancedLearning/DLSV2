namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System;
    using System.Security.Claims;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class CustomClaimHelperTests
    {
        [Test]
        public void GetCustomClaimAsInt_parses_int()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.UserCentreId, "50"),
            }, "mock"));

            // When
            var result = user.GetCustomClaimAsInt(CustomClaimTypes.UserCentreId);

            // Then
            result.Should().Be(50);
        }

        [Test]
        public void GetCustomClaimAsInt_handles_claim_missing()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            // When
            var result = user.GetCustomClaimAsInt(CustomClaimTypes.UserCentreId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetCustomClaimAsInt_handles_claim_invalid()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.AdminCategoryId, "test"),
            }, "mock"));

            // When
            var result = user.GetCustomClaimAsInt(CustomClaimTypes.AdminCategoryId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetCustomClaimAsBool_parses_lowercase_bool()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.LearnUserAuthenticated, "true"),
            }, "mock"));

            // When
            var result = user.GetCustomClaimAsBool(CustomClaimTypes.LearnUserAuthenticated);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void GetCustomClaimAsBool_parses_uppercase_bool()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.LearnUserAuthenticated, "FALSE"),
            }, "mock"));

            // When
            var result = user.GetCustomClaimAsBool(CustomClaimTypes.LearnUserAuthenticated);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void GetCustomClaimAsBool_handles_claim_missing()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            // When
            var result = user.GetCustomClaimAsBool(CustomClaimTypes.LearnUserAuthenticated);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetCustomClaimAsBool_handles_claim_invalid()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.LearnUserAuthenticated, "test"),
            }, "mock"));

            // When
            var result = user.GetCustomClaimAsBool(CustomClaimTypes.LearnUserAuthenticated);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetCustomClaimAsRequiredInt_parses_int()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.LearnCandidateId, "50"),
            }, "mock"));

            // When
            var result = user.GetCustomClaimAsRequiredInt(CustomClaimTypes.LearnCandidateId);

            // Then
            result.Should().Be(50);
        }

        [Test]
        public void GetCustomClaimAsRequiredInt_handles_claim_missing()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            // Then
            Assert.Throws<ArgumentNullException>(() => user.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserCentreId));
        }

        [Test]
        public void GetCustomClaimAsRequiredInt_handles_claim_invalid()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.LearnCandidateId, "test"),
            }, "mock"));

            // Then
            Assert.Throws<FormatException>(() => user.GetCustomClaimAsRequiredInt(CustomClaimTypes.LearnCandidateId));
        }

        [Test]
        public void GetCandidateIdHandlesZeroAsNull()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.LearnCandidateId, "0"),
            }, "mock"));

            // When
            var result = user.GetCandidateId();

            // Then
            result.Should().BeNull();
        }
    }
}
