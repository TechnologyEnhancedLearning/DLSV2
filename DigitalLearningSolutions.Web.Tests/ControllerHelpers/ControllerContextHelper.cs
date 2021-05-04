namespace DigitalLearningSolutions.Web.Tests.ControllerHelpers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using FakeItEasy;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public static class ControllerContextHelper
    {
        public static T WithDefaultContext<T>(this T controller) where T : Controller
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            return controller;
        }

        public static T WithMockUser<T>(this T controller, bool isAuthenticated) where T : Controller
        {
            var authenticationType = isAuthenticated ? "mock" : string.Empty;
            controller.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(authenticationType));
            return controller;
        }

        public static T WithMockTempData<T>(this T controller) where T : Controller
        {
            controller.TempData = new TempDataDictionary(controller.HttpContext, A.Fake<ITempDataProvider>());
            return controller;
        }

        public static T WithMockServices<T>(this T controller) where T : Controller
        {
            var authService = A.Fake<IAuthenticationService>();
            A.CallTo(() => authService.SignInAsync(A<HttpContext>._, A<string>._, A<ClaimsPrincipal>._,
                A<AuthenticationProperties>._)).Returns(Task.CompletedTask);
            A.CallTo(() => authService.SignOutAsync(A<HttpContext>._, A<string>._,
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
