namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;

    public class CustomPolicies
    {
        public const string BasicUser = "BasicUser";
        public const string CentreUser = "CentreUser";
        public const string UserDelegateOnly = "UserDelegateOnly";
        public const string UserAdmin = "UserAdmin";
        public const string UserCentreAdmin = "UserCentreAdmin";
        public const string UserFrameworksAdminOnly = "UserFrameworksAdminOnly";
        public const string UserCentreManager = "UserCentreManager";
        public const string UserSupervisor = "UserSupervisor";
        public const string UserCentreAdminOrFrameworksAdmin = "UserCentreAdminOrFrameworksAdmin";
        public const string UserSuperAdmin = "UserSuperAdmin";

        public static AuthorizationPolicyBuilder ConfigurePolicyBasicUser(AuthorizationPolicyBuilder policy)
        {
            return policy.RequireAssertion(context => context.User.GetUserId() != null);
        }

        public static AuthorizationPolicyBuilder ConfigurePolicyCentreUser(AuthorizationPolicyBuilder policy)
        {
            return policy.RequireAssertion(
                context => context.User.GetUserId() != null && context.User.GetCentreId() != null
            );
        }

        public static AuthorizationPolicyBuilder ConfigurePolicyUserDelegateOnly(AuthorizationPolicyBuilder policy)
        {
            return policy.RequireAssertion(
                context => context.User.GetUserId() != null && context.User.GetCandidateId() != null &&
                           context.User.GetCustomClaimAsBool(CustomClaimTypes.LearnUserAuthenticated) == true
            );
        }

        public static AuthorizationPolicyBuilder ConfigurePolicyUserAdmin(AuthorizationPolicyBuilder policy)
        {
            return policy.RequireAssertion(context => UserIsAdmin(context.User));
        }

        public static AuthorizationPolicyBuilder ConfigurePolicyUserCentreAdmin(AuthorizationPolicyBuilder policy)
        {
            return policy.RequireAssertion(
                context => UserIsAdmin(context.User) &&
                           context.User.HasCentreAdminPermissions()
            );
        }

        public static AuthorizationPolicyBuilder ConfigurePolicyUserFrameworksAdminOnly(
            AuthorizationPolicyBuilder policy
        )
        {
            return policy.RequireAssertion(
                context => UserIsAdmin(context.User) &&
                           (context.User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkDeveloper) == true) |
                           (context.User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkContributor) == true) |
                           (context.User.GetCustomClaimAsBool(CustomClaimTypes.IsWorkforceManager) == true) |
                           (context.User.GetCustomClaimAsBool(CustomClaimTypes.IsWorkforceContributor) == true)
            );
        }

        public static AuthorizationPolicyBuilder ConfigurePolicyUserCentreManager(
            AuthorizationPolicyBuilder policy
        )
        {
            return policy.RequireAssertion(
                context => UserIsAdmin(context.User) &&
                           context.User.HasCentreManagerPermissions()
            );
        }

        public static AuthorizationPolicyBuilder ConfigurePolicyUserCentreAdminOrFrameworksAdmin(
            AuthorizationPolicyBuilder policy
        )
        {
            return policy.RequireAssertion(
                context => UserIsAdmin(context.User) &&
                           (context.User.HasCentreAdminPermissions() || context.User.HasFrameworksAdminPermissions())
            );
        }

        public static AuthorizationPolicyBuilder ConfigurePolicyUserSupervisor(AuthorizationPolicyBuilder policy)
        {
            return policy.RequireAssertion(
                context => UserIsAdmin(context.User) &&
                           (context.User.GetCustomClaimAsBool(CustomClaimTypes.IsSupervisor) == true ||
                            context.User.GetCustomClaimAsBool(CustomClaimTypes.IsNominatedSupervisor) == true)
            );
        }

        public static AuthorizationPolicyBuilder ConfigurePolicyUserSuperAdmin(AuthorizationPolicyBuilder policy)
        {
            return policy.RequireAssertion(
                context => UserIsAdmin(context.User) &&
                           context.User.HasSuperAdminPermissions()
            );
        }

        private static bool UserIsAdmin(ClaimsPrincipal user)
        {
            return user.GetUserId() != null && user.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId) != null;
        }
    }
}
