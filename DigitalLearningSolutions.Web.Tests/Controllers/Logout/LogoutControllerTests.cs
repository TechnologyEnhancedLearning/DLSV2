namespace DigitalLearningSolutions.Web.Tests.Controllers.Logout
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Web.Controllers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Routing;
    using NUnit.Framework;

    public class LogoutControllerTests
    {
        private LogoutController controller;

        [SetUp]
        public void SetUp()
        {
            // Set up authenticated user
            var user = new ClaimsPrincipal(new ClaimsIdentity("mock"));
            var session = new MockHttpContextSession();

            var authService = A.Fake<IAuthenticationService>();
            A.CallTo(() => authService.SignInAsync(A<HttpContext>._, A<string>._, A<ClaimsPrincipal>._,
                A<AuthenticationProperties>._)).Returns(Task.CompletedTask);

            var urlHelperFactory = A.Fake<IUrlHelperFactory>();
            var services = A.Fake<IServiceProvider>();
            A.CallTo(() => services.GetService(typeof(IAuthenticationService))).Returns(authService);
            A.CallTo(() => services.GetService(typeof(IUrlHelperFactory))).Returns(urlHelperFactory);

            controller = new LogoutController
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

        [Test]
        public void Logout_should_redirect_user_to_home_page()
        {
            // When
            var result = controller.Index();

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home").WithActionName("Index");
        }
    }
}
