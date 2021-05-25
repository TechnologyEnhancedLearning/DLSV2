namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class ResetPasswordControllerTests
    {
        private ResetPasswordController authenticatedController;
        private ResetPasswordController unauthenticatedController;
        private IPasswordResetService passwordResetService;

        [SetUp]
        public void SetUp()
        {
            passwordResetService = A.Fake<IPasswordResetService>();

            unauthenticatedController = new ResetPasswordController(passwordResetService)
                .WithDefaultContext()
                .WithMockUser(false, 2);
            authenticatedController = new ResetPasswordController(passwordResetService)
                .WithDefaultContext()
                .WithMockUser(true, 2);
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
            A.CallTo(() => passwordResetService.GetValidMatchingUserReferencesAsync("email", "code"))
                .Returns(Task.FromResult(new List<UserReference> { new UserReference() }));

            // When
            var result = await unauthenticatedController.Index("email", "code");

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public async Task Index_should_redirect_to_error_page_if_user_is_unauthenticated_and_query_params_are_invalid()
        {
            // Given
            A.CallTo(() => passwordResetService.GetValidMatchingUserReferencesAsync("email", "code"))
                .Returns(Task.FromResult(new List<UserReference>()));

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
    }
}
