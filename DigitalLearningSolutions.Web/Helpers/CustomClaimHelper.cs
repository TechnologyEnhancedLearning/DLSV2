namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Security.Claims;

    public static class CustomClaimHelper
    {
        public static bool HasMoreThanDelegateAccess(this ClaimsPrincipal user)
        {
            var isAdmin = user.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId) > 0;
            return user.HasClaim(CustomClaimTypes.LearnUserAuthenticated, "True")
                   || user.HasClaim(CustomClaimTypes.LearnUserAuthenticated, "true")
                   || isAdmin;
        }

        public static string? GetCustomClaim(this ClaimsPrincipal user, string customClaimType)
        {
            return user.FindFirst(customClaimType)?.Value;
        }

        public static int? GetCustomClaimAsInt(this ClaimsPrincipal user, string customClaimType)
        {
            var customClaimString = user.GetCustomClaim(customClaimType);
            if (customClaimString == null) return null;
            try
            {
                return int.Parse(customClaimString);
            }
            catch
            {
                return null;
            }
        }

        // Should only be used for claims we know not be null from the authorization policy
        public static int GetCustomClaimAsRequiredInt(this ClaimsPrincipal user, string customClaimType)
        {
            var customClaimString = user.GetCustomClaim(customClaimType);
            return int.Parse(customClaimString);
        }
    }
}
