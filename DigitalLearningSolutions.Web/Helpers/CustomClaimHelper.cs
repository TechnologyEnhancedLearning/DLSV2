namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Security.Claims;

    public static class CustomClaimHelper
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var id = user.GetCustomClaimAsInt(CustomClaimTypes.UserId);
            return id == 0 ? null : id;
        }

        public static int GetUserIdKnownNotNull(this ClaimsPrincipal user)
        {
            return user.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserId);
        }

        public static bool IsMissingUserId(this ClaimsPrincipal user)
        {
            return user.Identity.IsAuthenticated && user.GetUserId() == null;
        }

        public static int? GetAdminId(this ClaimsPrincipal user)
        {
            return user.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId);
        }

        public static int GetAdminIdKnownNotNull(this ClaimsPrincipal user)
        {
            return user.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserAdminId);
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

        public static int? GetCentreId(this ClaimsPrincipal user)
        {
            return user.GetCustomClaimAsInt(CustomClaimTypes.UserCentreId);
        }

        public static int GetCentreIdKnownNotNull(this ClaimsPrincipal user)
        {
            return user.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserCentreId);
        }

        public static int? GetAdminCategoryId(this ClaimsPrincipal user)
        {
            var adminCategory = user.GetCustomClaimAsRequiredInt(CustomClaimTypes.AdminCategoryId);
            return AdminCategoryHelper.AdminCategoryToCategoryId(adminCategory);
        }

        public static string? GetCustomClaim(this ClaimsPrincipal user, string customClaimType)
        {
            return user.FindFirst(customClaimType)?.Value;
        }

        public static string? GetUserPrimaryEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email).Value;
        }

        public static string GetUserPrimaryEmailKnownNotNull(this ClaimsPrincipal user)
        {
            return user.GetCustomClaimAsRequiredString(ClaimTypes.Email);
        }

        public static int? GetCustomClaimAsInt(this ClaimsPrincipal user, string customClaimType)
        {
            var customClaimString = user.GetCustomClaim(customClaimType);
            if (customClaimString == null)
            {
                return null;
            }

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
            if (customClaimString == null)
            {
                return null;
            }

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

        // Should only be used for claims we know not be null from the authorization policy
        public static string GetCustomClaimAsRequiredString(this ClaimsPrincipal user, string customClaimType)
        {
            return user.GetCustomClaim(customClaimType)!;
        }

        public static bool IsDelegateOnlyAccount(this ClaimsPrincipal user)
        {
            return user.GetAdminId() == null
                   && (user.GetCustomClaimAsBool(CustomClaimTypes.LearnUserAuthenticated) ?? false);
        }

        public static bool HasLearningPortalPermissions(this ClaimsPrincipal user)
        {
            return user.GetCustomClaimAsBool(CustomClaimTypes.LearnUserAuthenticated) ?? false;
        }

        public static bool HasCentreAdminPermissions(this ClaimsPrincipal user)
        {
            return (user.GetCustomClaimAsBool(CustomClaimTypes.UserCentreAdmin) ?? false) ||
                   user.HasCentreManagerPermissions();
        }

        public static bool HasCentreManagerPermissions(this ClaimsPrincipal user)
        {
            return (user.GetCustomClaimAsBool(CustomClaimTypes.UserCentreManager) ?? false) ||
                   user.HasSuperAdminPermissions();
        }

        public static bool HasSuperAdminPermissions(this ClaimsPrincipal user)
        {
            return user.GetCustomClaimAsBool(CustomClaimTypes.UserUserAdmin) ?? false;
        }

        public static bool HasFrameworksAdminPermissions(this ClaimsPrincipal user)
        {
            return user.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkDeveloper) == true ||
                   user.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkContributor) == true ||
                   user.GetCustomClaimAsBool(CustomClaimTypes.IsWorkforceManager) == true ||
                   user.GetCustomClaimAsBool(CustomClaimTypes.IsWorkforceContributor) == true;
        }

        public static bool HasSupervisorAdminPermissions(this ClaimsPrincipal user)
        {
            return user.GetCustomClaimAsBool(CustomClaimTypes.IsSupervisor) == true ||
                   user.GetCustomClaimAsBool(CustomClaimTypes.IsNominatedSupervisor) == true;
        }

        public static string GetCandidateNumberKnownNotNull(this ClaimsPrincipal user)
        {
            return user.GetCustomClaim(CustomClaimTypes.LearnCandidateNumber)!;
        }
    }
}
