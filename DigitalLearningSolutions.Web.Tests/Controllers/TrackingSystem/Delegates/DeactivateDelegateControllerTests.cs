using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
using DigitalLearningSolutions.Web.Services;
using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
using DigitalLearningSolutions.Web.Tests.TestHelpers;
using DigitalLearningSolutions.Web.Helpers;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using NUnit.Framework;
using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DeactivateDelegate;
using DigitalLearningSolutions.Data.Models.User;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    public class DeactivateDelegateControllerTests
    {
        private const int DelegateId = 1;
        private DeactivateDelegateController controller = null!;
        private IUserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            userService = A.Fake<IUserService>();

            controller = new DeactivateDelegateController(userService)
                .WithDefaultContext()
                .WithMockUser(true);
        }

        [Test]
        public void Index_returns_not_found_with_null_delegate()
        {
            // Given
            A.CallTo(() => userService.CheckDelegateIsActive(DelegateId)).Returns(null);

            // When
            var result = controller.Index(DelegateId);

            // Then
            result.Should()
               .BeRedirectToActionResult()
               .WithControllerName("LearningSolutions")
               .WithActionName("StatusCode")
               .WithRouteValue("code", 410);
        }

        [Test]
        public void Index_returns_view_when_service_returns_valid_delegate()
        {
            // Given
            A.CallTo(() => userService.CheckDelegateIsActive(DelegateId)).Returns(DelegateId);
            int? centreId = controller.User.GetCentreId();

            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(DelegateId, centreId.Value);
            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(delegateEntity);

            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount> { UserTestHelper.GetDefaultAdminAccount(centreId.Value) },
                new List<DelegateAccount> { UserTestHelper.GetDefaultDelegateAccount(centreId.Value) }
            );

            A.CallTo(() => userService.GetUserById(delegateEntity.DelegateAccount.UserId)).Returns(userEntity);

            // When
            var result = controller.Index(DelegateId);

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
            result.Should().BeViewResult().ModelAs<DeactivateDelegateAccountViewModel>().DelegateId.Should().Be(DelegateId);
            result.Should().BeViewResult().ModelAs<DeactivateDelegateAccountViewModel>().Name.Should().Be(delegateEntity.UserAccount.FirstName + " " + delegateEntity.UserAccount.LastName);
            result.Should().BeViewResult().ModelAs<DeactivateDelegateAccountViewModel>().Email.Should().Be(delegateEntity.UserAccount.PrimaryEmail);
            result.Should().BeViewResult().ModelAs<DeactivateDelegateAccountViewModel>().UserId.Should().Be(delegateEntity.UserAccount.Id);
        }

        [Test]
        public void Index_post_returns_error_when_no_option_selected_to_deactivate()
        {
            // Given
            var formData = new DeactivateDelegateAccountViewModel
            {
                DelegateId = 1,
                Name = "Firstname Test",
                Roles = new List<string>(),
                Email = "email@test.com",
                UserId = 2,
                Deactivate = null
            };

            controller.ModelState.AddModelError("key", "Invalid for testing.");

            // When
            var result = controller.Index(formData);

            // Then
            result.Should().BeViewResult().ModelAs<DeactivateDelegateAccountViewModel>().DelegateId.Should().Be(DelegateId);
            Assert.That(controller.ModelState.IsValid, Is.False);
        }


        [Test]
        public void Index_post_returns_view_for_deactivate_delegate_only()
        {
            // Given
            int? centreId = controller.User.GetCentreId();
            var formData = new DeactivateDelegateAccountViewModel
            {
                DelegateId = 1,
                Name = "Firstname Test",
                Roles = new List<string>(),
                Email = "email@test.com",
                UserId = 2,
                Deactivate = true
            };

            // When
            var result = controller.Index(formData);

            // Then
            A.CallTo(() => userService.DeactivateDelegateUser(DelegateId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.DeactivateAdminAccount(formData.UserId, centreId.Value)).MustNotHaveHappened();

            result.Should().BeRedirectToActionResult()
                .WithControllerName("ViewDelegate")
                .WithActionName("Index")
                .WithRouteValue("delegateId", formData.DelegateId);

        }

        [Test]
        public void Index_post_returns_view_for_deactivate_delegate_and_admin()
        {
            // Given
            int? centreId = controller.User.GetCentreId();
            var formData = new DeactivateDelegateAccountViewModel
            {
                DelegateId = 1,
                Name = "Firstname Test",
                Roles = new List<string>(),
                Email = "email@test.com",
                UserId = 2,
                Deactivate = false
            };

            // When
            var result = controller.Index(formData);

            // Then
            A.CallTo(() => userService.DeactivateDelegateUser(DelegateId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.DeactivateAdminAccount(formData.UserId, centreId.Value)).MustHaveHappenedOnceExactly();

            result.Should().BeRedirectToActionResult()
                .WithControllerName("ViewDelegate")
                .WithActionName("Index")
                .WithRouteValue("delegateId", formData.DelegateId);
        }

    }
}
