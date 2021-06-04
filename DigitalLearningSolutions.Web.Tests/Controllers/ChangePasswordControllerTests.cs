namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class ChangePasswordControllerTests
    {
        private ChangePasswordController authenticatedController = null!;
        private ILoginService loginService = null!;
        private IPasswordService passwordService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            userService = A.Fake<IUserService>();
            loginService = A.Fake<ILoginService>();
            passwordService = A.Fake<IPasswordService>();
            authenticatedController = CreateNewChangePasswordControllerWithDefaultContext()
                .WithMockUser(isAuthenticated: true);
        }

        [Test]
        public async Task Post_returns_form_if_model_invalid()
        {
            // Given
            authenticatedController.ModelState.AddModelError("key", "Invalid for testing.");

            // When
            var result = await authenticatedController.Index(new ChangePasswordViewModel());

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public async Task Post_returns_form_if_current_password_does_not_match_email()
        {
            // Given
            GivenPasswordVerificationFails();

            // When
            var result = await authenticatedController.Index(new ChangePasswordViewModel());

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<ChangePasswordViewModel>();
        }

        [Test]
        public async Task Post_returns_form_if_current_password_does_not_match_user_ids()
        {
            // Given
            var controllerWithoutEmail = CreateNewChangePasswordControllerWithDefaultContext()
                .WithMockUser(true, emailAddress: null, adminId: 54, delegateId: 209);
            GivenPasswordVerificationFails();

            // When
            var result = await controllerWithoutEmail.Index(new ChangePasswordViewModel());

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<ChangePasswordViewModel>();
        }

        [Test]
        public async Task Post_does_not_change_password_if_current_password_does_not_match_email()
        {
            // Given
            GivenPasswordVerificationFails();

            // When
            await authenticatedController.Index(new ChangePasswordViewModel());

            // Then
            A.CallTo(() => passwordService.ChangePasswordAsync(A<UserReference>._, A<string>._)).MustNotHaveHappened();
            A.CallTo(() => passwordService.ChangePasswordAsync(A<string>._, A<string>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Post_does_not_change_password_if_current_password_does_not_match_user_ids()
        {
            // Given
            var controllerWithoutEmail = CreateNewChangePasswordControllerWithDefaultContext()
                .WithMockUser(true, emailAddress: null, adminId: 54, delegateId: 209);
            GivenPasswordVerificationFails();

            // When
            await controllerWithoutEmail.Index(new ChangePasswordViewModel());

            // Then
            A.CallTo(() => passwordService.ChangePasswordAsync(A<UserReference>._, A<string>._)).MustNotHaveHappened();
            A.CallTo(() => passwordService.ChangePasswordAsync(A<string>._, A<string>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Post_returns_success_page_if_model_and_password_valid()
        {
            // Given
            GivenPasswordVerificationSucceedsFor(authenticatedController.User.GetEmailIfAny()!, "current-password");

            // When
            var result = await authenticatedController.Index(
                new ChangePasswordViewModel
                    { Password = "new-password", ConfirmPassword = "new-password", CurrentPassword = "current-password" }
            );

            // Then
            result.Should().BeViewResult().WithViewName("Success");
        }

        [Test]
        public async Task Post_changes_password_if_model_and_password_valid()
        {
            // Given
            GivenPasswordVerificationSucceedsFor(authenticatedController.User.GetEmailIfAny()!, "current-password");

            // When
            await authenticatedController.Index(
                new ChangePasswordViewModel
                    { Password = "new-password", ConfirmPassword = "new-password", CurrentPassword = "current-password" }
            );

            A.CallTo(() => passwordService.ChangePasswordAsync(authenticatedController.User.GetEmailIfAny()!, "new-password"))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => passwordService.ChangePasswordAsync(A<UserReference>._, A<string>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Post_changes_password_for_logged_in_user_if_no_email_address_and_admin_only()
        {
            // Given
            GivenPasswordVerificationSucceedsFor(null, 209, "current-password");
            var controller = CreateNewChangePasswordControllerWithDefaultContext()
                .WithMockUser(true, emailAddress: null, adminId: null, delegateId: 209);

            // When
            await controller.Index(
                new ChangePasswordViewModel
                    { Password = "new-password", ConfirmPassword = "new-password", CurrentPassword = "current-password" }
            );

            // Then
            A.CallTo(
                () => passwordService.ChangePasswordAsync(new UserReference(209, UserType.DelegateUser), "new-password")
            ).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => passwordService.ChangePasswordAsync(authenticatedController.User.GetEmailIfAny()!, "new-password"))
                .MustNotHaveHappened();
        }

        private ChangePasswordController CreateNewChangePasswordControllerWithDefaultContext()
        {
            return new ChangePasswordController(passwordService, loginService, userService).WithDefaultContext();
        }

        [Test]
        public async Task Post_changes_password_for_logged_in_user_if_no_email_address_and_delegate_only()
        {
            // Given
            GivenPasswordVerificationSucceedsFor(52, null, "current-password");
            var controller = CreateNewChangePasswordControllerWithDefaultContext()
                .WithMockUser(true, emailAddress: null, adminId: 52, delegateId: null);

            // When
            await controller.Index(
                new ChangePasswordViewModel
                    { Password = "new-password", ConfirmPassword = "new-password", CurrentPassword = "current-password" }
            );

            // Then
            A.CallTo(
                () => passwordService.ChangePasswordAsync(new UserReference(52, UserType.AdminUser), "new-password")
            ).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => passwordService.ChangePasswordAsync(authenticatedController.User.GetEmailIfAny()!, "new-password"))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Post_changes_password_for_logged_in_user_if_no_email_address_and_both_admin_and_delegate()
        {
            // Given
            GivenPasswordVerificationSucceedsFor(52, 209, "current-password");
            var controller = CreateNewChangePasswordControllerWithDefaultContext()
                .WithMockUser(true, emailAddress: null, adminId: 52, delegateId: 209);

            // When
            await controller.Index(
                new ChangePasswordViewModel
                    { Password = "new-password", ConfirmPassword = "new-password", CurrentPassword = "current-password" }
            );

            // Then
            A.CallTo(
                () => passwordService.ChangePasswordAsync(new UserReference(52, UserType.AdminUser), "new-password")
            ).MustHaveHappened(1, Times.Exactly);
            A.CallTo(
                () => passwordService.ChangePasswordAsync(new UserReference(209, UserType.DelegateUser), "new-password")
            ).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => passwordService.ChangePasswordAsync(A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        private void GivenPasswordVerificationSucceedsFor(string email, string password)
        {
            A.CallTo(() => userService.GetVerifiedLinkedUsersAccounts(email, password))
                .Returns((Builder<AdminUser>.CreateNew().Build(), new List<DelegateUser>()));
        }

        private void GivenPasswordVerificationSucceedsFor(int? adminId, int? candidateId, string password)
        {
            A.CallTo(() => userService.GetVerifiedLinkedUsersAccounts(adminId, candidateId, password))
                .Returns((Builder<AdminUser>.CreateNew().Build(), new List<DelegateUser>()));
        }

        private void GivenPasswordVerificationFails()
        {
            A.CallTo(() => userService.GetVerifiedLinkedUsersAccounts(A<string>._, A<string>._))
                .Returns((null, new List<DelegateUser>()));
            A.CallTo(() => userService.GetVerifiedLinkedUsersAccounts(A<int>._, A<int>._, A<string>._))
                .Returns((null, new List<DelegateUser>()));
        }
    }
}
