namespace DigitalLearningSolutions.Web.Tests.Controllers.Login
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Login;
    using DocumentFormat.OpenXml.EMMA;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    internal class LoginControllerTests
    {
        private IAuthenticationService? authenticationService = null!;
        private IAuthenticationService? authenticationServiceWithAuthenticatedUser = null!;
        private IClockUtility clockUtility = null!;
        private LoginController controller = null!;
        private LoginController controllerWithAuthenticatedUser = null!;
        private ILogger<LoginController> logger = null!;
        private ILoginService loginService = null!;
        private ISessionService sessionService = null!;
        private IUrlHelper urlHelper = null!;
        private IConfigDataService configDataService = null!;
        private IUserService userService = null!;
        private IConfiguration config = null!;
        private ILearningHubUserApiClient apiClient = null!;

        [SetUp]
        public void SetUp()
        {
            loginService = A.Fake<ILoginService>();
            sessionService = A.Fake<ISessionService>();
            logger = A.Fake<ILogger<LoginController>>();
            userService = A.Fake<IUserService>();
            urlHelper = A.Fake<IUrlHelper>();
            configDataService = A.Fake<IConfigDataService>();
            clockUtility = A.Fake<IClockUtility>();
            config = A.Fake<IConfiguration>();
            apiClient = A.Fake<ILearningHubUserApiClient>();
            

            A.CallTo(() => clockUtility.UtcNow).Returns(DateTime.UtcNow);

            controller = new LoginController(
                    loginService,
                    sessionService,
                    logger,
                    userService,
                    clockUtility,
                    configDataService,
                    config,
                    apiClient
                )
                .WithDefaultContext()
                .WithMockUser(false)
                .WithMockTempData()
                .WithMockServices()
                .WithMockUrlHelper(urlHelper);

            authenticationService =
                (IAuthenticationService?)controller.HttpContext.RequestServices.GetService(
                    typeof(IAuthenticationService)
                );

            controllerWithAuthenticatedUser = new LoginController(
                    loginService,
                    sessionService,
                    logger,
                    userService,
                    clockUtility,
                    configDataService,
                    config,
                    apiClient
                )
                .WithDefaultContext()
                .WithMockUser(true)
                .WithMockTempData()
                .WithMockServices()
                .WithMockUrlHelper(urlHelper);

            authenticationServiceWithAuthenticatedUser =
                (IAuthenticationService?)controllerWithAuthenticatedUser.HttpContext.RequestServices.GetService(
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
                .WithControllerName("LinkAccount").WithActionName("Index");
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
                .WithControllerName("LinkAccount").WithActionName("Index");
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
                    () => authenticationService!.SignInAsync(
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
        public async Task Login_to_unclaimed_delegate_account_should_render_basic_form_with_error()
        {
            // Given
            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.UnclaimedDelegateAccount)
            );

            // When
            var result = await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("Index").ModelAs<LoginViewModel>();
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [Test]
        public async Task Unverified_email_should_redirect_to_Verify_Email_page()
        {
            // Given
            var userEntity = GetUserEntity(true, true);

            A.CallTo(() => loginService.AttemptLogin(A<string>._, A<string>._)).Returns(
                new LoginResult(LoginAttemptResult.UnverifiedEmail, userEntity)
            );

            // When
            var result = await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("VerifyYourEmail")
                .WithActionName("Index");
        }

        [Test]
        public async Task Multiple_available_centres_should_redirect_to_ChooseACentre_page()
        {
            // Given
            var userEntity = GetUserEntityWithTwoDelegateAccounts(true, true);

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
                .WithControllerName("LinkAccount").WithActionName("Index");
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

            GivenAUserEntityWithAdminAndDelegateAccountsAndVerifiedEmail(userEntity);

            // When
            var result = controllerWithAuthenticatedUser.ChooseACentre(DlsSubApplication.Default, returnUrl);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult().WithViewName("ChooseACentre");

                result.As<ViewResult>().Model.As<ChooseACentreViewModel>().ReturnUrl.Should().Be(returnUrl);

                A.CallTo(
                        () => loginService.GetChooseACentreAccountViewModels(
                            userEntity,
                            A<List<int>>._
                        )
                    )
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

            GivenAUserEntityWithAdminAndDelegateAccountsAndVerifiedEmail(userEntity);

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

            GivenAUserEntityWithAdminAndDelegateAccountsAndVerifiedEmail(userEntity);

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
                result.Should().BeRedirectToActionResult().WithControllerName("LinkAccount")
                    .WithActionName("Index");
            }
        }

        [Test]
        public async Task ChooseCentre_should_redirect_to_verify_email_page_if_centre_email_is_unverified()
        {
            // Given
            const int centreId = 2;
            var userEntity = GetUserEntity(true, true, centreId: centreId);

            A.CallTo(() => authenticationServiceWithAuthenticatedUser!.AuthenticateAsync(A<HttpContext>._, A<string>._))
                .Returns(GetAuthenticateResult());

            A.CallTo(() => userService.GetUserById(A<int>._)).Returns(userEntity ?? GetUserEntity(true, true));

            A.CallTo(() => loginService.CentreEmailIsVerified(userEntity!.UserAccount.Id, centreId)).Returns(false);

            // When
            var result = await controllerWithAuthenticatedUser.ChooseCentre(centreId, null);

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("VerifyYourEmail")
                .WithActionName("Index")
                .WithRouteValue("emailVerificationReason", EmailVerificationReason.EmailNotVerified);
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

            GivenAUserEntityWithAdminAndDelegateAccountsAndVerifiedEmail(userEntity);

            // When
            await controllerWithAuthenticatedUser.ChooseCentre(centreId, null);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => authenticationServiceWithAuthenticatedUser!.SignOutAsync(
                        A<HttpContext>._,
                        A<string>._,
                        A<AuthenticationProperties>._
                    )
                ).MustHaveHappened();

                A.CallTo(
                    () => authenticationServiceWithAuthenticatedUser!.SignInAsync(
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
                isAdminAccountActive,
                centreId: centreId
            );

            GivenAUserEntityWithAdminAndDelegateAccountsAndVerifiedEmail(userEntity);

            // When
            try
            {
                await controllerWithAuthenticatedUser.ChooseCentre(centreId, null);
            }
            catch (Exception)
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
            GivenAUserEntityWithAdminAndDelegateAccountsAndVerifiedEmail();

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
                result.Should().BeRedirectToActionResult().WithControllerName("LinkAccount")
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
            GivenAUserEntityWithAdminAndDelegateAccountsAndVerifiedEmail();

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
                    routeValues?.Keys.Should().NotContain("returnUrl");
                    routeValues?.Keys.Should().NotContain("dlsSubApplication");
                }
                else
                {
                    routeValues.Should().Contain("returnUrl", returnUrl);
                    routeValues?.Keys.Should().Contain("dlsSubApplication");
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

            GivenAUserEntityWithAdminAndDelegateAccountsAndVerifiedEmail(authenticateResult: authenticateResult);

            // When
            await controllerWithAuthenticatedUser.ChooseCentre(2, null);

            // Then
            A.CallTo(
                () => authenticationServiceWithAuthenticatedUser!.SignInAsync(
                    A<HttpContext>._,
                    A<string>._,
                    A<ClaimsPrincipal>._,
                    A<AuthenticationProperties>.That.Matches(
                        props => props.IsPersistent == rememberMe
                    )
                )
            ).MustHaveHappened();
        }

        [Test]
        public void SharedAuth_WhenUserIsAuthenticated_ReturnsRedirectToActionResult()
        {
            // Act
            var result = controllerWithAuthenticatedUser.SharedAuth();

            // Assert
            result
                .Should()
                .BeOfType<RedirectToActionResult>();
            result
                .Should()
                .BeRedirectToActionResult()
                .WithControllerName("Home")
                .WithActionName("Index");
        }

        [Test]
        public void SharedAuth_WhenUserIsNotAuthenticated_ReturnsChallengeResult()
        {
            // Act
            var result = controller.SharedAuth();

            // Assert
            result
                .Should()
                .BeOfType<ChallengeResult>();
            result
                .Should()
                .BeChallengeResult()
                .WithRedirectUri("/");
        }

        [Test]
        public void AccountLocked_ReturnsViewResult()
        {
            // Act
            var result = controller.AccountLocked();

            // Assert
            result
                .Should()
                .BeOfType<ViewResult>()
                .Which
                .ViewName
                .Should()
                .Be("AccountLocked");
        }

        [Test]
        public void AccountInactive_ReturnsViewResult()
        {
            // Arrange
            var supportEmail = "support@example.com";
            A.CallTo(() => configDataService
                .GetConfigValue(ConfigDataService.SupportEmail))
                .Returns(supportEmail);

            // Act
            var result = controller.AccountInactive();

            // Assert
            result
                .Should()
                .BeOfType<ViewResult>()
                .Which
                .ViewName
                .Should()
                .Be("AccountInactive");

            var model = result
                .As<ViewResult>()
                .Model
                .As<AccountInactiveViewModel>();
            model
                .SupportEmail
                .Should()
                .Be(supportEmail);
        }

        [Test]
        public void RemoteFailure_ReturnsViewResult()
        {
            // Arrange
            var supportEmail = "support@example.com";
            A.CallTo(() => configDataService
                .GetConfigValue(ConfigDataService.SupportEmail))
                .Returns(supportEmail);

            // Act
            var result = controller.RemoteFailure();

            // Assert
            result
                .Should()
                .BeOfType<ViewResult>()
                .Which
                .ViewName
                .Should()
                .Be("RemoteAuthenticationFailure");

            var model = result
                .As<ViewResult>()
                .Model
                .As<AccountInactiveViewModel>();
            model
                .SupportEmail
                .Should()
                .Be(supportEmail);
        }

        [Test]
        public void NotLinked_ReturnsViewResult()
        {
            // Act
            var result = controller.NotLinked();

            // Assert
            result
                .Should()
                .BeOfType<RedirectToActionResult>();
            result
                .Should()
                .BeRedirectToActionResult()
                .WithControllerName("Logout")
                .WithActionName("LogoutSharedAuth");
        }

        [Test]
        public void ForgottenPassword_ReturnsForgottenPasswordView()
        {
            // Arrange
            // Act
            var result = controller.ForgottenPassword();

            // Assert
            result
                .Should()
                .BeOfType<ViewResult>()
                .Which
                .ViewName
                .Should()
                .Be("ForgottenPassword");

            //var model = result
            //    .As<ViewResult>()
            //    .Model
            //    .As<ForgotPasswordViewModel>();
        }

        [Test]
        public void ForgotPassword_ReturnsMultipleUsersView()
        {
            // Arrange
            var fakeModel = A.Fake<ForgotPasswordViewModel>();

            var apiClient = A.Fake<ILearningHubUserApiClient>();
            A.CallTo(() => apiClient.hasMultipleUsersForEmailAsync(A<string>._)).Returns(true);

            var controller = new LoginController(
                   loginService,
                   sessionService,
                   logger,
                   userService,
                   clockUtility,
                   configDataService,
                   config,
                   apiClient
               )
               .WithDefaultContext()
               .WithMockUser(false)
               .WithMockTempData()
               .WithMockServices()
               .WithMockUrlHelper(urlHelper);

            // Act
            var result = controller.ForgotPassword(fakeModel);

            // Assert
            result.Result
               .Should()
               .BeOfType<ViewResult>()
               .Which
               .ViewName
               .Should()
               .Be("MultipleUsersForEmail");
        }

        [Test]
        public void ForgotPassword_ReturnsForgotPasswordFailure()
        {
            // Arrange
            var fakeModel = A.Fake<ForgotPasswordViewModel>();

            var apiClient = A.Fake<ILearningHubUserApiClient>();
            A.CallTo(() => apiClient.forgotPasswordAsync(A<string>._)).Returns(false);

            var controller = new LoginController(
                   loginService,
                   sessionService,
                   logger,
                   userService,
                   clockUtility,
                   configDataService,
                   config,
                   apiClient
               )
               .WithDefaultContext()
               .WithMockUser(false)
               .WithMockTempData()
               .WithMockServices()
               .WithMockUrlHelper(urlHelper);

            // Act
            var result = controller.ForgotPassword(fakeModel);

            // Assert
            result.Result
               .Should()
               .BeOfType<ViewResult>()
               .Which
               .ViewName
               .Should()
               .Be("ForgotPasswordFailure");
        }

        [Test]
        public void ForgotPassword_ReturnsForgotPasswordAcknowledgement()
        {
            // Arrange
            var fakeModel = A.Fake<ForgotPasswordViewModel>();

            var apiClient = A.Fake<ILearningHubUserApiClient>();
            A.CallTo(() => apiClient.forgotPasswordAsync(A<string>._)).Returns(true);

            var controller = new LoginController(
                   loginService,
                   sessionService,
                   logger,
                   userService,
                   clockUtility,
                   configDataService,
                   config,
                   apiClient
               )
               .WithDefaultContext()
               .WithMockUser(false)
               .WithMockTempData()
               .WithMockServices()
               .WithMockUrlHelper(urlHelper);

            // Act
            var result = controller.ForgotPassword(fakeModel);

            // Assert
            result.Result
               .Should()
               .BeOfType<ViewResult>()
               .Which
               .ViewName
               .Should()
               .Be("ForgotPasswordAcknowledgement");
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

        private UserEntity GetUserEntityWithTwoDelegateAccounts(
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

            var delegateAccount1 = UserTestHelper.GetDefaultDelegateAccount(
                centreId: centreId,
                centreActive: isCentreActive,
                active: isDelegateAccountActive,
                approved: isDelegateAccountApproved
            );

            var delegateAccount2 = UserTestHelper.GetDefaultDelegateAccount(
                centreId: 3,
                centreActive: isCentreActive,
                active: isDelegateAccountActive,
                approved: isDelegateAccountApproved
            );

            return new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                withAdminAccount ? new List<AdminAccount> { adminAccount } : new List<AdminAccount>(),
                withDelegateAccount ? new List<DelegateAccount> { delegateAccount1, delegateAccount2 } : new List<DelegateAccount>()
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

        private void GivenAUserEntityWithAdminAndDelegateAccountsAndVerifiedEmail(
            UserEntity? userEntity = null,
            AuthenticateResult? authenticateResult = null
        )
        {
            A.CallTo(() => authenticationServiceWithAuthenticatedUser!.AuthenticateAsync(A<HttpContext>._, A<string>._))
                .Returns(authenticateResult ?? GetAuthenticateResult());

            A.CallTo(() => userService.GetUserById(A<int>._)).Returns(userEntity ?? GetUserEntity(true, true));

            A.CallTo(() => loginService.CentreEmailIsVerified(A<int>._, A<int>._)).Returns(true);
        }
    }
}
