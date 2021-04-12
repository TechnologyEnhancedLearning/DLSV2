namespace DigitalLearningSolutions.Web.Tests.Controllers.Login
{
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Login;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    internal class LoginControllerTests
    {
        private LoginController controller;
        private ILoginService loginService;

        [SetUp]
        public void SetUp()
        {
            loginService = A.Fake<ILoginService>();
            controller = LoginTestHelper.GetLoginControllerWithUnauthenticatedUser(loginService);
        }

        [Test]
        public void Index_should_render_if_user_is_unauthenticated()
        {
            // When
            var result = controller.Index();

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void Index_should_redirect_if_user_is_authenticated()
        {
            // Given
            var controllerWithAuthenticatedUser = LoginTestHelper.GetLoginControllerWithAuthenticatedUser(loginService);

            // When
            var result = controllerWithAuthenticatedUser.Index();

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public void Successful_sign_in_should_render_home_page()
        {
            // Given
            A.CallTo(() => loginService.VerifyUserDetailsAndGetClaims(A<string>._, A<string>._)).Returns((UserTestHelper.GetDefaultAdminUser(), UserTestHelper.GetDefaultDelegateUser()));
            controller = LoginTestHelper.GetLoginControllerWithSignInFunctionality(loginService);

            // When
            var result = controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public void Log_in_request_should_call_login_service()
        {
            // Given
            A.CallTo(() => loginService.VerifyUserDetailsAndGetClaims(A<string>._, A<string>._)).Returns((UserTestHelper.GetDefaultAdminUser(), UserTestHelper.GetDefaultDelegateUser()));
            controller = LoginTestHelper.GetLoginControllerWithSignInFunctionality(loginService);

            // When
            controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            A.CallTo(() => loginService.VerifyUserDetailsAndGetClaims(A<string>._, A<string>._)).MustHaveHappened();
        }

        [Test]
        public void No_user_account_found_should_render_basic_form_with_error()
        {
            // Given
            A.CallTo(() => loginService.VerifyUserDetailsAndGetClaims(A<string>._, A<string>._))
                .Throws(new UserAccountNotFoundException("No user account found for that email or user id"));

            // When
            var result = controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("Index")
                .ModelAs<LoginViewModel>();
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [Test]
        public void Bad_password_should_render_basic_form_with_error()
        {
            // Given
            A.CallTo(() => loginService.VerifyUserDetailsAndGetClaims(A<string>._, A<string>._))
                .Throws(new IncorrectPasswordLoginException("Password incorrect for that account"));

            // When
            var result = controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("Index")
                .ModelAs<LoginViewModel>();
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [Test]
        public void Unapproved_delegate_account_redirect_to_not_approved_page()
        {
            // Given
            A.CallTo(() => loginService.VerifyUserDetailsAndGetClaims(A<string>._, A<string>._))
                .Throws(new DelegateUserNotApprovedException("Delegate account not approved"));

            // When
            var result = controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("AccountNotApproved");
        }
    }
}
