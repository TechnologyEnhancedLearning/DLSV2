namespace DigitalLearningSolutions.Web.IntegrationTests
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;

    public static class CustomWebApplicationFactoryExtensions
    {
        public static WebApplicationFactory<TStartup> WithAuthentication<TStartup>
            (this CustomWebApplicationFactory<TStartup> factory) where TStartup : class
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    factory.ConfigureServicesWithoutAuthentication(services);

                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "Test", options => { });
                });
            });
        }

        public static HttpClient CreateClientWithTestAuth<TStartup>(this CustomWebApplicationFactory<TStartup> factory) where TStartup : class
        {
            var client = factory.WithAuthentication().CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
 
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
 
            return client;
        }
    }
}
