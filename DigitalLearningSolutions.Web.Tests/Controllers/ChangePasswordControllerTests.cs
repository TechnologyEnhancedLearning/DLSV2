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

        [SetUp]
        public void SetUp()
        {
            loginService = A.Fake<ILoginService>();
            passwordService = A.Fake<IPasswordService>();
            authenticatedController = new ChangePasswordController(passwordService).WithDefaultContext().WithMockUser
                (isAuthenticated: true);
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
        public async Task Post_returns_form_if_current_password_incorrect()
        {
            // Given
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._)).Returns
                ((null, new List<DelegateUser>()));
            var model = new ChangePasswordViewModel();

            // When
            var result = await authenticatedController.Index(model);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<ChangePasswordViewModel>();
        }

        [Test]
        public async Task Post_returns_success_page_if_model_and_password_valid()
        {
            // Given
            var user = Builder<AdminUser>.CreateNew().Build();
            A.CallTo(() => loginService.VerifyUsers("password", A<AdminUser>._, A<List<DelegateUser>>._)).Returns
                ((user, new List<DelegateUser>()));

            // When
            var result = await authenticatedController.Index(
                new ChangePasswordViewModel
                    { Password = "password", ConfirmPassword = "password", CurrentPassword = "current-password" }
            );

            // Then
            result.Should().BeViewResult().WithViewName("Success");
        }

        [Test]
        public async Task Post_changes_password_if_model_and_password_valid()
        {
            // Given
            A.CallTo(() => loginService.VerifyUsers("password", A<AdminUser>._, A<List<DelegateUser>>._)).Returns
                ((Builder<AdminUser>.CreateNew().Build(), new List<DelegateUser>()));

            // When
            await authenticatedController.Index(
                new ChangePasswordViewModel
                    { Password = "password", ConfirmPassword = "password", CurrentPassword = "current-password" }
            );

            A.CallTo(() => passwordService.ChangePasswordAsync(authenticatedController.User.GetEmail()!, "password"))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => passwordService.ChangePasswordAsync(A<UserReference>._, "password")).MustNotHaveHappened();
        }

        [Test]
        public async Task Post_changes_password_for_logged_in_user_if_no_email_address_and_admin_only()
        {
            // Given
            A.CallTo(() => loginService.VerifyUsers("password", A<AdminUser>._, A<List<DelegateUser>>._)).Returns
                ((Builder<AdminUser>.CreateNew().Build(), new List<DelegateUser>()));
            var controller = new ChangePasswordController(passwordService).WithDefaultContext()
                .WithMockUser(true, emailAddress: null, adminId: null, delegateId: 209);

            // When
            await controller.Index(
                new ChangePasswordViewModel
                    { Password = "password", ConfirmPassword = "password", CurrentPassword = "current-password" }
            );

            // Then
            A.CallTo(
                () => passwordService.ChangePasswordAsync(new UserReference(209, UserType.DelegateUser), "password")
            ).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => passwordService.ChangePasswordAsync(authenticatedController.User.GetEmail()!, "password"))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Post_changes_password_for_logged_in_user_if_no_email_address_and_delegate_only()
        {
            // Given
            A.CallTo(() => loginService.VerifyUsers("password", A<AdminUser>._, A<List<DelegateUser>>._)).Returns
                ((Builder<AdminUser>.CreateNew().Build(), new List<DelegateUser>()));
            var controller = new ChangePasswordController(passwordService).WithDefaultContext()
                .WithMockUser(true, emailAddress: null, adminId: 52, delegateId: null);

            // When
            await controller.Index(
                new ChangePasswordViewModel
                    { Password = "password", ConfirmPassword = "password", CurrentPassword = "current-password" }
            );

            // Then
            A.CallTo(
                () => passwordService.ChangePasswordAsync(new UserReference(52, UserType.AdminUser), "password")
            ).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => passwordService.ChangePasswordAsync(authenticatedController.User.GetEmail()!, "password"))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Post_changes_password_for_logged_in_user_if_no_email_address_and_both_admin_and_delegate()
        {
            // Given
            A.CallTo(() => loginService.VerifyUsers("password", A<AdminUser>._, A<List<DelegateUser>>._)).Returns
                ((Builder<AdminUser>.CreateNew().Build(), new List<DelegateUser>()));
            var controller = new ChangePasswordController(passwordService).WithDefaultContext()
                .WithMockUser(true, emailAddress: null, adminId: 52, delegateId: 209);

            // When
            await controller.Index(
                new ChangePasswordViewModel
                    { Password = "password", ConfirmPassword = "password", CurrentPassword = "current-password" }
            );

            // Then
            A.CallTo(
                () => passwordService.ChangePasswordAsync(new UserReference(52, UserType.AdminUser), "password")
            ).MustHaveHappened(1, Times.Exactly);
            A.CallTo(
                () => passwordService.ChangePasswordAsync(new UserReference(209, UserType.DelegateUser), "password")
            ).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => passwordService.ChangePasswordAsync(authenticatedController.User.GetEmail()!, "password"))
                .MustNotHaveHappened();
        }
    }
}
