namespace DigitalLearningSolutions.Web.IntegrationTests.AuthenticatedTests
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Web.IntegrationTests.TestHelpers;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class OnRedirectToAccessDeniedTests : IClassFixture<AuthenticationWebApplicationFactory<Startup>>
    {
        private readonly AuthenticationWebApplicationFactory<Startup> _factory;

        public OnRedirectToAccessDeniedTests(AuthenticationWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task redirect_to_PleaseLogout_occurs_when_user_is_authenticated_without_UserId_claim()
        {
            // Given
            using var scope = _factory.Services.CreateScope();

            var client = await HttpClientHelper.SetDelegateSessionWithoutUserIdClaimAndGetClient(_factory, 1);

            // When
            var response = await client.GetAsync("/SuperAdmin/Admins");

            // Then
            response.Headers.Location.AbsolutePath.Should().Be("/PleaseLogout");
        }

        [Fact]
        public async Task redirect_to_AccessDenied_occurs_when_user_is_authenticated_without_sufficient_privileges()
        {
            // Given
            using var scope = _factory.Services.CreateScope();

            var client = await HttpClientHelper.SetDelegateSessionAndGetClient(_factory, 1);

            // When
            var response = await client.GetAsync("/SuperAdmin/Admins");

            // Then
            response.Headers.Location.AbsolutePath.Should().Be("/AccessDenied");
        }
    }
}
