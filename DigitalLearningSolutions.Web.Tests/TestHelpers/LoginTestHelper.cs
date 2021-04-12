namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.ViewModels.Login;
    using FakeItEasy;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Routing;

    public static class LoginTestHelper
    {
        public static LoginViewModel GetDefaultLoginViewModel
        (
            string username = "testUsername",
            string password = "testPassword",
            bool rememberMe = false
        )
        {
            return new LoginViewModel
            {
                Username = username,
                Password = password,
                RememberMe = rememberMe
            };
        }

        public static LoginController GetLoginControllerWithUnauthenticatedUser(ILoginService loginService)
        {
            return GetLoginController(loginService, string.Empty);
        }

        public static LoginController GetLoginControllerWithAuthenticatedUser(ILoginService loginService)
        {
            return GetLoginController(loginService, "mock");
        }

        private static LoginController GetLoginController(ILoginService loginService, string authenticationType)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(authenticationType));
            var session = new MockHttpContextSession();

            return new LoginController(loginService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = user,
                        Session = session
                    }
                }
            };
        }

        public static LoginController GetLoginControllerWithSignInFunctionality(ILoginService loginService)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(""));
            var session = new MockHttpContextSession();

            var authService = A.Fake<IAuthenticationService>();
            A.CallTo(() => authService.SignInAsync(A<HttpContext>._, A<string>._, A<ClaimsPrincipal>._, A<AuthenticationProperties>._)).Returns(Task.CompletedTask);

            var urlHelperFactory = A.Fake<IUrlHelperFactory>();

            var services = A.Fake<IServiceProvider>();
            A.CallTo(() => services.GetService(typeof(IAuthenticationService))).Returns(authService);
            A.CallTo(() => services.GetService(typeof(IUrlHelperFactory))).Returns(urlHelperFactory);
            return new LoginController(loginService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = user,
                        Session = session,
                        RequestServices = services
                    }
                }
            };
        }
    }
}
