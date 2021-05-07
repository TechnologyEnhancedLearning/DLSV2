namespace DigitalLearningSolutions.Web.Helpers
{
    using Microsoft.AspNetCore.Authorization;

    public class CustomPolicies
    {
        public const string UserOnly = "UserOnly";
        public const string UserCentreAdminOnly = "UserCentreAdminOnly";

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
                           && context.User.GetCustomClaimAsBool(CustomClaimTypes.UserCentreAdmin) == true);
        }
    }
}
