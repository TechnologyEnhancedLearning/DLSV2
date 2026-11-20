namespace DigitalLearningSolutions.Api.Authentication
{
    using DigitalLearningSolutions.Data.Extensions;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Sets up authentication for the DLS API using the Learning Hub auth service.
    /// </summary>
    /// <remarks>
    /// The Web project signs users in through the Learning Hub auth service using OpenID Connect
    /// (see Startup.SetUpAuthentication in DigitalLearningSolutions.Web). The API uses the same
    /// auth service (same "LearningHubAuthentication:Authority" setting) but, because API callers
    /// send a token rather than a browser cookie, it validates the "Authorization: Bearer {token}"
    /// header with the JWT bearer handler. The handler downloads the Learning Hub auth service's
    /// OpenID Connect discovery document and signing keys from the Authority and uses them to check
    /// the token's signature, issuer, lifetime and (optionally) audience.
    /// No other identity provider is involved.
    /// </remarks>
    public static class LearningHubAuthenticationExtensions
    {
        /// <summary>
        /// Config key for the audience (API resource name) the token must be issued for.
        /// If empty, the audience is not checked.
        /// </summary>
        public const string ApiAudienceConfigKey = "LearningHubAuthentication:ApiAudience";

        /// <summary>
        /// Adds Learning Hub bearer token authentication and makes every endpoint require an authenticated user by default.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="config">The application configuration.</param>
        /// <returns>The service collection, for chaining.</returns>
        public static IServiceCollection AddLearningHubBearerAuthentication(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    JwtBearerDefaults.AuthenticationScheme,
                    options => ConfigureJwtBearerOptions(options, config)
                );

            // Secure by default: any endpoint without [AllowAnonymous] needs a valid Learning Hub bearer token.
            services.AddAuthorization(
                options =>
                {
                    options.FallbackPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build();
                }
            );

            return services;
        }

        /// <summary>
        /// Applies the Learning Hub settings to the JWT bearer options.
        /// Kept separate (and internal) so it can be unit tested.
        /// </summary>
        internal static void ConfigureJwtBearerOptions(JwtBearerOptions options, IConfiguration config)
        {
            // Same auth service the Web project uses for OpenID Connect sign in.
            options.Authority = config.GetLearningHubAuthenticationAuthority();

            // Tokens must only ever be fetched over HTTPS.
            options.RequireHttpsMetadata = true;

            // Keep the default inbound claim mapping so "sub" becomes ClaimTypes.NameIdentifier,
            // which is the claim type the Web project reads the Learning Hub auth ID from.
            options.MapInboundClaims = true;

            var audience = config[ApiAudienceConfigKey];
            var hasAudience = !string.IsNullOrWhiteSpace(audience);

            options.Audience = hasAudience ? audience : null;
            options.TokenValidationParameters.ValidateIssuer = true;
            options.TokenValidationParameters.ValidateIssuerSigningKey = true;
            options.TokenValidationParameters.ValidateLifetime = true;

            // Only check the audience when one is configured (see "LearningHubAuthentication:ApiAudience").
            options.TokenValidationParameters.ValidateAudience = hasAudience;
        }
    }
}
