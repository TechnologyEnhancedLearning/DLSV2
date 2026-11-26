namespace DigitalLearningSolutions.Api.Authentication
{
    using System.Linq;
    using System.Security.Claims;

    /// <summary>
    /// Helpers for reading Learning Hub specific values from the claims in a validated bearer token.
    /// </summary>
    public static class LearningHubClaimsHelper
    {
        /// <summary>
        /// The claim type that carries the Learning Hub auth ID.
        /// This is the same claim type the Web project reads in Startup.OnTicketReceived.
        /// The JWT bearer handler maps the standard "sub" claim onto this type by default.
        /// </summary>
        public const string LearningHubAuthIdClaimType = ClaimTypes.NameIdentifier;

        /// <summary>
        /// The raw OpenID Connect subject claim. Used as a fallback in case inbound claim mapping is turned off.
        /// </summary>
        public const string SubjectClaimType = "sub";

        /// <summary>
        /// Gets the Learning Hub auth ID from the authenticated user's claims.
        /// </summary>
        /// <param name="user">The principal built from the validated bearer token.</param>
        /// <returns>The Learning Hub auth ID, or null if the claim is missing or is not an integer.</returns>
        public static int? GetLearningHubAuthId(this ClaimsPrincipal? user)
        {
            if (user == null)
            {
                return null;
            }

            // Prefer the mapped claim type (matches the Web project), then fall back to the raw "sub" claim.
            var claim = user.Claims.FirstOrDefault(c => c.Type == LearningHubAuthIdClaimType)
                        ?? user.Claims.FirstOrDefault(c => c.Type == SubjectClaimType);

            return int.TryParse(claim?.Value, out var learningHubAuthId) ? learningHubAuthId : (int?)null;
        }
    }
}
