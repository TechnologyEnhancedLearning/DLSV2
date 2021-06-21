namespace DigitalLearningSolutions.Web.Helpers
{
    using Microsoft.AspNetCore.Authorization;

    public class CustomPolicies
    {
        public const string UserOnly = "UserOnly";
        public const string UserCentreAdmin = "UserCentreAdmin";
        public const string UserFrameworksAdminOnly = "UserFrameworksAdminOnly";
        public const string UserCentreManager = "UserCentreManager";

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
                           (context.User.GetCustomClaimAsBool(CustomClaimTypes.UserCentreManager) == true ||
                            context.User.GetCustomClaimAsBool(CustomClaimTypes.UserUserAdmin) == true)
            );
        }
    }
}
