namespace DigitalLearningSolutions.Web.Tests.Controllers.Logout
{
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    internal class LogoutControllerTests
    {
        private IAuthenticationService authenticationService;
        private LogoutController controller;

        [SetUp]
        public void SetUp()
        {
            controller = new LogoutController();
            ControllerContextHelper.SetUpControllerWithServices(ref controller);

            authenticationService =
                (IAuthenticationService)controller.HttpContext.RequestServices.GetService(
                    typeof(IAuthenticationService));
        }

        [Test]
        public void Logout_should_redirect_user_to_home_page()
        {
            // When
            var result = controller.Index();

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public void Logout_should_call_SignOutAsync()
        {
            // When
            controller.Index();

            // Then
            A.CallTo(() =>
                    authenticationService.SignOutAsync(A<HttpContext>._, A<string>._, A<AuthenticationProperties>._))
                .MustHaveHappened();
        }
    }
}
