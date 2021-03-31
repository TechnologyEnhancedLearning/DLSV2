namespace DigitalLearningSolutions.Web.Tests.Controllers.ForgotPassword
{
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.ViewModels.ForgotPassword;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    internal class ForgotPasswordControllerTests
    {
        private ForgotPasswordController controller;
        private IPasswordResetService passwordResetService;

        [SetUp]
        public void SetUp()
        {
            passwordResetService = A.Fake<IPasswordResetService>();

            controller = new ForgotPasswordController(passwordResetService);
        }

        [Test]
        public void Render_basic_form_when_there_is_no_error_model()
        {
            // When
            var result = controller.Index(model: null);

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void Successful_email_submission_should_render_confirm_page()
        {
            // Given
            A.CallTo(() => passwordResetService.GenerateAndSendPasswordResetLink(A<string>._, A<string>._)).DoesNothing();

            // When
            var result = controller.Index("recipient@example.com");

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Confirm");
        }

        [Test]
        public void Email_submission_should_call_password_reset_service()
        {
            // When
            controller.Index("recipient@example.com");

            // Then
            A.CallTo(() => passwordResetService.GenerateAndSendPasswordResetLink(A<string>._, A<string>._)).MustHaveHappened();
        }

        [Test]
        public void Bad_email_submission_should_render_basic_form_with_error()
        {
            string errorMessage = "No user account could be found with the specified email address";

            // Given
            A.CallTo(() => passwordResetService.GenerateAndSendPasswordResetLink(A<string>._, A<string>._)).Throws(new EmailAddressNotFoundException(errorMessage));

            // When
            var result = controller.Index("recipient@example.com");

            // Then
            result.Should().BeViewResult().WithDefaultViewName()
                .ModelAs<ForgotPasswordViewModel>().EmailErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void Bad_database_insertion_should_render_unknown_error_page()
        {
            // Given
            A.CallTo(() => passwordResetService.GenerateAndSendPasswordResetLink(A<string>._, A<string>._)).Throws(new ResetPasswordInsertException("DB Insert failed"));

            // When
            var result = controller.Index("recipient@example.com");

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions").WithActionName("Error");
        }
    }
}
