namespace DigitalLearningSolutions.Web.Tests.Controllers.Login
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Login;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    internal class LoginControllerTests
    {
        private IAuthenticationService authenticationService = null!;
        private IAuthenticationService authenticationServiceWithAuthenticatedUser = null!;
        private LoginController controller = null!;
        private LoginController controllerWithAuthenticatedUser = null!;
        private ILogger<LoginController> logger = null!;
        private ILoginService loginService = null!;
        private ISessionService sessionService = null!;
        private IUserService userService = null!;
        private IUrlHelper urlHelper = null!;

        [SetUp]
        public void SetUp()
        {
            loginService = A.Fake<ILoginService>();
            sessionService = A.Fake<ISessionService>();
            logger = A.Fake<ILogger<LoginController>>();
            userService = A.Fake<IUserService>();
            urlHelper = A.Fake<IUrlHelper>();

            controller = new LoginController(loginService, sessionService, logger, userService)
                .WithDefaultContext()
                .WithMockUser(false)
                .WithMockTempData()
                .WithMockServices()
                .WithMockUrlHelper(urlHelper);

            authenticationService =
                (IAuthenticationService)controller.HttpContext.RequestServices.GetService(
                    typeof(IAuthenticationService)
                );

            controllerWithAuthenticatedUser = new LoginController(loginService, sessionService, logger, userService)
                .WithDefaultContext()
                .WithMockUser(true)
                .WithMockTempData()
                .WithMockServices()
                .WithMockUrlHelper(urlHelper);

            authenticationServiceWithAuthenticatedUser =
                (IAuthenticationService)controllerWithAuthenticatedUser.HttpContext.RequestServices.GetService(
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
        public async Task Successful_sign_in_for_user_who_needs_details_check_redirects_to_edit_details()
        {
            // Given
            GivenSignInIsSuccessful();
            A.CallTo(() => userService.ShouldForceDetailsCheck(A<UserEntity>._, A<int>._)).Returns(true);

            // When
            var loginViewModel = LoginTestHelper.GetDefaultLoginViewModel();
            var result = await controller.Index(loginViewModel);

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("MyAccount").WithActionName("EditDetails");
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
        public async Task No_user_account_found_should_render_basic_form_with_error()
        {
            // Given
            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.InvalidUsername)
            );

            // When
            var result = await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("Index").ModelAs<LoginViewModel>();
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [Test]
        public async Task Bad_password_should_render_basic_form_with_error()
        {
            // Given
            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.InvalidPassword)
            );

            // When
            var result = await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("Index").ModelAs<LoginViewModel>();
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [Test]
        public async Task Multiple_available_centres_should_redirect_to_ChooseACentre_page()
        {
            // Given
            var userEntity = GetUserEntity(true, true);

            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.ChooseACentre, userEntity, 2)
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
            var userEntity = GetUserEntity(true, false);

            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.LogIntoSingleCentre, userEntity, 2)
            );

            // When
            await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            A.CallTo(() => sessionService.StartAdminSession(userEntity.AdminAccounts.Single().Id))
                .MustHaveHappened();
        }

        [Test]
        public async Task Log_in_as_delegate_does_not_record_admin_session()
        {
            // Given
            var userEntity = GetUserEntity(false, true);

            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.LogIntoSingleCentre, userEntity, 2)
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
            var userEntity = GetUserEntity(true, true);

            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.LogIntoSingleCentre, userEntity, 2)
            );

            // When
            var result = await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public async Task Leading_trailing_whitespaces_in_username_are_ignored()
        {
            // Given
            GivenSignInIsSuccessful();

            // When
            await controller.Index(LoginTestHelper.GetDefaultLoginViewModel("\ttest@example.com "));

            // Then
            A.CallTo(() => loginService.AttemptLogin("test@example.com", "testPassword"))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void ChooseACentre_should_render_page()
        {
            // Given
            const string returnUrl = "/some/other/page";
            var userEntity = GetUserEntity(true, true);

            GivenAUserEntityWithAdminAndDelegateAccounts(userEntity);

            // When
            var result = controllerWithAuthenticatedUser.ChooseACentre(returnUrl);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult().WithViewName("ChooseACentre");

                result.As<ViewResult>().Model.As<ChooseACentreViewModel>().ReturnUrl.Should().Be(returnUrl);

                A.CallTo(() => loginService.GetChooseACentreAccountViewModels(userEntity))
                    .MustHaveHappened();
            }
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public async Task ChooseCentre_should_redirect_to_access_denied_if_centre_is_inactive(
            bool withAdminAccount,
            bool withDelegateAccount
        )
        {
            // Given
            const int centreId = 2;
            var userEntity = GetUserEntity(
                withAdminAccount,
                withDelegateAccount,
                centreId: centreId,
                isCentreActive: false
            );

            GivenAUserEntityWithAdminAndDelegateAccounts(userEntity);

            // When
            var result = await controllerWithAuthenticatedUser.ChooseCentre(centreId, null);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        [TestCase(true, false, 1)]
        [TestCase(false, true, 1)]
        [TestCase(true, true, 1)]
        [TestCase(true, false, 2)]
        [TestCase(false, true, 2)]
        [TestCase(true, true, 2)]
        public async Task ChooseCentre_should_uses_the_correct_accounts_to_determine_if_centre_is_inactive(
            bool withAdminAccount,
            bool withDelegateAccount,
            int centreId
        )
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                withAdminAccount
                    ? new List<AdminAccount>
                    {
                        UserTestHelper.GetDefaultAdminAccount(centreId: 1, centreActive: true),
                        UserTestHelper.GetDefaultAdminAccount(centreId: 2, centreActive: false),
                    }
                    : new List<AdminAccount>(),
                withDelegateAccount
                    ? new List<DelegateAccount>
                    {
                        UserTestHelper.GetDefaultDelegateAccount(centreId: 1, centreActive: true),
                        UserTestHelper.GetDefaultDelegateAccount(centreId: 2, centreActive: false),
                    }
                    : new List<DelegateAccount>()
            );

            GivenAUserEntityWithAdminAndDelegateAccounts(userEntity);

            // When
            var result = await controllerWithAuthenticatedUser.ChooseCentre(centreId, null);

            // Then
            if (centreId == 2)
            {
                result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                    .WithActionName("AccessDenied");
            }
            else
            {
                result.Should().BeRedirectToActionResult().WithControllerName("Home")
                    .WithActionName("Index");
            }
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public async Task ChooseCentre_should_log_out_and_back_in_to_single_centre_if_centre_is_active(
            bool withAdminAccount,
            bool withDelegateAccount
        )
        {
            // Given
            const int centreId = 2;
            var userEntity = GetUserEntity(withAdminAccount, withDelegateAccount, centreId: centreId);

            GivenAUserEntityWithAdminAndDelegateAccounts(userEntity);

            // When
            await controllerWithAuthenticatedUser.ChooseCentre(centreId, null);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => authenticationServiceWithAuthenticatedUser.SignOutAsync(
                        A<HttpContext>._,
                        A<string>._,
                        A<AuthenticationProperties>._
                    )
                ).MustHaveHappened();

                A.CallTo(
                    () => authenticationServiceWithAuthenticatedUser.SignInAsync(
                        A<HttpContext>._,
                        A<string>._,
                        A<ClaimsPrincipal>._,
                        A<AuthenticationProperties>._
                    )
                ).MustHaveHappened();
            }
        }

        [Test]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(true, true, true)]
        [TestCase(true, false, false, true)]
        [TestCase(false, true, false)]
        [TestCase(true, true, false)]
        public async Task ChooseCentre_should_start_an_admin_session_when_necessary(
            bool withAdminAccount,
            bool withDelegateAccount,
            bool isAdminAccountActive,
            bool shouldThrowException = false
        )
        {
            // Given
            const int centreId = 2;
            var threwException = false;
            var userEntity = GetUserEntity(
                withAdminAccount,
                withDelegateAccount,
                isAdminAccountActive: isAdminAccountActive,
                centreId: centreId
            );

            GivenAUserEntityWithAdminAndDelegateAccounts(userEntity);

            // When
            try
            {
                await controllerWithAuthenticatedUser.ChooseCentre(centreId, null);
            }
            catch (Exception e)
            {
                threwException = true;
            }

            // Then
            if (shouldThrowException)
            {
                threwException.Should().BeTrue();
            }
            else
            {
                threwException.Should().BeFalse();

                if (withAdminAccount && isAdminAccountActive)
                {
                    A.CallTo(() => sessionService.StartAdminSession(userEntity.AdminAccounts.First().Id))
                        .MustHaveHappened();
                }
                else
                {
                    A.CallTo(() => sessionService.StartAdminSession(A<int>._)).MustNotHaveHappened();
                }
            }
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("/some/other/page", true)]
        [TestCase("/some/other/page", false)]
        [TestCase(null, true)]
        public async Task ChooseCentre_redirects_to_the_correct_page_after_login(
            string? returnUrl,
            bool? isReturnUrlValid
        )
        {
            // Given
            GivenAUserEntityWithAdminAndDelegateAccounts();

            A.CallTo(() => urlHelper.IsLocalUrl(A<string>._)).Returns(isReturnUrlValid ?? false);

            // When
            var result = await controllerWithAuthenticatedUser.ChooseCentre(2, returnUrl);

            // Then
            if (returnUrl != null && isReturnUrlValid == true)
            {
                result.Should().BeRedirectResult().WithUrl(returnUrl);
            }
            else
            {
                result.Should().BeRedirectToActionResult().WithControllerName("Home")
                    .WithActionName("Index");
            }
        }

        [Test]
        [TestCase(null)]
        [TestCase("/some/other/page")]
        public async Task ChooseCentre_redirects_to_edit_details_page_after_login_if_user_details_need_editing(
            string? returnUrl
        )
        {
            // Given
            GivenAUserEntityWithAdminAndDelegateAccounts();

            A.CallTo(() => userService.ShouldForceDetailsCheck(A<UserEntity>._, A<int>._)).Returns(true);

            // When
            var result = await controllerWithAuthenticatedUser.ChooseCentre(2, returnUrl);
            var routeValues = result.As<RedirectToActionResult>().RouteValues;

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userService.ShouldForceDetailsCheck(A<UserEntity>._, A<int>._)).MustHaveHappened();

                result.Should().BeRedirectToActionResult().WithControllerName("MyAccount")
                    .WithActionName("EditDetails")
                    .WithRouteValue("isCheckDetailsRedirect", true);

                if (returnUrl == null)
                {
                    routeValues.Keys.Should().NotContain("returnUrl");
                    routeValues.Keys.Should().NotContain("dlsSubApplication");
                }
                else
                {
                    routeValues.Should().Contain("returnUrl", returnUrl);
                    routeValues.Keys.Should().Contain("dlsSubApplication");
                }
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task ChooseCentre_remember_me_setting_is_used_to_set_authentication_persistence(bool rememberMe)
        {
            // Given
            var authenticateResult = GetAuthenticateResult(rememberMe);

            GivenAUserEntityWithAdminAndDelegateAccounts(authenticateResult: authenticateResult);

            // When
            await controllerWithAuthenticatedUser.ChooseCentre(2, null);

            // Then
            A.CallTo(
                () => authenticationServiceWithAuthenticatedUser.SignInAsync(
                    A<HttpContext>._,
                    A<string>._,
                    A<ClaimsPrincipal>._,
                    A<AuthenticationProperties>.That.Matches(
                        props => props.IsPersistent == rememberMe
                    )
                )
            ).MustHaveHappened();
        }

        private void GivenSignInIsSuccessful()
        {
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount> { UserTestHelper.GetDefaultAdminAccount() },
                new List<DelegateAccount> { UserTestHelper.GetDefaultDelegateAccount() }
            );

            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.LogIntoSingleCentre, userEntity, 2)
            );
        }

        private UserEntity GetUserEntity(
            bool withAdminAccount,
            bool withDelegateAccount,
            bool isAdminAccountActive = true,
            bool isDelegateAccountActive = true,
            bool isDelegateAccountApproved = true,
            int centreId = 2,
            bool isCentreActive = true
        )
        {
            var adminAccount = UserTestHelper.GetDefaultAdminAccount(
                centreId: centreId,
                centreActive: isCentreActive,
                active: isAdminAccountActive
            );

            var delegateAccount = UserTestHelper.GetDefaultDelegateAccount(
                centreId: centreId,
                centreActive: isCentreActive,
                active: isDelegateAccountActive,
                approved: isDelegateAccountApproved
            );

            return new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                withAdminAccount ? new List<AdminAccount> { adminAccount } : new List<AdminAccount>(),
                withDelegateAccount ? new List<DelegateAccount> { delegateAccount } : new List<DelegateAccount>()
            );
        }

        private AuthenticateResult GetAuthenticateResult(bool rememberMe = false)
        {
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
            };

            return AuthenticateResult.Success(
                new AuthenticationTicket(new ClaimsPrincipal(), authenticationProperties, "test")
            );
        }

        private void GivenAUserEntityWithAdminAndDelegateAccounts(
            UserEntity? userEntity = null,
            AuthenticateResult? authenticateResult = null
        )
        {
            A.CallTo(() => authenticationServiceWithAuthenticatedUser.AuthenticateAsync(A<HttpContext>._, A<string>._))
                .Returns(authenticateResult ?? GetAuthenticateResult());

            A.CallTo(() => userService.GetUserById(A<int>._)).Returns(userEntity ?? GetUserEntity(true, true));
        }
    }
}
