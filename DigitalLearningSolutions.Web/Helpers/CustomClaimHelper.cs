﻿namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Security.Claims;

    public static class CustomClaimHelper
    {
        public static int? GetAdminId(this ClaimsPrincipal user)
        {
            return user.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId);
        }

        public static int? GetCandidateId(this ClaimsPrincipal user)
        {
            var id = user.GetCustomClaimAsInt(CustomClaimTypes.LearnCandidateId);
            return id == 0 ? null : id;
        }

        public static int GetCandidateIdKnownNotNull(this ClaimsPrincipal user)
        {
            return user.GetCustomClaimAsRequiredInt(CustomClaimTypes.LearnCandidateId);
        }

        public static int GetCentreId(this ClaimsPrincipal user)
        {
            return user.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserCentreId);
        }

        public static string? GetCustomClaim(this ClaimsPrincipal user, string customClaimType)
        {
            return user.FindFirst(customClaimType)?.Value;
        }
        public static string? GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email).Value;
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

        public static bool? GetCustomClaimAsBool(this ClaimsPrincipal user, string customClaimType)
        {
            var customClaimString = user.GetCustomClaim(customClaimType);
            if (customClaimString == null) return null;
            try
            {
                return bool.Parse(customClaimString);
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

        public static bool IsDelegateOnlyAccount(this ClaimsPrincipal user)
        {
            return user.GetAdminId() == null
                   && (user.GetCustomClaimAsBool(CustomClaimTypes.LearnUserAuthenticated) ?? false);
        }

        public static bool HasCentreAdminPermissions(this ClaimsPrincipal user)
        {
            return (user.GetCustomClaimAsBool(CustomClaimTypes.UserCentreAdmin) ?? false) ||
                   (user.GetCustomClaimAsBool(CustomClaimTypes.UserCentreManager) ?? false) ||
                   (user.GetCustomClaimAsBool(CustomClaimTypes.UserUserAdmin) ?? false);
        }

        public static bool HasCentreManagerPermissions(this ClaimsPrincipal user)
        {
            return (user.GetCustomClaimAsBool(CustomClaimTypes.UserCentreManager) ?? false) ||
                   (user.GetCustomClaimAsBool(CustomClaimTypes.UserUserAdmin) ?? false);
        }
    }
}
