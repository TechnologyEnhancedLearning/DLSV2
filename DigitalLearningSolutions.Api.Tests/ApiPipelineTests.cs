namespace DigitalLearningSolutions.Api.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.JsonWebTokens;
    using Microsoft.IdentityModel.Protocols;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Microsoft.IdentityModel.Tokens;
    using NUnit.Framework;

    /// <summary>
    /// Runs the real API pipeline in memory (authentication, authorization, routing, versioning and Swagger).
    /// </summary>
    /// <remarks>
    /// The Learning Hub auth service can't be reached from a unit test, so the JWT bearer handler is given a
    /// local issuer and signing key instead of downloading them from the Authority. Everything else
    /// (claim mapping, [Authorize], fallback policy, controller logic) is the production code.
    /// Database access is replaced with fakes.
    /// </remarks>
    public class ApiPipelineTests
    {
        private const string TestIssuer = "https://test-learninghub-auth.example";
        private const string SelfAssessmentUrl = "/api/v1/SelfAssessment/7";
        private const int LearningHubAuthId = 1234;
        private const int DelegateUserId = 56;

        private static readonly SymmetricSecurityKey SigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes("unit-test-signing-key-that-is-at-least-256-bits-long"));

        private WebApplicationFactory<Program> factory = null!;
        private HttpClient client = null!;
        private IUserDataService userDataService = null!;
        private ISelfAssessmentDataService selfAssessmentDataService = null!;

        [SetUp]
        public void SetUp()
        {
            userDataService = A.Fake<IUserDataService>();
            selfAssessmentDataService = A.Fake<ISelfAssessmentDataService>();

            factory = new WebApplicationFactory<Program>().WithWebHostBuilder(
                builder => builder.ConfigureTestServices(
                    services =>
                    {
                        services.AddScoped(_ => userDataService);
                        services.AddScoped(_ => selfAssessmentDataService);

                        // Trust tokens signed by the local test key instead of the real Learning Hub keys.
                        services.PostConfigure<JwtBearerOptions>(
                            JwtBearerDefaults.AuthenticationScheme,
                            options =>
                            {
                                var configuration = new OpenIdConnectConfiguration { Issuer = TestIssuer };
                                configuration.SigningKeys.Add(SigningKey);
                                options.Configuration = configuration;
                                options.ConfigurationManager =
                                    new StaticConfigurationManager<OpenIdConnectConfiguration>(configuration);
                                options.TokenValidationParameters.ValidIssuer = TestIssuer;
                                options.TokenValidationParameters.IssuerSigningKey = SigningKey;
                            }
                        );
                    }
                )
            );

            client = factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            client.Dispose();
            factory.Dispose();
        }

        [Test]
        public async Task GetSelfAssessment_without_token_returns_401()
        {
            // When
            var response = await client.GetAsync(SelfAssessmentUrl);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            A.CallTo(() => selfAssessmentDataService.GetSelfAssessmentForCandidateById(A<int>._, A<int>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task GetSelfAssessment_with_malformed_token_returns_401()
        {
            // Given
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "not-a-real-token");

            // When
            var response = await client.GetAsync(SelfAssessmentUrl);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task GetSelfAssessment_with_token_signed_by_unknown_key_returns_401()
        {
            // Given
            var otherKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("a-different-key-not-trusted-by-the-api-xxxxxxxx"));
            UseToken(CreateToken(otherKey, DateTime.UtcNow.AddMinutes(10)));

            // When
            var response = await client.GetAsync(SelfAssessmentUrl);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task GetSelfAssessment_with_expired_token_returns_401()
        {
            // Given
            UseToken(CreateToken(SigningKey, DateTime.UtcNow.AddMinutes(-10)));

            // When
            var response = await client.GetAsync(SelfAssessmentUrl);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task GetSelfAssessment_with_valid_token_returns_self_assessment_json()
        {
            // Given
            UseToken(CreateToken(SigningKey, DateTime.UtcNow.AddMinutes(10)));
            A.CallTo(() => userDataService.GetUserIdFromLearningHubAuthId(LearningHubAuthId)).Returns(DelegateUserId);
            A.CallTo(() => selfAssessmentDataService.GetSelfAssessmentForCandidateById(DelegateUserId, 7))
                .Returns(new CurrentSelfAssessment { Id = 7, Name = "Test assessment" });

            // When
            var response = await client.GetAsync(SelfAssessmentUrl);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            json.RootElement.GetProperty("id").GetInt32().Should().Be(7);
            json.RootElement.GetProperty("name").GetString().Should().Be("Test assessment");
        }

        [Test]
        public async Task GetSelfAssessment_with_valid_token_for_unlinked_user_returns_403()
        {
            // Given
            UseToken(CreateToken(SigningKey, DateTime.UtcNow.AddMinutes(10)));
            A.CallTo(() => userDataService.GetUserIdFromLearningHubAuthId(LearningHubAuthId)).Returns(null);

            // When
            var response = await client.GetAsync(SelfAssessmentUrl);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task Swagger_v1_document_describes_versioned_self_assessment_endpoint_and_bearer_auth()
        {
            // When
            var response = await client.GetAsync("/swagger/v1/swagger.json");

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var root = json.RootElement;

            root.GetProperty("info").GetProperty("version").GetString().Should().Be("1.0");

            var operation = root.GetProperty("paths")
                .GetProperty("/api/v1/SelfAssessment/{selfAssessmentId}")
                .GetProperty("get");
            operation.GetProperty("summary").GetString().Should().Be("Gets a self assessment for a delegate.");
            operation.GetProperty("responses").TryGetProperty("200", out _).Should().BeTrue();
            operation.GetProperty("responses").TryGetProperty("404", out _).Should().BeTrue();

            var bearer = root.GetProperty("components").GetProperty("securitySchemes").GetProperty("Bearer");
            bearer.GetProperty("scheme").GetString().Should().Be("bearer");
        }

        private void UseToken(string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Creates a JWT shaped like a Learning Hub access token: the Learning Hub auth ID is in the "sub" claim.
        /// </summary>
        private static string CreateToken(SecurityKey key, DateTime expires)
        {
            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = TestIssuer,
                Claims = new Dictionary<string, object> { { "sub", LearningHubAuthId.ToString() } },
                NotBefore = expires.AddMinutes(-30),
                IssuedAt = expires.AddMinutes(-30),
                Expires = expires,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
            };

            return new JsonWebTokenHandler().CreateToken(descriptor);
        }
    }
}
