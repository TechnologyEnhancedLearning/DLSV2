namespace DigitalLearningSolutions.Web.IntegrationTests
{
    using System.Net;

    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Xunit;

    public class AuthenticationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public AuthenticationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/Home")]
        [InlineData("/Login")]
        [InlineData("/ForgotPassword")]
        [InlineData("/Register")]
        [InlineData("/LearningSolutions/AccessibilityHelp")]
        [InlineData("/LearningSolutions/Terms")]
        public async Task EndpointIsUnauthenticated(string url)
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("/Login")]
        [InlineData("/ForgotPassword")]
        [InlineData("/Register")]
        public async Task EndpointRedirectsToHomeIfAuthenticated(string url)
        {
            // Arrange
            var client = _factory.CreateClientWithTestAuth();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }
    }
}
