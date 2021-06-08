namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class ChangePasswordControllerTests
    {
        private const int AdminId = 34;
        private const int DelegateId = 12;

        private ChangePasswordController authenticatedController = null!;
        private IPasswordService passwordService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            userService = A.Fake<IUserService>();
            passwordService = A.Fake<IPasswordService>();
            authenticatedController = new ChangePasswordController(passwordService, userService)
                .WithDefaultContext()
                .WithMockUser(isAuthenticated: true, adminId: AdminId, delegateId: DelegateId);
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
        public async Task Post_returns_form_if_current_password_does_not_match_user_ids()
        {
            // Given
            GivenPasswordVerificationFails();

            // When
            var result = await authenticatedController.Index(new ChangePasswordViewModel());

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<ChangePasswordViewModel>();
        }

        [Test]
        public async Task Post_does_not_change_password_if_current_password_does_not_match_user_ids()
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
        public async Task Post_returns_success_page_if_model_and_password_valid()
        {
            // Given
            GivenPasswordVerificationSucceedsForLoggedInUser("current-password");

            // When
            var result = await authenticatedController.Index(
                new ChangePasswordViewModel
                {
                    Password = "new-password", ConfirmPassword = "new-password", CurrentPassword = "current-password"
                }
            );

            // Then
            result.Should().BeViewResult().WithViewName("Success");
        }

        [Test]
        public async Task Post_changes_password_if_model_and_password_valid()
        {
            // Given
            GivenPasswordVerificationSucceedsForLoggedInUser("current-password");
            var adminUser = Builder<AdminUser>.CreateNew().Build();
            var delegateUser = Builder<DelegateUser>.CreateNew().Build();
            A.CallTo(() => userService.GetUsersById(AdminId, DelegateId)).Returns((adminUser, delegateUser));

            // When
            await authenticatedController.Index(
                new ChangePasswordViewModel
                {
                    Password = "new-password", ConfirmPassword = "new-password", CurrentPassword = "current-password"
                }
            );

            A.CallTo(() => passwordService.ChangePasswordForLinkedUserAccounts(adminUser, delegateUser, "new-password"))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public async Task Post_changes_password_if_admin_only()
        {
            // Given
            GivenPasswordVerificationSucceedsForLoggedInUser("current-password");
            var adminUser = Builder<AdminUser>.CreateNew().Build();
            A.CallTo(() => userService.GetUsersById(AdminId, DelegateId)).Returns((adminUser, null));

            // When
            await authenticatedController.Index(
                new ChangePasswordViewModel
                {
                    Password = "new-password", ConfirmPassword = "new-password", CurrentPassword = "current-password"
                }
            );

            A.CallTo(() => passwordService.ChangePasswordForLinkedUserAccounts(adminUser, null, "new-password"))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public async Task Post_changes_password_if_delegate_only()
        {
            // Given
            GivenPasswordVerificationSucceedsForLoggedInUser("current-password");
            var delegateUser = Builder<DelegateUser>.CreateNew().Build();
            A.CallTo(() => userService.GetUsersById(AdminId, DelegateId)).Returns((null, delegateUser));

            // When
            await authenticatedController.Index(
                new ChangePasswordViewModel
                {
                    Password = "new-password", ConfirmPassword = "new-password", CurrentPassword = "current-password"
                }
            );

            A.CallTo(() => passwordService.ChangePasswordForLinkedUserAccounts(null, delegateUser, "new-password"))
                .MustHaveHappened(1, Times.Exactly);
        }

        private void GivenPasswordVerificationSucceedsForLoggedInUser(string password)
        {
            A.CallTo(() => userService.GetVerifiedLinkedUsersAccounts(AdminId, DelegateId, password))
                .Returns((Builder<AdminUser>.CreateNew().Build(), new List<DelegateUser>()));
        }

        private void GivenPasswordVerificationFails()
        {
            A.CallTo(() => userService.GetVerifiedLinkedUsersAccounts(A<int>._, A<int>._, A<string>._))
                .Returns((null, new List<DelegateUser>()));
        }
    }
}
