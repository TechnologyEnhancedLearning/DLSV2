namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class ChangePasswordControllerTests
    {
        private const int LoggedInUserId = 1;

        private ChangePasswordController authenticatedController = null!;
        private IPasswordService passwordService = null!;
        private IUserVerificationService userVerificationService = null!;

        [SetUp]
        public void SetUp()
        {
            passwordService = A.Fake<IPasswordService>();
            userVerificationService = A.Fake<IUserVerificationService>();
            authenticatedController = new ChangePasswordController(passwordService, userVerificationService)
                .WithDefaultContext()
                .WithMockUser(true, userId: LoggedInUserId);
        }

        [Test]
        public async Task Post_returns_form_if_model_invalid()
        {
            // Given
            authenticatedController.ModelState.AddModelError("key", "Invalid for testing.");

            // When
            var result = await authenticatedController.Index(new ChangePasswordFormData(), DlsSubApplication.Default);

            // Then
            A.CallTo(() => passwordService.ChangePasswordAsync(A<int>._, A<string>._)).MustNotHaveHappened();
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public async Task Post_returns_form_if_current_password_does_not_match_user_ids()
        {
            // Given
            A.CallTo(() => userVerificationService.IsPasswordValid(A<string>._, A<int>._)).Returns(false);

            // When
            var result = await authenticatedController.Index(new ChangePasswordFormData(), DlsSubApplication.Default);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<ChangePasswordViewModel>();
        }

        [Test]
        public async Task Post_does_not_change_password_if_current_password_does_not_match_user_ids()
        {
            // Given
            A.CallTo(() => userVerificationService.IsPasswordValid(A<string>._, A<int>._)).Returns(false);

            // When
            await authenticatedController.Index(new ChangePasswordFormData(), DlsSubApplication.Default);

            // Then
            A.CallTo(() => passwordService.ChangePasswordAsync(A<int>._, A<string>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Post_changes_password_and_returns_success_page_if_model_and_password_valid()
        {
            // Given
            A.CallTo(() => userVerificationService.IsPasswordValid(A<string>._, A<int>._)).Returns(true);

            // When
            var result = await authenticatedController.Index(
                new ChangePasswordFormData
                {
                    Password = "new-password",
                    ConfirmPassword = "new-password",
                    CurrentPassword = "current-password",
                },
                DlsSubApplication.Default
            );

            // Then
            ThenMustHaveChangedPasswordForUserIdOnce(LoggedInUserId, "new-password");
            result.Should().BeViewResult().WithViewName("Success");
        }

        private void ThenMustHaveChangedPasswordForUserIdOnce(
            int userId,
            string newPassword
        )
        {
            A.CallTo(
                    () => passwordService.ChangePasswordAsync(
                        userId,
                        newPassword
                    )
                )
                .MustHaveHappened(1, Times.Exactly);
        }
    }
}
