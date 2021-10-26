namespace DigitalLearningSolutions.Web.Helpers
{
    using Microsoft.AspNetCore.Authorization;

    public class CustomPolicies
    {
        public const string UserOnly = "UserOnly";
        public const string UserCentreAdmin = "UserCentreAdmin";
        public const string UserFrameworksAdminOnly = "UserFrameworksAdminOnly";
        public const string UserCentreManager = "UserCentreManager";
        public const string UserSupervisor = "UserSupervisor";
        public const string UserCentreAdminOrFrameworksAdmin = "UserCentreAdminOrFrameworksAdmin";
        public const string UserSuperAdmin = "UserSuperAdmin";

        public static AuthorizationPolicyBuilder ConfigurePolicyUserOnly(AuthorizationPolicyBuilder policy)
        {
            return policy.RequireAssertion(
                context => context.User.GetCandidateId() != null
                           && context.User.GetCustomClaimAsBool(CustomClaimTypes.LearnUserAuthenticated) == true
            );
        }

        public static AuthorizationPolicyBuilder ConfigurePolicyUserCentreAdmin(AuthorizationPolicyBuilder policy)
        {
            return policy.RequireAssertion(
                context => context.User.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId) != null
                           && context.User.HasCentreAdminPermissions()
            );
        }

        public static AuthorizationPolicyBuilder ConfigurePolicyUserFrameworksAdminOnly(
            AuthorizationPolicyBuilder policy
        )
        {
            return policy.RequireAssertion(
                context => context.User.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId) != null
                           && (context.User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkDeveloper) == true) |
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
                context => context.User.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId) != null &&
                           context.User.HasCentreManagerPermissions()
            );
        }

        public static AuthorizationPolicyBuilder ConfigurePolicyUserCentreAdminOrFrameworksAdmin(
            AuthorizationPolicyBuilder policy
        )
        {
            return policy.RequireAssertion(
                context => context.User.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId) != null
                           && (context.User.HasCentreAdminPermissions()
                               || context.User.HasFrameworksAdminPermissions())
            );
        }

        public static AuthorizationPolicyBuilder ConfigurePolicyUserSupervisor(AuthorizationPolicyBuilder policy)
        {
            return policy.RequireAssertion(
                context => context.User.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId) != null
                           && context.User.GetCustomClaimAsBool(CustomClaimTypes.IsSupervisor) == true
            );
        }

        public static AuthorizationPolicyBuilder ConfigurePolicyUserSuperAdmin(AuthorizationPolicyBuilder policy)
        {
            return policy.RequireAssertion(
                context => context.User.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId) != null &&
                           context.User.HasSuperAdminPermissions()
            );
        }
    }
}
