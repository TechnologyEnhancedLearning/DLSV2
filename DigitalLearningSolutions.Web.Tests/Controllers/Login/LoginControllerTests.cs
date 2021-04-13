namespace DigitalLearningSolutions.Web.Tests.Controllers.Login
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
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
        private ICryptoService cryptoService;
        private IUserService userService;

        [SetUp]
        public void SetUp()
        {
            userService = A.Fake<IUserService>();
            cryptoService = A.Fake<ICryptoService>();

            A.CallTo(() => userService.GetAdminUserByUsername(A<string>._))
                .Returns(UserTestHelper.GetDefaultAdminUser());
            A.CallTo(() => userService.GetDelegateUsersByUsername(A<string>._))
                .Returns(new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() });
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._))
                .Returns(true);

            controller = LoginTestHelper.GetLoginControllerWithUnauthenticatedUser(userService, cryptoService);
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
            var controllerWithAuthenticatedUser =
                LoginTestHelper.GetLoginControllerWithAuthenticatedUser(userService, cryptoService);

            // When
            var result = controllerWithAuthenticatedUser.Index();

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public void Successful_sign_in_should_render_home_page()
        {
            controller = LoginTestHelper.GetLoginControllerWithSignInFunctionality(userService, cryptoService);

            // When
            var result = controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public void Log_in_request_should_call_user_service()
        {
            controller = LoginTestHelper.GetLoginControllerWithSignInFunctionality(userService, cryptoService);

            // When
            controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            A.CallTo(() => userService.GetAdminUserByUsername(A<string>._))
                .MustHaveHappened();
            A.CallTo(() => userService.GetDelegateUsersByUsername(A<string>._))
                .MustHaveHappened();
        }

        [Test]
        public void Log_in_request_should_call_crypto_service()
        {
            controller = LoginTestHelper.GetLoginControllerWithSignInFunctionality(userService, cryptoService);

            // When
            controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._))
                .MustHaveHappened();
        }

        [Test]
        public void No_user_account_found_should_render_basic_form_with_error()
        {
            // Given
            A.CallTo(() => userService.GetAdminUserByUsername(A<string>._))
                .Returns(null);
            A.CallTo(() => userService.GetDelegateUsersByUsername(A<string>._))
                .Returns(new List<DelegateUser>());

            // When
            var result =
                controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("Index").ModelAs<LoginViewModel>();
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [Test]
        public void Bad_password_should_render_basic_form_with_error()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._))
                .Returns(false);

            // When
            var result =
                controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("Index").ModelAs<LoginViewModel>();
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [Test]
        public void Unapproved_delegate_account_redirect_to_not_approved_page()
        {
            // Given
            A.CallTo(() => userService.GetAdminUserByUsername(A<string>._))
                .Returns(null);
            A.CallTo(() => userService.GetDelegateUsersByUsername(A<string>._))
                .Returns(new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser(approved: false) });

            // When
            var result =
                controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("AccountNotApproved");
        }
    }
}
