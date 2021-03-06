namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class ResetPasswordControllerTests
    {
        private ResetPasswordController authenticatedController;
        private ResetPasswordController unauthenticatedController;
        private IPasswordResetService passwordResetService;
        private IPasswordService passwordService;

        [SetUp]
        public void SetUp()
        {
            passwordResetService = A.Fake<IPasswordResetService>();
            passwordService = A.Fake<IPasswordService>();

            unauthenticatedController = new ResetPasswordController(passwordResetService, passwordService)
                .WithDefaultContext()
                .WithMockTempData()
                .WithMockUser(false);
            authenticatedController = new ResetPasswordController(passwordResetService, passwordService)
                .WithDefaultContext()
                .WithMockTempData()
                .WithMockUser(true);
        }

        [Test]
        public async Task Index_should_redirect_to_homepage_if_user_is_authenticated()
        {
            // When
            var result = await authenticatedController.Index("email", "code");

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public async Task Index_should_render_if_user_is_unauthenticated_and_query_params_are_valid()
        {
            // Given
            A.CallTo(() => passwordResetService.EmailAndResetPasswordHashAreValidAsync("email", "code"))
                .Returns(Task.FromResult(true));

            // When
            var result = await unauthenticatedController.Index("email", "code");

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public async Task Index_should_redirect_to_error_page_if_user_is_unauthenticated_and_query_params_are_invalid()
        {
            // Given
            A.CallTo(() => passwordResetService.EmailAndResetPasswordHashAreValidAsync("email", "code"))
                .Returns(Task.FromResult(false));

            // When
            var result = await unauthenticatedController.Index("email", "code");

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Error");
        }

        [Test]
        public async Task Index_should_redirect_to_login_page_if_user_is_unauthenticated_and_email_is_missing()
        {
            // When
            var result = await unauthenticatedController.Index(null, "code");

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Login").WithActionName("Index");
        }

        [Test]
        public async Task Index_should_redirect_to_login_page_if_user_is_unauthenticated_and_code_is_missing()
        {
            // When
            var result = await unauthenticatedController.Index("email", null);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Login").WithActionName("Index");
        }

        [Test]
        public async Task Index_should_set_email_and_hash_in_temp_data_if_valid()
        {
            // Given
            A.CallTo(() => passwordResetService.EmailAndResetPasswordHashAreValidAsync("email", "hash"))
                .Returns(true);

            // When
            await unauthenticatedController.Index("email", "hash");

            // Then
            unauthenticatedController.TempData.Peek<ResetPasswordData>().Should().BeEquivalentTo(
                new ResetPasswordData("email", "hash"));
        }

        [Test]
        public async Task Post_to_index_should_invalidate_reset_hash_if_model_and_hash_valid()
        {
            // Given
            A.CallTo(() => passwordResetService.EmailAndResetPasswordHashAreValidAsync("email", "hash")).Returns(true);
            unauthenticatedController.TempData.Set(new ResetPasswordData("email", "hash"));

            // When
            await unauthenticatedController.Index(
                new ConfirmPasswordViewModel { Password = "testPass-9", ConfirmPassword = "testPass-9" });

            // Then
            A.CallTo(() => passwordResetService.InvalidateResetPasswordForEmailAsync("email"))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public async Task Post_to_index_should_update_password_if_model_and_hash_valid()
        {
            // Given
            unauthenticatedController.TempData.Set(new ResetPasswordData("email", "hash"));
            A.CallTo(() => passwordResetService.EmailAndResetPasswordHashAreValidAsync("email", "hash")).Returns(true);

            // When
            await unauthenticatedController.Index(
                new ConfirmPasswordViewModel { Password = "testPass-9", ConfirmPassword = "testPass-9" });

            // Then
            A.CallTo(() => passwordService.ChangePasswordAsync("email", "testPass-9"))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public async Task Post_to_index_should_return_success_page_if_model_and_hash_valid()
        {
            // Given
            unauthenticatedController.TempData.Set(new ResetPasswordData("email", "hash"));
            A.CallTo(() => passwordResetService.EmailAndResetPasswordHashAreValidAsync("email", "hash")).Returns(true);

            // When
            var result = await unauthenticatedController.Index(
                new ConfirmPasswordViewModel { Password = "testPass-9", ConfirmPassword = "testPass-9" });

            // Then
            result.Should().BeViewResult().WithViewName("Success");
        }

        [Test]
        public async Task Post_to_index_should_clear_temp_data_if_model_and_hash_valid()
        {
            // Given
            unauthenticatedController.TempData.Set(new ResetPasswordData("email", "hash"));
            unauthenticatedController.TempData.Set("some string");
            A.CallTo(() => passwordResetService.EmailAndResetPasswordHashAreValidAsync("email", "hash")).Returns(true);

            // When
            await unauthenticatedController.Index(
                new ConfirmPasswordViewModel { Password = "testPass-9", ConfirmPassword = "testPass-9" });

            // Then
            unauthenticatedController.TempData.Peek<ResetPasswordData>().Should().BeNull();
            unauthenticatedController.TempData.Peek<string>().Should().BeNull();
        }

        [Test]
        public async Task Post_to_index_should_clear_temp_data_if_hash_invalid()
        {
            // Given
            unauthenticatedController.TempData.Set(new ResetPasswordData("email", "hash"));
            unauthenticatedController.TempData.Set("some string");
            A.CallTo(() => passwordResetService.EmailAndResetPasswordHashAreValidAsync("email", "hash")).Returns(false);

            // When
            await unauthenticatedController.Index(
                new ConfirmPasswordViewModel { Password = "testPass-9", ConfirmPassword = "testPass-9" });

            // Then
            unauthenticatedController.TempData.Peek<ResetPasswordData>().Should().BeNull();
            unauthenticatedController.TempData.Peek<string>().Should().BeNull();
        }

        [Test]
        public async Task Post_to_index_should_preserve_temp_data_if_model_invalid()
        {
            // Given
            unauthenticatedController.TempData.Set(new ResetPasswordData("email", "hash"));
            A.CallTo(() => passwordResetService.EmailAndResetPasswordHashAreValidAsync("email", "hash")).Returns(true);
            unauthenticatedController.ModelState.AddModelError("model", "Invalid for testing");

            // When
            await unauthenticatedController.Index(
                new ConfirmPasswordViewModel { Password = "testPass-9", ConfirmPassword = "testPass-9" });

            // Then
            unauthenticatedController.TempData.Peek<ResetPasswordData>().Should()
                .BeEquivalentTo(new ResetPasswordData("email", "hash"));
        }

        [Test]
        public async Task Post_to_index_should_return_form_if_model_state_invalid()
        {
            // Given
            unauthenticatedController.TempData.Set(new ResetPasswordData("email", "hash"));
            unauthenticatedController.ModelState.AddModelError("Testings", "errors for testing");
            A.CallTo(() => passwordResetService.EmailAndResetPasswordHashAreValidAsync("email", "hash")).Returns(true);

            // When
            var result = await unauthenticatedController.Index(
                new ConfirmPasswordViewModel { Password = "testPass-9", ConfirmPassword = "testPass-9" });

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public async Task Post_to_index_should_redirect_to_Error_if_reset_password_invalid()
        {
            // Given
            A.CallTo(() => passwordResetService.EmailAndResetPasswordHashAreValidAsync("email", "hash")).Returns(false);
            unauthenticatedController.TempData.Set(new ResetPasswordData("email", "hash"));

            // When
            var result = await unauthenticatedController.Index(
                new ConfirmPasswordViewModel { Password = "testPass-9", ConfirmPassword = "testPass-9" });

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Error").WithControllerName(null);
        }
    }
}
