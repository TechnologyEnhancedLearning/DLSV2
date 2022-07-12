namespace DigitalLearningSolutions.Web.Tests.Controllers.Login
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Login;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    internal class LoginControllerTests
    {
        private IAuthenticationService authenticationService = null!;
        private LoginController controller = null!;
        private ILogger<LoginController> logger = null!;
        private ILoginService loginService = null!;
        private ISessionService sessionService = null!;

        [SetUp]
        public void SetUp()
        {
            loginService = A.Fake<ILoginService>();
            sessionService = A.Fake<ISessionService>();
            logger = A.Fake<ILogger<LoginController>>();

            controller = new LoginController(loginService, sessionService, logger)
                .WithDefaultContext()
                .WithMockUser(false)
                .WithMockTempData()
                .WithMockServices();

            authenticationService =
                (IAuthenticationService)controller.HttpContext.RequestServices.GetService(
                    typeof(IAuthenticationService)
                );
        }

        [Test]
        public void Index_should_render_basic_form()
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
            var controllerWithAuthenticatedUser = new LoginController(
                    loginService,
                    sessionService,
                    logger
                )
                .WithDefaultContext()
                .WithMockUser(true);

            // When
            var result = controllerWithAuthenticatedUser.Index();

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public async Task Invalid_model_should_render_basic_form_with_error()
        {
            // Given
            controller.ModelState.AddModelError(nameof(LoginViewModel.Username), "Required");

            // When
            var result = await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("Index").ModelAs<LoginViewModel>();
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [Test]
        public async Task Successful_sign_in_without_return_url_should_render_home_pageAsync()
        {
            // Given
            GivenSignInIsSuccessful();

            // When
            var result = await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public async Task Successful_sign_in_with_local_return_url_should_redirect_to_return_url()
        {
            // Given
            GivenSignInIsSuccessful();

            var returnUrl = "/some/other/page";
            var urlHelper = controller.Url;
            A.CallTo(() => urlHelper.IsLocalUrl(returnUrl)).Returns(true);

            // When
            var loginViewModel = LoginTestHelper.GetDefaultLoginViewModel();
            loginViewModel.ReturnUrl = returnUrl;
            var result = await controller.Index(loginViewModel);

            // Then
            result.Should().BeRedirectResult().WithUrl(returnUrl);
        }

        [Test]
        public async Task Successful_sign_in_with_query_string_should_redirect_with_correct_query()
        {
            // Given
            GivenSignInIsSuccessful();

            var returnUrl = "/some/other/page?query=true&other=false";
            var urlHelper = controller.Url;
            A.CallTo(() => urlHelper.IsLocalUrl(returnUrl)).Returns(true);
            var loginViewModel = LoginTestHelper.GetDefaultLoginViewModel();
            loginViewModel.ReturnUrl = returnUrl;

            // When
            var result = await controller.Index(loginViewModel);

            // Then
            result.Should().BeRedirectResult().WithUrl(returnUrl);
        }

        [Test]
        public async Task Successful_sign_in_with_nonlocal_return_url_should_render_home_page()
        {
            // Given
            GivenSignInIsSuccessful();

            var returnUrl = "www.suspicious.com";
            var urlHelper = controller.Url;
            A.CallTo(() => urlHelper.IsLocalUrl(returnUrl)).Returns(false);

            // When
            var loginViewModel = LoginTestHelper.GetDefaultLoginViewModel();
            loginViewModel.ReturnUrl = returnUrl;
            var result = await controller.Index(loginViewModel);

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public async Task Successful_sign_in_should_call_SignInAsync()
        {
            // Given
            GivenSignInIsSuccessful();

            // When
            await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            A.CallTo(
                    () => authenticationService.SignInAsync(
                        A<HttpContext>._,
                        A<string>._,
                        A<ClaimsPrincipal>._,
                        A<AuthenticationProperties>._
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public async Task Invalid_credentials_should_render_basic_form_with_error()
        {
            // Given
            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.InvalidCredentials)
            );

            // When
            var result = await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("Index").ModelAs<LoginViewModel>();
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [Test]
        public async Task Unapproved_delegate_account_redirects_to_not_approved_page()
        {
            // Given
            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.AccountNotApproved)
            );

            // When
            var result = await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("AccountNotApproved");
        }

        [Test]
        public async Task Multiple_available_centres_should_redirect_to_ChooseACentre_page()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser(centreId: 1, centreName: "Centre 1");
            var expectedDelegateUsers = new List<DelegateUser>
                { UserTestHelper.GetDefaultDelegateUser(centreId: 2, centreName: "Centre 2") };
            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.ChooseACentre, expectedAdminUser, expectedDelegateUsers)
            );

            // When
            var result = await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("ChooseACentre");
        }

        [Test]
        public async Task Log_in_as_admin_records_admin_session()
        {
            // Given
            var expectedAdmin = UserTestHelper.GetDefaultAdminUser(10);
            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.LogIntoSingleCentre, expectedAdmin, new List<DelegateUser>())
            );

            // When
            await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            A.CallTo(() => sessionService.StartAdminSession(expectedAdmin.Id))
                .MustHaveHappened();
        }

        [Test]
        public async Task Log_in_as_delegate_does_not_record_admin_session()
        {
            // Given
            var expectedDelegates = new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() };
            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.LogIntoSingleCentre, delegateUsers: expectedDelegates)
            );

            // When
            await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            A.CallTo(() => sessionService.StartAdminSession(A<int>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Multiple_approved_accounts_at_same_centre_should_log_in()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser(centreId: 1, centreName: "Centre 1");
            var expectedDelegateUsers = new List<DelegateUser>
                { UserTestHelper.GetDefaultDelegateUser(centreId: 1, centreName: "Centre 1", approved: true) };
            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.LogIntoSingleCentre, expectedAdminUser, expectedDelegateUsers)
            );

            // When
            var result = await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public async Task
            When_user_has_verified_accounts_only_at_inactive_centres_then_redirect_to_centre_inactive_page()
        {
            // Given
            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.InactiveCentre)
            );

            // When
            var result = await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("CentreInactive");
        }

        [Test]
        public async Task Leading_trailing_whitespaces_in_username_are_ignored()
        {
            // Given
            GivenSignInIsSuccessful();

            // When
            await controller.Index(LoginTestHelper.GetDefaultLoginViewModel("\ttest@example.com "));

            // Then
            A.CallTo(() => loginService.AttemptLogin("test@example.com", "testPassword")).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public async Task Locked_admin_returns_locked_view()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(failedLoginCount: 6);
            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.AccountLocked, adminUser)
            );

            // When
            var result = await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("AccountLocked");
        }

        private void GivenSignInIsSuccessful()
        {
            var admin = UserTestHelper.GetDefaultAdminUser();
            var delegates = new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() };

            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.LogIntoSingleCentre, admin, delegates)
            );
        }
    }
}
