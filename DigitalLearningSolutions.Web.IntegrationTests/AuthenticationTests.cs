namespace DigitalLearningSolutions.Web.IntegrationTests
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Xunit;

    public class AuthenticationTests: IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public AuthenticationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/Home/Section/Welcome")]
        [InlineData("/Home/Section/Products")]
        [InlineData("/Home/Section/LearningContent")]
        [InlineData("/Login")]
        [InlineData("/ForgotPassword")]
        [InlineData("/LearningSolutions/AccessibilityHelp")]
        [InlineData("/LearningSolutions/Terms")]
        public async Task EndpointIsUnauthenticated(string url)
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions {
                AllowAutoRedirect = false
            });

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task BaseUrlRedirectsToWelcomePage()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            response.RequestMessage.RequestUri.LocalPath.Should().Be("/Home/Section/Welcome");
        }
    }
}
