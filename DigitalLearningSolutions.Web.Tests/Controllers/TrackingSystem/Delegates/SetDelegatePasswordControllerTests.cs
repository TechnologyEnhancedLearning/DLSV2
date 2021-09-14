namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.SetDelegatePassword;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Primitives;
    using NUnit.Framework;

    public class SetDelegatePasswordControllerTests
    {
        private const string Password = "password";
        private const int DelegateId = 2;

        private IPasswordService passwordService = null!;
        private HttpRequest request = null!;
        private SetDelegatePasswordController setDelegatePasswordController = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            request = A.Fake<HttpRequest>();
            userDataService = A.Fake<IUserDataService>();
            passwordService = A.Fake<IPasswordService>();
            setDelegatePasswordController = new SetDelegatePasswordController(passwordService, userDataService)
                .WithMockHttpRequestHttpContext(request)
                .WithMockServices()
                .WithMockTempData()
                .WithMockUser(true);
        }

        [Test]
        public void Index_should_return_not_found_with_null_delegate_user()
        {
            // Given
            A.CallTo(() => userDataService.GetDelegateUserById(DelegateId)).Returns(null);

            // When
            var result = setDelegatePasswordController.Index(DelegateId);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_should_return_not_found_with_delegate_user_at_different_centre()
        {
            // Given
            A.CallTo(() => userDataService.GetDelegateUserById(DelegateId))
                .Returns(UserTestHelper.GetDefaultDelegateUser(centreId: 1));

            // When
            var result = setDelegatePasswordController.Index(DelegateId);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_should_throw_exception_for_delegate_user_with_no_email()
        {
            // Given
            A.CallTo(() => userDataService.GetDelegateUserById(DelegateId))
                .Returns(UserTestHelper.GetDefaultDelegateUser(emailAddress: null));

            // Then
            Assert.Throws<NoDelegateEmailException>(() => setDelegatePasswordController.Index(DelegateId));
        }

        [Test]
        public void Index_should_return_view_result_with_IsFromViewDelegatePage_false_when_referer_is_not_view_page()
        {
            // Given
            A.CallTo(() => userDataService.GetDelegateUserById(DelegateId))
                .Returns(UserTestHelper.GetDefaultDelegateUser());
            request.Headers["Referer"] = "https://baseurl/Some/Other/Url";

            // When
            var result = setDelegatePasswordController.Index(DelegateId);

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
            result.As<ViewResult>().Model.As<SetDelegatePasswordViewModel>().IsFromViewDelegatePage.Should().BeFalse();
        }

        [Test]
        public void Index_should_return_view_result_with_IsFromViewDelegatePage_false_when_referer_is_null()
        {
            // Given
            A.CallTo(() => userDataService.GetDelegateUserById(DelegateId))
                .Returns(UserTestHelper.GetDefaultDelegateUser());
            request.Headers["Referer"] = new StringValues();

            // When
            var result = setDelegatePasswordController.Index(DelegateId);

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
            result.As<ViewResult>().Model.As<SetDelegatePasswordViewModel>().IsFromViewDelegatePage.Should().BeFalse();
        }

        [Test]
        public void Index_should_return_view_result_with_IsFromViewDelegatePage_true_when_referer_is_view_page()
        {
            // Given
            A.CallTo(() => userDataService.GetDelegateUserById(DelegateId))
                .Returns(UserTestHelper.GetDefaultDelegateUser());
            request.Headers["Referer"] = "https://baseurl/TrackingSystem/Delegates/View/86726";

            // When
            var result = setDelegatePasswordController.Index(DelegateId);

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
            result.As<ViewResult>().Model.As<SetDelegatePasswordViewModel>().IsFromViewDelegatePage.Should().BeTrue();
        }

        [Test]
        public async Task IndexAsync_with_invalid_model_returns_view_async()
        {
            // Given
            var model = new SetDelegatePasswordViewModel();
            setDelegatePasswordController.ModelState.AddModelError(
                nameof(SetDelegatePasswordViewModel.Password),
                "Required"
            );

            // When
            var result = await setDelegatePasswordController.IndexAsync(model, DelegateId);

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public async Task IndexAsync_with_valid_model_calls_password_service_and_returns_confirmation_view_async()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var model = new SetDelegatePasswordViewModel { Password = Password };
            A.CallTo(() => userDataService.GetDelegateUserById(DelegateId))
                .Returns(delegateUser);
            A.CallTo(() => passwordService.ChangePasswordAsync(A<string>._, A<string>._)).Returns(Task.CompletedTask);

            // When
            var result = await setDelegatePasswordController.IndexAsync(model, DelegateId);

            // Then
            A.CallTo(() => passwordService.ChangePasswordAsync(delegateUser.EmailAddress!, Password))
                .MustHaveHappened();
            result.Should().BeViewResult().WithViewName("Confirmation");
        }
    }
}
