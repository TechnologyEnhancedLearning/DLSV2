﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.Login
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
        private ILoginService loginService;
        private IUserService userService;

        [SetUp]
        public void SetUp()
        {
            loginService = A.Fake<ILoginService>();
            userService = A.Fake<IUserService>();

            controller = LoginTestHelper.GetLoginControllerWithUnauthenticatedUser(loginService, userService);
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
            var controllerWithAuthenticatedUser =
                LoginTestHelper.GetLoginControllerWithAuthenticatedUser(loginService, userService);

            // When
            var result = controllerWithAuthenticatedUser.Index();

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public void Successful_sign_in_should_render_home_page()
        {
            controller = LoginTestHelper.GetLoginControllerWithSignInFunctionality(loginService, userService);

            //Given
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((UserTestHelper.GetDefaultAdminUser(),
                    new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() }));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((UserTestHelper.GetDefaultAdminUser(),
                    new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() }));

            // When
            var result = controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public void Log_in_request_should_call_login_service()
        {
            controller = LoginTestHelper.GetLoginControllerWithSignInFunctionality(loginService, userService);

            //Given
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((UserTestHelper.GetDefaultAdminUser(),
                    new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() }));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((UserTestHelper.GetDefaultAdminUser(),
                    new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() }));

            // When
            controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .MustHaveHappened();
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .MustHaveHappened();
        }

        [Test]
        public void No_user_account_found_should_render_basic_form_with_error()
        {
            // Given
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((null, new List<DelegateUser>()));

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
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((UserTestHelper.GetDefaultAdminUser(), new List<DelegateUser>()));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((null, new List<DelegateUser>()));

            // When
            var result =
                controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("Index").ModelAs<LoginViewModel>();
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [Test]
        public void Unapproved_delegate_account_redirects_to_not_approved_page()
        {
            // Given
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((UserTestHelper.GetDefaultAdminUser(),
                    new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() }));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((null, new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser(approved: false) }));

            // When
            var result =
                controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("AccountNotApproved");
        }

        [Test]
        public void Log_in_with_approved_delegate_id_fetches_associated_admin_user()
        {
            // Given
            controller = LoginTestHelper.GetLoginControllerWithSignInFunctionality(loginService, userService);
            var testDelegate = UserTestHelper.GetDefaultDelegateUser(emailAddress: "TestAccountAssociation@email.com");
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((null, new List<DelegateUser> { testDelegate }));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((null, new List<DelegateUser> { testDelegate }));

            // When
            controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            A.CallTo(() => loginService.GetVerifiedAdminUserAssociatedWithDelegateUser(testDelegate, A<string>._))
                .MustHaveHappened();
        }

        [Test]
        public void Multiple_available_centres_should_redirect_to_ChooseACentre_page()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser(centreId: 1, centreName: "Centre 1");
            var expectedDelegateUsers = new List<DelegateUser>
                { UserTestHelper.GetDefaultDelegateUser(centreId: 2, centreName: "Centre 2") };
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));

            // When
            var result =
                controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("ChooseACentre");
        }

        [Test]
        public void Multiple_accounts_with_different_passwords_should_use_ones_with_input_password()
        {
            // Given
            controller = LoginTestHelper.GetLoginControllerWithSignInFunctionality(loginService, userService);
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser(centreId: 1, centreName: "Centre 1");
            var expectedDelegateUsers = new List<DelegateUser>
                { UserTestHelper.GetDefaultDelegateUser(centreId: 2, centreName: "Centre 2") };
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((expectedAdminUser, new List<DelegateUser>()));

            // When
            var result =
                controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public void Multiple_accounts_with_different_approved_statuses_should_use_accounts_with_approved_statuses()
        {
            // Given
            controller = LoginTestHelper.GetLoginControllerWithSignInFunctionality(loginService, userService);
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser(centreId: 1, centreName: "Centre 1");
            var expectedDelegateUsers = new List<DelegateUser>
            {
                UserTestHelper.GetDefaultDelegateUser(centreId: 1, centreName: "Centre 1", approved: true),
                UserTestHelper.GetDefaultDelegateUser(centreId: 2, centreName: "Centre 2", approved: false)
            };
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));

            // When
            var result =
                controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public void Multiple_approved_accounts_at_same_centre_should_log_in()
        {
            // Given
            controller = LoginTestHelper.GetLoginControllerWithSignInFunctionality(loginService, userService);
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser(centreId: 1, centreName: "Centre 1");
            var expectedDelegateUsers = new List<DelegateUser>
                { UserTestHelper.GetDefaultDelegateUser(centreId: 1, centreName: "Centre 1", approved: true) };
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));

            // When
            var result =
                controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }
    }
}
