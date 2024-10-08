﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.SetDelegatePassword;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class SetDelegatePasswordControllerTests
    {
        private const string Password = "password";
        private const int DelegateId = 2;

        private IPasswordService passwordService = null!;
        private SetDelegatePasswordController setDelegatePasswordController = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            userService = A.Fake<IUserService>();
            passwordService = A.Fake<IPasswordService>();
            setDelegatePasswordController = new SetDelegatePasswordController(passwordService, userService)
                .WithDefaultContext()
                .WithMockUser(true);
        }

        [Test]
        public void Index_should_return_view_result_with_IsFromViewDelegatePage_false_when_not_from_view_page()
        {
            // Given
            A.CallTo(() => userService.GetDelegateUserById(DelegateId))
                .Returns(UserTestHelper.GetDefaultDelegateUser());

            // When
            var result = setDelegatePasswordController.Index(DelegateId, false, null);

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
            result.As<ViewResult>().Model.As<SetDelegatePasswordViewModel>().IsFromViewDelegatePage.Should().BeFalse();
        }

        [Test]
        public void Index_should_return_view_result_with_IsFromViewDelegatePage_true_when_from_view_page()
        {
            // Given
            A.CallTo(() => userService.GetDelegateUserById(DelegateId))
                .Returns(UserTestHelper.GetDefaultDelegateUser());

            // When
            var result = setDelegatePasswordController.Index(DelegateId, true, null);

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
            result.As<ViewResult>().Model.As<SetDelegatePasswordViewModel>().IsFromViewDelegatePage.Should().BeTrue();
        }

        [Test]
        public async Task IndexAsync_with_invalid_model_returns_initial_form_async()
        {
            // Given
            var model = new SetDelegatePasswordViewModel();
            setDelegatePasswordController.ModelState.AddModelError(
                nameof(SetDelegatePasswordViewModel.Password),
                "Required"
            );

            // When
            var result = await setDelegatePasswordController.IndexAsync(model, DelegateId, true);

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public async Task IndexAsync_with_valid_model_calls_password_service_and_returns_confirmation_view_async()
        {
            // Given
            var delegateAccount = UserTestHelper.GetDefaultDelegateAccount();
            var model = new SetDelegatePasswordViewModel { Password = Password };
            A.CallTo(() => userService.GetDelegateAccountById(DelegateId))
                .Returns(delegateAccount);
            A.CallTo(() => passwordService.ChangePasswordAsync(A<int>._, A<string>._)).Returns(Task.CompletedTask);

            // When
            var result = await setDelegatePasswordController.IndexAsync(model, DelegateId, true);

            // Then
            A.CallTo(() => passwordService.ChangePasswordAsync(delegateAccount.UserId, Password))
                .MustHaveHappened();
            result.Should().BeViewResult().WithViewName("Confirmation");
        }
    }
}
