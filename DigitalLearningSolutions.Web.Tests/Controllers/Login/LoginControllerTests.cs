namespace DigitalLearningSolutions.Web.Tests.Controllers.Login
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Login;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Common;
    using NUnit.Framework;

    internal class LoginControllerTests
    {
        private LoginController controller;
        private ILoginService loginService;
        private IUserService userService;
        private ISessionService sessionService;

        [SetUp]
        public void SetUp()
        {
            loginService = A.Fake<ILoginService>();
            userService = A.Fake<IUserService>();
            sessionService = A.Fake<ISessionService>();

            controller = LoginTestHelper.GetLoginControllerWithUnauthenticatedUser(loginService, userService, sessionService);
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
                LoginTestHelper.GetLoginControllerWithAuthenticatedUser(loginService, userService, sessionService);

            // When
            var result = controllerWithAuthenticatedUser.Index();

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public void Successful_sign_in_should_render_home_page()
        {
            controller = LoginTestHelper.GetLoginControllerWithSignInFunctionality(loginService, userService, sessionService);

            //Given
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((UserTestHelper.GetDefaultAdminUser(),
                    new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() }));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((UserTestHelper.GetDefaultAdminUser(),
                    new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() }));
            A.CallTo(() => userService.GetUserCentres(A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns(
                    new List<CentreUserDetails> { new CentreUserDetails(1, "Centre 1", true, true) });

            // When
            var result = controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public void Log_in_request_should_call_login_service()
        {
            controller = LoginTestHelper.GetLoginControllerWithSignInFunctionality(loginService, userService, sessionService);

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
            controller = LoginTestHelper.GetLoginControllerWithSignInFunctionality(loginService, userService, sessionService);
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
            A.CallTo(() => userService.GetUserCentres(A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns(new List<CentreUserDetails>
                {
                    new CentreUserDetails(1, "Centre 1", true),
                    new CentreUserDetails(2, "Centre 2", false, true)
                });

            // When
            var result =
                controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeViewResult().WithViewName("ChooseACentre");
        }

        [Test]
        public void
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
          
            // When
            controller.Index(LoginTestHelper.GetDefaultLoginViewModel());
            A.CallTo(() => userService.GetUserCentres(
                    A<AdminUser>.That.IsEqualTo(expectedAdminUser),
                    A<List<DelegateUser>>.That.IsEmpty()))
              .MustHaveHappened();
        }

        [Test]
        public void Log_in_as_admin_records_admin_session()
        {
            // Given
            controller = LoginTestHelper.GetLoginControllerWithSignInFunctionality(loginService, userService, sessionService);
            var expectedAdmin = UserTestHelper.GetDefaultAdminUser(10);
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((expectedAdmin, new List<DelegateUser>()));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((expectedAdmin, new List<DelegateUser>()));
            A.CallTo(() => userService.GetUserCentres(A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns(
                    new List<CentreUserDetails> { new CentreUserDetails(expectedAdmin.CentreId, expectedAdmin.CentreName, true) });


            // When
            controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            A.CallTo(() => sessionService.StartAdminSession(expectedAdmin.Id))
                .MustHaveHappened();
        }

        [Test]
        public void Log_in_as_delegate_does_not_record_admin_session()
        {
            // Given
            controller = LoginTestHelper.GetLoginControllerWithSignInFunctionality(loginService, userService, sessionService);
            var expectedDelegates = new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser(approved: true) };
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((null, expectedDelegates));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((null, expectedDelegates));
            A.CallTo(() => loginService.GetVerifiedAdminUserAssociatedWithDelegateUser(A<DelegateUser>._, A<string>._))
                .Returns(null);
  
              // When
            controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
              A.CallTo(() => sessionService.StartAdminSession(A<int>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void
            When_user_has_accounts_with_different_approved_statuses_only_check_for_centres_on_approved_accounts()
        {
            // Given
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
            var expectedApprovedDelegateUsers = expectedDelegateUsers.Where(du => du.Approved).ToList();
  
            // When
            controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            A.CallTo(() => userService.GetUserCentres(
                    A<AdminUser>.That.IsEqualTo(expectedAdminUser),
                    A<List<DelegateUser>>.That.Matches(list => list.SequenceEqual(expectedApprovedDelegateUsers))))
                .MustHaveHappened();
        }

        [Test]
        public void Multiple_approved_accounts_at_same_centre_should_log_in()
        {
            // Given
            controller = LoginTestHelper.GetLoginControllerWithSignInFunctionality(loginService, userService, sessionService);
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser(centreId: 1, centreName: "Centre 1");
            var expectedDelegateUsers = new List<DelegateUser>
                { UserTestHelper.GetDefaultDelegateUser(centreId: 1, centreName: "Centre 1", approved: true) };
            A.CallTo(() => userService.GetUsersByUsername(A<string>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((expectedAdminUser, expectedDelegateUsers));
            A.CallTo(() => userService.GetUserCentres(A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns(
                    new List<CentreUserDetails> { new CentreUserDetails(1, "Centre 1", true, true) });

            // When
            var result =
                controller.Index(LoginTestHelper.GetDefaultLoginViewModel());

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }
    }
}
