namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class ChangePasswordControllerTests
    {
        private const int LoggedInAdminId = 34;
        private const int LoggedInDelegateId = 12;

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
                .WithMockUser(true, adminId: LoggedInAdminId, delegateId: LoggedInDelegateId);
        }

        [Test]
        public async Task Post_returns_form_if_model_invalid()
        {
            // Given
            authenticatedController.ModelState.AddModelError("key", "Invalid for testing.");

            // When
            var result = await authenticatedController.Index(new ChangePasswordFormData(), DlsSubApplication.Default);

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public async Task Post_returns_form_if_current_password_does_not_match_user_ids()
        {
            // Given
            GivenPasswordVerificationFails();

            // When
            var result = await authenticatedController.Index(new ChangePasswordFormData(), DlsSubApplication.Default);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<ChangePasswordViewModel>();
        }

        [Test]
        public async Task Post_does_not_change_password_if_current_password_does_not_match_user_ids()
        {
            // Given
            GivenPasswordVerificationFails();

            // When
            await authenticatedController.Index(new ChangePasswordFormData(), DlsSubApplication.Default);

            // Then
            ThenMustNotHaveChangedPassword();
        }

        [Test]
        public async Task Post_returns_success_page_if_model_and_password_valid()
        {
            // Given
            var user = Builder<AdminUser>.CreateNew().Build();
            GivenPasswordVerificationReturnsUsers(new UserAccountSet(user, null), "current-password");

            // When
            var result = await authenticatedController.Index(
                new ChangePasswordFormData
                {
                    Password = "new-password",
                    ConfirmPassword = "new-password",
                    CurrentPassword = "current-password"
                },
                DlsSubApplication.Default
            );

            // Then
            result.Should().BeViewResult().WithViewName("Success");
        }

        [Test]
        public async Task Post_changes_password_if_model_and_password_valid()
        {
            // Given
            var adminUser = Builder<AdminUser>.CreateNew().Build();
            var delegateUser = Builder<DelegateUser>.CreateNew().Build();
            GivenLoggedInUserAccountsAre(adminUser, delegateUser);

            var verifiedUsers = new UserAccountSet(adminUser, new[] { delegateUser });
            GivenPasswordVerificationReturnsUsers(verifiedUsers, "current-password");

            // When
            await authenticatedController.Index(
                new ChangePasswordFormData
                {
                    Password = "new-password",
                    ConfirmPassword = "new-password",
                    CurrentPassword = "current-password"
                },
                DlsSubApplication.Default
            );

            ThenMustHaveChangedPasswordForUserRefsOnce("new-password", verifiedUsers.GetUserRefs());
        }

        [Test]
        public async Task Post_changes_password_only_for_verified_users()
        {
            // Given
            var adminUser = Builder<AdminUser>.CreateNew().Build();
            var delegateUser = Builder<DelegateUser>.CreateNew().Build();

            GivenLoggedInUserAccountsAre(adminUser, delegateUser);
            GivenPasswordVerificationReturnsUsers(
                new UserAccountSet(null, new[] { delegateUser }),
                "current-password"
            );

            // When
            await authenticatedController.Index(
                new ChangePasswordFormData
                {
                    Password = "new-password",
                    ConfirmPassword = "new-password",
                    CurrentPassword = "current-password"
                },
                DlsSubApplication.Default
            );

            // Then
            ThenMustHaveChangedPasswordForUsersOnce("new-password", new[] { delegateUser });
        }

        private void GivenLoggedInUserAccountsAre(AdminUser? adminUser, DelegateUser? delegateUser)
        {
            A.CallTo(() => userService.GetUsersById(LoggedInAdminId, LoggedInDelegateId))
                .Returns((adminUser, delegateUser));
        }

        private void GivenPasswordVerificationReturnsUsers(UserAccountSet users, string password)
        {
            A.CallTo(() => userService.GetVerifiedLinkedUsersAccounts(LoggedInAdminId, LoggedInDelegateId, password))
                .Returns(users);
        }

        private void GivenPasswordVerificationFails()
        {
            A.CallTo(() => userService.GetVerifiedLinkedUsersAccounts(A<int>._, A<int>._, A<string>._))
                .Returns(new UserAccountSet());
        }

        private void ThenMustHaveChangedPasswordForUsersOnce(string newPassword, IEnumerable<User> expectedUsers)
        {
            var expectedUserRefs = expectedUsers.Select(u => u.ToUserReference());
            ThenMustHaveChangedPasswordForUserRefsOnce(newPassword, expectedUserRefs);
        }

        private void ThenMustHaveChangedPasswordForUserRefsOnce(
            string newPassword,
            IEnumerable<UserReference> expectedUserRefs
        )
        {
            A.CallTo(
                    () => passwordService.ChangePasswordAsync(
                        A<IEnumerable<UserReference>>.That.IsSameSequenceAs(expectedUserRefs),
                        newPassword
                    )
                )
                .MustHaveHappened(1, Times.Exactly);
        }

        private void ThenMustNotHaveChangedPassword()
        {
            A.CallTo(() => passwordService.ChangePasswordAsync(A<IEnumerable<UserReference>>._, A<string>._))
                .MustNotHaveHappened();
            A.CallTo(() => passwordService.ChangePasswordAsync(A<string>._, A<string>._)).MustNotHaveHappened();
        }
    }
}
