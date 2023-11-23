namespace DigitalLearningSolutions.Web.Tests.Controllers.ForgotPassword
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.ForgotPassword;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    internal class ForgotPasswordControllerTests
    {
        private ForgotPasswordController controller = null!;
        private IPasswordResetService passwordResetService = null!;
        private IConfiguration config = null!;

        [SetUp]
        public void SetUp()
        {
            passwordResetService = A.Fake<IPasswordResetService>();
            config = A.Fake<IConfiguration>();

            controller = new ForgotPasswordController(passwordResetService, config)
                .WithDefaultContext()
                .WithMockUser(false);
        }

        [Test]
        public void Index_should_render_if_user_is_unauthenticated()
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
            var controllerWithAuthenticatedUser = new ForgotPasswordController(passwordResetService, config)
                .WithDefaultContext()
                .WithMockUser(true);

            // When
            var result = controllerWithAuthenticatedUser.Index();

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public async Task Successful_email_submission_should_render_confirm_page()
        {
            // Given
            A.CallTo(() => passwordResetService.GenerateAndSendPasswordResetLink(A<string>._, A<string>._))
                .Returns(Task.CompletedTask);

            // When
            var result = await controller.Index(new ForgotPasswordViewModel("recipient@example.com"));

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Confirm");
        }

        [Test]
        public async Task Email_submission_should_call_password_reset_service()
        {
            // When
            await controller.Index(new ForgotPasswordViewModel("recipient@example.com"));

            // Then
            A.CallTo(() => passwordResetService.GenerateAndSendPasswordResetLink(A<string>._, A<string>._))
                .MustHaveHappened();
        }

        [Test]
        public async Task Bad_email_submission_should_render_basic_form_with_error()
        {
            string errorMessage = "No user account could be found with the specified email address";

            // Given
            A.CallTo(() => passwordResetService.GenerateAndSendPasswordResetLink(A<string>._, A<string>._))
                .Throws(new UserAccountNotFoundException(errorMessage));

            // When
            var result = await controller.Index(new ForgotPasswordViewModel("recipient@example.com"));

            // Then
            result.Should().BeViewResult().WithDefaultViewName()
                .ModelAs<ForgotPasswordViewModel>();
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [Test]
        public async Task Bad_database_insertion_should_render_unknown_error_page()
        {
            // Given
            A.CallTo(() => passwordResetService.GenerateAndSendPasswordResetLink(A<string>._, A<string>._))
                .Throws(new ResetPasswordInsertException("DB Insert failed"));

            // When
            var result = await controller.Index(new ForgotPasswordViewModel("recipient@example.com"));

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        [Test]
        public async Task Leading_trailing_whitespaces_in_email_should_be_ignored()
        {
            // Given
            A.CallTo(() => passwordResetService.GenerateAndSendPasswordResetLink(A<string>._, A<string>._))
                .Returns(Task.CompletedTask);

            // When
            var result = await controller.Index(new ForgotPasswordViewModel("  recipient@example.com\t"));

            // Then
            A.CallTo(() => passwordResetService.GenerateAndSendPasswordResetLink("recipient@example.com", A<string>._))
                .MustHaveHappened(1, Times.Exactly);
            result.Should().BeRedirectToActionResult().WithActionName("Confirm");
        }
    }
}
