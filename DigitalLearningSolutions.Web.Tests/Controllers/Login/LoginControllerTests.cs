namespace DigitalLearningSolutions.Web.Tests.Controllers.Login
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers;
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
        private IUserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            loginService = A.Fake<ILoginService>();
            userService = A.Fake<IUserService>();
            sessionService = A.Fake<ISessionService>();
            logger = A.Fake<ILogger<LoginController>>();

            controller = new LoginController(loginService, userService, sessionService, logger)
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
            var controllerWithAuthenticatedUser = new LoginController(loginService, userService, sessionService, logger)
                .WithDefaultContext()
                .WithMockUser(true);

            // When
            var result = controllerWithAuthenticatedUser.Index();

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
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
        public async Task Log_in_request_should_call_login_service()
        {
            // Given
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns(
                    (UserTestHelper.GetDefaultAdminUser(),
                        new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() })
                );
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns(
                    (UserTestHelper.GetDefaultAdminUser(),
                        new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() })
                );

            // When
            await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .MustHaveHappened();
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .MustHaveHappened();
        }

        [Test]
        public async Task No_user_account_found_should_render_basic_form_with_error()
        {
            // Given
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((null, new List<DelegateUser>()));

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
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((UserTestHelper.GetDefaultAdminUser(), new List<DelegateUser>()));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((null, new List<DelegateUser>()));

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
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns(
                    (UserTestHelper.GetDefaultAdminUser(),
                        new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() })
                );
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((null, new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser(approved: false) }));

            // When
            var result = await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("AccountNotApproved");
        }

        [Test]
        public async Task Log_in_with_approved_delegate_id_fetches_associated_admin_user()
        {
            // Given
            var testDelegate = UserTestHelper.GetDefaultDelegateUser(emailAddress: "TestAccountAssociation@email.com");
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((null, new List<DelegateUser> { testDelegate }));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((null, new List<DelegateUser> { testDelegate }));

            // When
            await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            A.CallTo(() => loginService.GetVerifiedAdminUserAssociatedWithDelegateUser(testDelegate, A<string>._))
                .MustHaveHappened();
        }

        [Test]
        public async Task Multiple_available_centres_should_redirect_to_ChooseACentre_page()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser(centreId: 1, centreName: "Centre 1");
            var expectedDelegateUsers = new List<DelegateUser>
                { UserTestHelper.GetDefaultDelegateUser(centreId: 2, centreName: "Centre 2") };
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => userService.GetUserCentres(A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns(
                    new List<CentreUserDetails>
                    {
                        new CentreUserDetails(1, "Centre 1", true),
                        new CentreUserDetails(2, "Centre 2", false, true)
                    }
                );

            // When
            var result = await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("ChooseACentre");
        }

        [Test]
        public async Task
            When_user_has_multiple_accounts_with_different_passwords_only_use_ones_matching_input_password_to_check_for_centres()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser(centreId: 1, centreName: "Centre 1");
            var expectedDelegateUsers = new List<DelegateUser>
                { UserTestHelper.GetDefaultDelegateUser(centreId: 2, centreName: "Centre 2") };
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((expectedAdminUser, new List<DelegateUser>()));
            A.CallTo(() => userService.GetUsersWithActiveCentres(A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((expectedAdminUser, new List<DelegateUser>()));

            // When
            await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());
            A.CallTo(
                    () => userService.GetUserCentres(
                        A<AdminUser>.That.IsEqualTo(expectedAdminUser),
                        A<List<DelegateUser>>.That.IsEmpty()
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public async Task Log_in_as_admin_records_admin_session()
        {
            // Given
            var expectedAdmin = UserTestHelper.GetDefaultAdminUser(10);
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((expectedAdmin, new List<DelegateUser>()));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((expectedAdmin, new List<DelegateUser>()));
            A.CallTo(() => userService.GetUsersWithActiveCentres(A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((expectedAdmin, new List<DelegateUser>()));
            A.CallTo(() => userService.GetUserCentres(A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns(
                    new List<CentreUserDetails>
                        { new CentreUserDetails(expectedAdmin.CentreId, expectedAdmin.CentreName, true) }
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
            var expectedDelegates = new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser(approved: true) };
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((null, expectedDelegates));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((null, expectedDelegates));
            A.CallTo(() => loginService.GetVerifiedAdminUserAssociatedWithDelegateUser(A<DelegateUser>._, A<string>._))
                .Returns(null);

            // When
            await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            A.CallTo(() => sessionService.StartAdminSession(A<int>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task
            When_user_has_accounts_with_different_approved_statuses_only_check_for_centres_on_approved_accounts()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser(centreId: 1, centreName: "Centre 1");
            var expectedDelegateUsers = new List<DelegateUser>
            {
                UserTestHelper.GetDefaultDelegateUser(centreId: 1, centreName: "Centre 1", approved: true),
                UserTestHelper.GetDefaultDelegateUser(centreId: 2, centreName: "Centre 2", approved: false)
            };
            var expectedApprovedDelegateUsers = expectedDelegateUsers.Where(du => du.Approved).ToList();
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => userService.GetUsersWithActiveCentres(A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((expectedAdminUser, expectedApprovedDelegateUsers));

            // When
            await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            A.CallTo(
                    () => userService.GetUserCentres(
                        A<AdminUser>.That.IsEqualTo(expectedAdminUser),
                        A<List<DelegateUser>>.That.IsSameSequenceAs(expectedApprovedDelegateUsers)
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public async Task Multiple_approved_accounts_at_same_centre_should_log_in()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser(centreId: 1, centreName: "Centre 1");
            var expectedDelegateUsers = new List<DelegateUser>
                { UserTestHelper.GetDefaultDelegateUser(centreId: 1, centreName: "Centre 1", approved: true) };
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => userService.GetUsersWithActiveCentres(A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => userService.GetUserCentres(A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns(
                    new List<CentreUserDetails> { new CentreUserDetails(1, "Centre 1", true, true) }
                );

            // When
            var result = await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public async Task
            When_user_has_accounts_with_inactive_centres_only_use_active_centre_details_for_login()
        {
            // Given
            var delegateUserAtActiveCentre =
                UserTestHelper.GetDefaultDelegateUser(centreId: 2, centreName: "Centre 2", centreActive: true);
            var expectedAdminUser =
                UserTestHelper.GetDefaultAdminUser(centreId: 1, centreName: "Centre 1", centreActive: false);
            var expectedDelegateUsers = new List<DelegateUser>
            {
                UserTestHelper.GetDefaultDelegateUser(centreId: 1, centreName: "Centre 1", centreActive: false),
                delegateUserAtActiveCentre
            };
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => userService.GetUsersWithActiveCentres(A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((null, new List<DelegateUser> { delegateUserAtActiveCentre }));

            // When
            await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            A.CallTo(
                    () => userService.GetUserCentres(
                        null,
                        A<List<DelegateUser>>.That.IsSameSequenceAs(delegateUserAtActiveCentre)
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public async Task
            When_user_has_verified_accounts_only_at_inactive_centres_then_redirect_to_centre_inactive_page()
        {
            // Given
            var expectedAdminUser =
                UserTestHelper.GetDefaultAdminUser(centreId: 1, centreName: "Centre 1", centreActive: false);
            var expectedDelegateUsers = new List<DelegateUser>
            {
                UserTestHelper.GetDefaultDelegateUser(centreId: 1, centreName: "Centre 1", centreActive: false)
            };
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => userService.GetUserCentres(expectedAdminUser, expectedDelegateUsers))
                .Returns(new List<CentreUserDetails>());

            // When
            var result = await controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("CentreInactive");
        }

        [Test]
        public async Task User_without_email_address_can_still_login()
        {
            // Given
            var delegates = new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser(emailAddress: null) };
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((null, delegates));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((null, delegates));
            A.CallTo(() => userService.GetUsersWithActiveCentres(A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((null, delegates));
            A.CallTo(() => userService.GetUserCentres(A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns(
                    new List<CentreUserDetails> { new CentreUserDetails(1, "Centre 1", false, true) }
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
            // When
            await controller.Index(LoginTestHelper.GetDefaultLoginViewModel("\ttest@example.com "));

            // Then
            A.CallTo(() => userService.GetUsersByUsername("test@example.com")).MustHaveHappened(1, Times.Exactly);
        }

        private void GivenSignInIsSuccessful()
        {
            var admin = UserTestHelper.GetDefaultAdminUser();
            var delegates = new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() };

            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((admin, delegates));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((admin, delegates));
            A.CallTo(() => userService.GetUsersWithActiveCentres(A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((admin, delegates));
            A.CallTo(() => userService.GetUserCentres(A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns(
                    new List<CentreUserDetails> { new CentreUserDetails(1, "Centre 1", true, true) }
                );
        }
    }
}
