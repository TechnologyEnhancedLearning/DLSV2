namespace DigitalLearningSolutions.Api.Tests.Authentication
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Api.Authentication;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Authorization.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using NUnit.Framework;

    public class LearningHubAuthenticationExtensionsTests
    {
        private const string Authority = "https://auth.learninghub.example/";

        [Test]
        public void ConfigureJwtBearerOptions_uses_learning_hub_authority()
        {
            // Given
            var options = new JwtBearerOptions();

            // When
            LearningHubAuthenticationExtensions.ConfigureJwtBearerOptions(options, BuildConfig(audience: null));

            // Then
            options.Authority.Should().Be(Authority);
            options.RequireHttpsMetadata.Should().BeTrue();
            options.MapInboundClaims.Should().BeTrue();
            options.TokenValidationParameters.ValidateIssuer.Should().BeTrue();
            options.TokenValidationParameters.ValidateIssuerSigningKey.Should().BeTrue();
            options.TokenValidationParameters.ValidateLifetime.Should().BeTrue();
        }

        [Test]
        public void ConfigureJwtBearerOptions_validates_audience_when_configured()
        {
            // Given
            var options = new JwtBearerOptions();

            // When
            LearningHubAuthenticationExtensions.ConfigureJwtBearerOptions(options, BuildConfig("dls-api"));

            // Then
            options.Audience.Should().Be("dls-api");
            options.TokenValidationParameters.ValidateAudience.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ConfigureJwtBearerOptions_skips_audience_check_when_not_configured(string? audience)
        {
            // Given
            var options = new JwtBearerOptions();

            // When
            LearningHubAuthenticationExtensions.ConfigureJwtBearerOptions(options, BuildConfig(audience));

            // Then
            options.Audience.Should().BeNull();
            options.TokenValidationParameters.ValidateAudience.Should().BeFalse();
        }

        [Test]
        public async Task AddLearningHubBearerAuthentication_makes_jwt_bearer_the_default_scheme()
        {
            // Given
            var provider = BuildServiceProvider();

            // When
            var schemeProvider = provider.GetRequiredService<IAuthenticationSchemeProvider>();
            var defaultScheme = await schemeProvider.GetDefaultAuthenticateSchemeAsync();
            var challengeScheme = await schemeProvider.GetDefaultChallengeSchemeAsync();

            // Then
            defaultScheme!.Name.Should().Be(JwtBearerDefaults.AuthenticationScheme);
            challengeScheme!.Name.Should().Be(JwtBearerDefaults.AuthenticationScheme);
        }

        [Test]
        public void AddLearningHubBearerAuthentication_requires_authenticated_user_by_default()
        {
            // Given
            var provider = BuildServiceProvider();

            // When
            var fallbackPolicy = provider.GetRequiredService<IOptions<AuthorizationOptions>>().Value.FallbackPolicy;

            // Then
            fallbackPolicy.Should().NotBeNull();
            fallbackPolicy!.AuthenticationSchemes.Should().BeEquivalentTo(JwtBearerDefaults.AuthenticationScheme);
            fallbackPolicy.Requirements.OfType<DenyAnonymousAuthorizationRequirement>().Should().HaveCount(1);
        }

        private static ServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddLearningHubBearerAuthentication(BuildConfig(audience: null));
            return services.BuildServiceProvider();
        }

        private static IConfiguration BuildConfig(string? audience)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        { "LearningHubAuthentication:Authority", Authority },
                        { LearningHubAuthenticationExtensions.ApiAudienceConfigKey, audience },
                    }
                )
                .Build();
        }
    }
}
