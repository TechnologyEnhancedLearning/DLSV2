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
        public void HasMoreThanDelegateAccess_should_be_true_if_user_admin()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.UserAdminId, "1"),
            }, "mock"));

            // When
            var result = user.HasMoreThanDelegateAccess();

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void HasMoreThanDelegateAccess_should_be_true_if_user_authenticated()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.LearnUserAuthenticated, "True"),
            }, "mock"));

            // When
            var result = user.HasMoreThanDelegateAccess();

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void HasMoreThanDelegateAccess_should_be_true_if_user_authenticated_not_capitalized()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.LearnUserAuthenticated, "true"),
            }, "mock"));

            // When
            var result = user.HasMoreThanDelegateAccess();

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void HasMoreThanDelegateAccess_should_be_false_if_user_admin_id_zero()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.UserAdminId, "0"),
            }, "mock"));

            // When
            var result = user.HasMoreThanDelegateAccess();

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void HasMoreThanDelegateAccess_should_be_false_if_user_admin_id_not_valid()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.UserAdminId, "test"),
            }, "mock"));

            // When
            var result = user.HasMoreThanDelegateAccess();

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void HasMoreThanDelegateAccess_should_be_false_if_user_not_authenticated()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.LearnUserAuthenticated, "false"),
            }, "mock"));

            // When
            var result = user.HasMoreThanDelegateAccess();

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void HasMoreThanDelegateAccess_should_be_false_if_user_authenticated_invalid()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.LearnUserAuthenticated, "test"),
            }, "mock"));

            // When
            var result = user.HasMoreThanDelegateAccess();

            // Then
            result.Should().BeFalse();
        }

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
        public void GetCustomClaimAsNotNullableInt_parses_int()
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
        public void GetCustomClaimAsNotNullableInt_handles_claim_missing()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            // Then
            Assert.Throws<ArgumentNullException>(() => user.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserCentreId));
        }

        [Test]
        public void GetCustomClaimAsNotNullableInt_handles_claim_invalid()
        {
            // Given
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.LearnCandidateId, "test"),
            }, "mock"));

            // Then
            Assert.Throws<FormatException>(() => user.GetCustomClaimAsRequiredInt(CustomClaimTypes.LearnCandidateId));
        }
    }
}
