namespace DigitalLearningSolutions.Web.Tests.API
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Web.API;
    using FluentAssertions;
    using NUnit.Framework;

    public class DlsApiAuthorizationTests
    {
        [Test]
        public void HasRequiredScope_accepts_an_Entra_scp_claim_containing_the_required_scope()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("scp", "profile dls.api") }));

            DlsApiAuthorization.HasRequiredScope(principal, "dls.api").Should().BeTrue();
        }

        [Test]
        public void HasRequiredScope_accepts_the_default_ASPNET_Core_mapped_Entra_scope_claim()
        {
            var principal = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim("http://schemas.microsoft.com/identity/claims/scope", "dls.api"),
                })
            );

            DlsApiAuthorization.HasRequiredScope(principal, "dls.api").Should().BeTrue();
        }

        [Test]
        public void HasRequiredScope_rejects_a_valid_principal_without_the_required_scope()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("scp", "profile") }));

            DlsApiAuthorization.HasRequiredScope(principal, "dls.api").Should().BeFalse();
        }

        [Test]
        public void HasRequiredScope_rejects_the_full_scope_URI_when_Entra_provides_the_scope_name_claim()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("scp", "dls.api") }));

            DlsApiAuthorization.HasRequiredScope(
                principal,
                "api://91ae0044-f0a9-4aaf-b54f-8a0325bdd348/dls.api"
            ).Should().BeFalse();
        }

        [Test]
        public void HasRequiredScope_rejects_missing_configuration()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("scp", "dls.api") }));

            DlsApiAuthorization.HasRequiredScope(principal, null).Should().BeFalse();
        }
    }
}
