namespace DigitalLearningSolutions.Web.Tests.Controllers.ForgotPassword
{
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.ViewModels.ForgotPassword;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    class ForgotPasswordControllerTests
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
        public void No_model_should_render_basic_form()
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
            A.CallTo(() => passwordResetService.SendResetPasswordEmail(A<string>._)).DoesNothing();

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
            A.CallTo(() => passwordResetService.SendResetPasswordEmail(A<string>._)).MustHaveHappened();
        }

        [Test]
        public void Bad_email_submission_should_render_basic_form_with_error()
        {
            string errorMessage = "No user account could be found with the specified email address";

            // Given
            A.CallTo(() => passwordResetService.SendResetPasswordEmail(A<string>._)).Throws(new EmailAddressNotFoundException(errorMessage));

            // When
            var result = controller.Index("recipient@example.com");

            // Then
            result.Should().BeViewResult().WithDefaultViewName()
                .ModelAs<ForgotPasswordViewModel>().ErrorHasOccurred.Should().Be(true);
        }
    }
}
