namespace DigitalLearningSolutions.Web.IntegrationTests.TestHelpers
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Net.Http.Headers;

    public static class HttpClientHelper
    {
        public static async Task<HttpClient> SetDelegateSessionAndGetClient(
            AuthenticationWebApplicationFactory<Startup> factory,
            int delegateId
        )
        {
            return await GetClient(factory).GetAsyncAndSetCookie($"/SetDelegateTestSession?delegateId={delegateId}");
        }

        public static async Task<HttpClient> SetDelegateSessionWithoutUserIdClaimAndGetClient(
            AuthenticationWebApplicationFactory<Startup> factory,
            int delegateId
        )
        {
            return await GetClient(factory)
                .GetAsyncAndSetCookie($"/SetDelegateTestSession?delegateId={delegateId}&withoutUserIdClaim=1");
        }

        private static HttpClient GetClient(AuthenticationWebApplicationFactory<Startup> factory)
        {
            var client = factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    HandleCookies = false,
                    AllowAutoRedirect = false,
                }
            );

            return client;
        }

        private static async Task<HttpClient> GetAsyncAndSetCookie(this HttpClient client, string requestUri)
        {
            var cookieContainer = new CookieContainer();
            var response = await client.GetAsync(requestUri);

            if (response.Headers.TryGetValues(HeaderNames.SetCookie, out var responseCookies))
            {
                var cookieHeaders = SetCookieHeaderValue.ParseList(responseCookies.ToList()).ToList();
                cookieHeaders.ForEach(
                    c => cookieContainer.Add(client.BaseAddress, new Cookie(c.Name.Value, c.Value.Value, c.Path.Value))
                );
            }

            client.DefaultRequestHeaders.Add(HeaderNames.Cookie, cookieContainer.GetCookieHeader(client.BaseAddress));
            return client;
        }
    }
}
