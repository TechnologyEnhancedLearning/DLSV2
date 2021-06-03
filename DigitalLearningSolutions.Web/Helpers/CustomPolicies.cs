namespace DigitalLearningSolutions.Web.Helpers
{
    using Microsoft.AspNetCore.Authorization;

    public class CustomPolicies
    {
        public const string UserOnly = "UserOnly";
        public const string UserCentreAdmin = "UserCentreAdminOnly";
        public const string UserFrameworksAdminOnly = "UserFrameworksAdminOnly";

        public static AuthorizationPolicyBuilder ConfigurePolicyUserOnly(AuthorizationPolicyBuilder policy)
        {
            return policy.RequireAssertion(
                context => context.User.GetCustomClaimAsInt(CustomClaimTypes.LearnCandidateId) != null
                           && context.User.GetCustomClaimAsBool(CustomClaimTypes.LearnUserAuthenticated) == true);
        }

        public static AuthorizationPolicyBuilder ConfigurePolicyUserCentreAdminOnly(AuthorizationPolicyBuilder policy)
        {
            return policy.RequireAssertion(
                context => context.User.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId) != null
                           && context.User.HasCentreAdminPermissions() == true);
        }

        public static AuthorizationPolicyBuilder ConfigurePolicyUserFrameworksAdminOnly(AuthorizationPolicyBuilder policy)
        {
            return policy.RequireAssertion(
                context => context.User.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId) != null
                           && (context.User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkDeveloper) == true | context.User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkContributor) == true));
        }
    }
}
