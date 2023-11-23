namespace DigitalLearningSolutions.Web.Tests.Controllers.Logout
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    internal class LogoutControllerTests
    {
        private IAuthenticationService authenticationService = null!;
        private LogoutController controller = null!;

        [SetUp]
        public void SetUp()
        {
            controller = new LogoutController()
                .WithDefaultContext()
                .WithMockUser(true)
                .WithMockServices();

            authenticationService =
                (IAuthenticationService)controller.HttpContext.RequestServices.GetService(
                    typeof(IAuthenticationService)
                );
        }

        [Test]
        public async Task Logout_should_redirect_user_to_home_page()
        {
            // When
            var result = await controller.Index();

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public async Task Logout_should_call_SignOutAsync()
        {
            // When
            await controller.Index();

            // Then
            A.CallTo(
                    () =>
                        authenticationService.SignOutAsync(A<HttpContext>._, A<string>._, A<AuthenticationProperties>._)
                )
                .MustHaveHappened();
        }
    }
}
