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
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

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

        public static LoginController GetLoginControllerWithUnauthenticatedUser
        (
            ILoginService loginService,
            IUserService userService
        )
        {
            return GetLoginController(loginService, userService, string.Empty);
        }

        public static LoginController GetLoginControllerWithAuthenticatedUser
        (
            ILoginService loginService,
            IUserService userService
        )
        {
            return GetLoginController(loginService, userService, "mock");
        }

        private static LoginController GetLoginController
        (
            ILoginService loginService,
            IUserService userService,
            string authenticationType
        )
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(authenticationType));
            var session = new MockHttpContextSession();

            var controller = new LoginController(loginService, userService)
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

            var tempData = new TempDataDictionary(controller.HttpContext, A.Fake<ITempDataProvider>());
            controller.TempData = tempData;

            return controller;
        }

        public static LoginController GetLoginControllerWithSignInFunctionality
        (
            ILoginService loginService,
            IUserService userService
        )
        {
            var controller = GetLoginControllerWithUnauthenticatedUser(loginService, userService);

            var authService = A.Fake<IAuthenticationService>();
            A.CallTo(() => authService.SignInAsync(A<HttpContext>._, A<string>._, A<ClaimsPrincipal>._,
                A<AuthenticationProperties>._)).Returns(Task.CompletedTask);

            var urlHelperFactory = A.Fake<IUrlHelperFactory>();
            var services = A.Fake<IServiceProvider>();
            A.CallTo(() => services.GetService(typeof(IAuthenticationService))).Returns(authService);
            A.CallTo(() => services.GetService(typeof(IUrlHelperFactory))).Returns(urlHelperFactory);

            controller.HttpContext.RequestServices = services;

            return controller;
        }
    }
}
