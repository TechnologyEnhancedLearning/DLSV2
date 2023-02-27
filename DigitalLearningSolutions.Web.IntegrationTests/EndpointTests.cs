namespace DigitalLearningSolutions.Web.IntegrationTests
{
    using System.Threading.Tasks;
    using System.Net.Http;
    using AngleSharp.Io;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Xunit;
    using DocumentFormat.OpenXml.InkML;

    public class EndpointTests : IClassFixture<DefaultWebApplicationFactory<Startup>>
    {
        private readonly DefaultWebApplicationFactory<Startup> _factory;

        public EndpointTests(DefaultWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/Home/Welcome")]
        [InlineData("/Home/Products")]
        [InlineData("/Home/LearningContent")]
        [InlineData("/Login")]
        [InlineData("/ForgotPassword")]
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

            checkHeaderValue(response, "content-security-policy", "default-src 'self'; " +
                    "script-src 'self' 'sha256-+6WnXIl4mbFTCARd8N3COQmT3bJJmo32N8q8ZSQAIcU='; " +
                    "style-src 'self' 'unsafe-inline'; " +
                    "font-src https://assets.nhs.uk/; " +
                    "connect-src 'self' http: ws:; " +
                    "img-src 'self' data: https:; " +
                    "frame-src 'self' https:"
                    );
            checkHeaderValue(response, "Referrer-Policy", "no-referrer");
            checkHeaderValue(response, "strict-transport-security", "max-age=31536000; includeSubDomains");
            checkHeaderValue(response, "x-content-type-options", "nosniff");
            checkHeaderValue(response, "X-Frame-Options", "deny");
            checkHeaderValue(response, "X-XSS-protection", "0");


        }

        private void checkHeaderValue(HttpResponseMessage response, string header, string expectedValue)
        {
            var contentTypeOptionsHeader = response.Headers.GetValues(header).GetEnumerator();
            contentTypeOptionsHeader.MoveNext();
            contentTypeOptionsHeader.Current.Should().Be(expectedValue);
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
            response.RequestMessage.RequestUri.LocalPath.Should().Be("/Home/Welcome");
        }
    }
}
