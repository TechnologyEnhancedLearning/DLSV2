namespace DigitalLearningSolutions.Web.API
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using Microsoft.Extensions.Configuration;

    public static class DlsApiAuthorization
    {
        public const string RequiredScopePolicy = "DlsApiScope";
        private const string MappedEntraScopeClaimType = "http://schemas.microsoft.com/identity/claims/scope";

        public static bool HasRequiredScope(ClaimsPrincipal user, string? requiredScope)
        {
            if (string.IsNullOrWhiteSpace(requiredScope))
            {
                return false;
            }

            return user.Claims
                .Where(claim =>
                    claim.Type == "scp" ||
                    claim.Type == "scope" ||
                    claim.Type == MappedEntraScopeClaimType
                )
                .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .Any(scope => string.Equals(scope, requiredScope, StringComparison.Ordinal));
        }
    }

    public static class DlsApiAuthentication
    {
        public static string GetAuthority(IConfiguration configuration)
        {
            var instance = configuration["DlsApiAuthentication:Instance"]?.TrimEnd('/');
            var tenantId = configuration["DlsApiAuthentication:TenantId"];
            return string.IsNullOrWhiteSpace(instance) || string.IsNullOrWhiteSpace(tenantId)
                ? string.Empty
                : $"{instance}/{tenantId}/v2.0";
        }
    }
}
