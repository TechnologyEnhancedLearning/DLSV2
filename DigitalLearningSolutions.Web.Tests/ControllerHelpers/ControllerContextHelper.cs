namespace DigitalLearningSolutions.Web.Tests.ControllerHelpers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public static class ControllerContextHelper
    {
        public static void SetUpControllerWithUser<T>(ref T controller, string authenticationType = "")
            where T : Controller
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(authenticationType));
            var session = new MockHttpContextSession();

            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user,
                    Session = session
                }
            };

            controller.ControllerContext = controllerContext;

            var tempData = new TempDataDictionary(controller.HttpContext, A.Fake<ITempDataProvider>());
            controller.TempData = tempData;
        }

        public static void SetUpControllerWithServices<T>(ref T controller, string authenticationType = "")
            where T : Controller
        {
            SetUpControllerWithUser(ref controller, authenticationType);

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
        }
    }
}
