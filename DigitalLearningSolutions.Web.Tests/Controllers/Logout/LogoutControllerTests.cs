namespace DigitalLearningSolutions.Web.Tests.Controllers.Logout
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Net.Http.Headers;
    using NUnit.Framework;

    internal class LogoutControllerTests
    {
        private IAuthenticationService? authenticationService = null!;
        private IConfiguration configuration = null!;
        private LogoutController controller = null!;

        [SetUp]
        public void SetUp()
        {
            configuration = A.Fake<IConfiguration>();
            controller = new LogoutController(configuration)
                .WithDefaultContext()
                .WithMockUser(true)
                .WithMockServices();

            authenticationService =
                (IAuthenticationService?)controller
                .HttpContext
                .RequestServices
                .GetService(typeof(IAuthenticationService));
        }

        [Test]
        public async Task Logout_should_redirect_user_to_home_page()
        {
            // When
            var result = await controller.Index();

            // Then
            result
                .Should()
                .BeRedirectResult()
                .WithUrl("/home");
        }

        [Test]
        public async Task Logout_should_call_SignOutAsync()
        {
            // When
            await controller.Index();

            // Then
            A.CallTo(() =>authenticationService!
                .SignOutAsync(A<HttpContext>._,
                A<string>._,
                A<AuthenticationProperties>._)
            )
            .MustHaveHappened();
        }

        [Test]
        public async Task Logout_should_redirect_to_home_page_not_autheticated()
        {
            var controller = new LogoutController(configuration)
               .WithDefaultContext()
               .WithMockUser(false)
               .WithMockServices();

            var result = await controller.Index();

            // Then
            result
                .Should()
                .BeRedirectToActionResult("home");
        }

        [Test]
        public async Task Logout_should_redirect_to_logout_external_provider()
        {
            // Given
            var context = controller.ControllerContext.HttpContext;
            var request = context.Request;
            var cookieDict = new Dictionary<string, string>()
            {
                {"id_token", "my_id_token_value"},
                {"auth_method", "oidc"}
            };
            var cookies = MockRequestCookieCollection(
                cookieDict);
            request.Cookies = cookies;

            // When
            var result = await controller.Index();

            // Then
            A.CallTo(() => authenticationService!
               .SignOutAsync(A<HttpContext>._,
               "Identity.Application",
               A<AuthenticationProperties>._)
           )
           .MustHaveHappened();
            A.CallTo(() => authenticationService!
               .SignOutAsync(A<HttpContext>._,
               OpenIdConnectDefaults.AuthenticationScheme,
               A<AuthenticationProperties>._)
           )
           .MustHaveHappened();
            result
               .Should()
               .BeOfType<RedirectResult>();
            result
                .Should()
                .BeRedirectResult()
                .WithUrl("/connect/endsession?post_logout_redirect_uri=%2Fsignout-callback-oidc&id_token_hint=my_id_token_value");
        }

        [Test]
        public void LogoutExternalProvider_Returns_RedirectResult()
        {
            // Arrange
            var context = controller.ControllerContext.HttpContext;
            var request = context.Request;
            var cookieDict = new Dictionary<string, string>()
            {
                {"id_token", "my_id_token_value"}
            };
            var cookies = MockRequestCookieCollection(
                cookieDict);
            request.Cookies = cookies;

            // Act
            var result = controller.LogoutSharedAuth();

            // Assert
            result
                .Should()
                .BeOfType<RedirectResult>();
            result
                .Should()
                .BeRedirectResult()
                .WithUrl("/connect/endsession?post_logout_redirect_uri=%2Fsignout-callback-oidc&id_token_hint=my_id_token_value");
        }

        private static IRequestCookieCollection MockRequestCookieCollection(
           Dictionary<string,string> cookieDict)
        {
            var requestFeature = new HttpRequestFeature();
            var featureCollection = new FeatureCollection();
            requestFeature.Headers = new HeaderDictionary();
            var cookieString = string.Join(";", cookieDict.Select(x => x.Key + "=" + x.Value));
            requestFeature.Headers.Add(
                HeaderNames.Cookie,
                cookieString);
            featureCollection.Set<IHttpRequestFeature>(requestFeature);
            var cookiesFeature = new RequestCookiesFeature(featureCollection);
            return cookiesFeature.Cookies;
        }

    }
}
