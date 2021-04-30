namespace DigitalLearningSolutions.Web.Tests.Controllers.Logout
{
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class LogoutControllerTests
    {
        private LogoutController controller;

        [SetUp]
        public void SetUp()
        {
            controller = new LogoutController();
            ControllerContextHelper.SetUpControllerWithSignInFunctionality(ref controller, "mock");
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
