namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Models.User;

    public static class CustomClaimHelper
    {
        public static int? GetAdminId(this ClaimsPrincipal user)
        {
            return user.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId);
        }

        public static int GetAdminIdKnownNotNull(this ClaimsPrincipal user)
        {
            return (int)user.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId)!;
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

        /// <summary>
        ///     Returns the Admin Category ID or null if the ID is non-existent
        ///     Also returns null if ID is zero to match the data service convention of not filtering on NULL category filter
        /// </summary>
        public static int? GetAdminCourseCategoryFilter(this ClaimsPrincipal user)
        {
            var categoryId = user.GetCustomClaimAsInt(CustomClaimTypes.AdminCategoryId);
            return categoryId == 0 ? null : categoryId;
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
            return user.GetCustomClaimAsBool(CustomClaimTypes.IsSupervisor) == true;
        }

        public static string GetCandidateNumberKnownNotNull(this ClaimsPrincipal user)
        {
            return user.GetCustomClaim(CustomClaimTypes.LearnCandidateNumber)!;
        }
    }
}
