namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class ChangePasswordControllerTests
    {
        private ChangePasswordController authenticatedController = null!;
        private ILoginService loginService = null!;

        [SetUp]
        public void SetUp()
        {
            loginService = A.Fake<ILoginService>();
            authenticatedController = new ChangePasswordController().WithDefaultContext().WithMockUser
                (isAuthenticated: true);
        }

        [Test]
        public void Post_returns_form_if_model_invalid()
        {
            // Given
            authenticatedController.ModelState.AddModelError("key", "Invalid for testing.");

            // When
            var result = authenticatedController.Index(new ChangePasswordViewModel());

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void Post_returns_form_if_current_password_incorrect()
        {
            // Given
            A.CallTo(() => loginService.VerifyUsers(A<string>._, A<AdminUser>._, A<List<DelegateUser>>._)).Returns
                ((null, new List<DelegateUser>()));
            var model = new ChangePasswordViewModel();

            // When
            var result = authenticatedController.Index(model);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<ChangePasswordViewModel>();
        }

        [Test]
        public void Post_returns_success_page_if_model_and_password_valid()
        {
            // Given
            A.CallTo(() => loginService.VerifyUsers("password", A<AdminUser>._, A<List<DelegateUser>>._)).Returns
                ((, new List<DelegateUser>()));

            // When
            var result = authenticatedController.Index
            (
                new ChangePasswordViewModel
                    { Password = "password", ConfirmPassword = "password", CurrentPassword = "current-password" }
            );

            // Then
            result.Should().BeViewResult().WithViewName("Success");
        }
    }
}
