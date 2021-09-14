namespace DigitalLearningSolutions.Web.Tests.Controllers.MyAccount
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using NUnit.Framework;

    public class MyAccountControllerTests
    {
        private const string Email = "test@user.com";
        private CentreCustomPromptHelper centreCustomPromptHelper = null!;
        private ICentreCustomPromptsService centreCustomPromptsService = null!;
        private IImageResizeService imageResizeService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            centreCustomPromptsService = A.Fake<ICentreCustomPromptsService>();
            userService = A.Fake<IUserService>();
            imageResizeService = A.Fake<ImageResizeService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            centreCustomPromptHelper = new CentreCustomPromptHelper(centreCustomPromptsService);
        }

        [Test]
        public void EditDetailsPostSave_with_invalid_model_doesnt_call_services()
        {
            // Given
            var myAccountController = new MyAccountController(
                centreCustomPromptsService,
                userService,
                imageResizeService,
                jobGroupsDataService,
                centreCustomPromptHelper
            ).WithDefaultContext().WithMockUser(true);
            var model = new EditDetailsViewModel();
            myAccountController.ModelState.AddModelError(nameof(EditDetailsViewModel.Email), "Required");

            // When
            var result = myAccountController.EditDetails(model, "save");

            // Then
            A.CallTo(() => userService.NewEmailAddressIsValid(A<string>._, A<int?>._, A<int?>._, A<int>._))
                .MustNotHaveHappened();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model);
        }

        [Test]
        public void EditDetailsPostSave_with_missing_delegate_answers_fails_validation()
        {
            // Given
            var myAccountController = new MyAccountController(
                centreCustomPromptsService,
                userService,
                imageResizeService,
                jobGroupsDataService,
                centreCustomPromptHelper
            ).WithDefaultContext().WithMockUser(true, adminId: null);
            var customPromptLists = new List<CustomPrompt>
                { CustomPromptsTestHelper.GetDefaultCustomPrompt(1, mandatory: true) };
            A.CallTo
                (() => centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(2)).Returns(
                CustomPromptsTestHelper.GetDefaultCentreCustomPrompts(customPromptLists, 2)
            );
            var model = new EditDetailsViewModel();

            // When
            var result = myAccountController.EditDetails(model, "save");

            // Then
            A.CallTo(() => userService.NewEmailAddressIsValid(A<string>._, A<int?>._, A<int?>._, A<int>._))
                .MustNotHaveHappened();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model);
            myAccountController.ModelState[nameof(EditDetailsViewModel.JobGroupId)].ValidationState.Should().Be
                (ModelValidationState.Invalid);
            myAccountController.ModelState[nameof(EditDetailsViewModel.Answer1)].ValidationState.Should().Be
                (ModelValidationState.Invalid);
        }

        [Test]
        public void EditDetailsPostSave_for_admin_user_with_missing_delegate_answers_doesnt_fail_validation()
        {
            // Given
            var myAccountController = new MyAccountController(
                centreCustomPromptsService,
                userService,
                imageResizeService,
                jobGroupsDataService,
                centreCustomPromptHelper
            ).WithDefaultContext().WithMockUser(true, delegateId: null);
            A.CallTo(() => userService.IsPasswordValid(7, null, "password")).Returns(true);
            A.CallTo(() => userService.NewEmailAddressIsValid(Email, 7, null, 2)).Returns(true);
            A.CallTo(() => userService.UpdateUserAccountDetails(A<AccountDetailsData>._, null)).DoesNothing();
            var model = new EditDetailsViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Email = Email,
                Password = "password",
                Application = ApplicationType.Default,
            };

            // When
            var result = myAccountController.EditDetails(model, "save");

            // Then
            A.CallTo(() => userService.NewEmailAddressIsValid(Email, 7, null, 2)).MustHaveHappened();
            A.CallTo(() => userService.UpdateUserAccountDetails(A<AccountDetailsData>._, null)).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void EditDetailsPostSave_without_previewing_profile_image_fails_validation()
        {
            // Given
            var myAccountController = new MyAccountController(
                centreCustomPromptsService,
                userService,
                imageResizeService,
                jobGroupsDataService,
                centreCustomPromptHelper
            ).WithDefaultContext().WithMockUser(true, adminId: null);
            var customPromptLists = new List<CustomPrompt>
                { CustomPromptsTestHelper.GetDefaultCustomPrompt(1, mandatory: true) };
            A.CallTo
                (() => centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(2)).Returns(
                CustomPromptsTestHelper.GetDefaultCentreCustomPrompts(customPromptLists, 2)
            );
            var model = new EditDetailsViewModel
            {
                ProfileImageFile = A.Fake<FormFile>()
            };

            // When
            var result = myAccountController.EditDetails(model, "save");

            // Then
            A.CallTo(() => userService.NewEmailAddressIsValid(A<string>._, A<int?>._, A<int?>._, A<int>._))
                .MustNotHaveHappened();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model);
            myAccountController.ModelState[nameof(EditDetailsViewModel.ProfileImageFile)].ValidationState.Should().Be
                (ModelValidationState.Invalid);
        }

        [Test]
        public void EditDetailsPostSave_with_invalid_password_fails_validation()
        {
            // Given
            var myAccountController = new MyAccountController(
                centreCustomPromptsService,
                userService,
                imageResizeService,
                jobGroupsDataService,
                centreCustomPromptHelper
            ).WithDefaultContext().WithMockUser(true, delegateId: null);
            A.CallTo(() => userService.IsPasswordValid(7, null, "password")).Returns(false);
            A.CallTo(() => userService.NewEmailAddressIsValid(Email, 7, null, 2)).Returns(true);
            A.CallTo(() => userService.UpdateUserAccountDetails(A<AccountDetailsData>._, null)).DoesNothing();

            var model = new EditDetailsViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Email = Email,
                Password = "password"
            };

            // When
            var result = myAccountController.EditDetails(model, "save");

            // Then
            A.CallTo(() => userService.NewEmailAddressIsValid(A<string>._, A<int?>._, A<int?>._, A<int>._))
                .MustNotHaveHappened();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model);
            myAccountController.ModelState[nameof(EditDetailsViewModel.Password)].ValidationState.Should().Be
                (ModelValidationState.Invalid);
        }

        [Test]
        public void PostEditRegistrationPrompt_returns_error_with_unexpected_action()
        {
            // Given
            var myAccountController = new MyAccountController(
                centreCustomPromptsService,
                userService,
                imageResizeService,
                jobGroupsDataService,
                centreCustomPromptHelper
            ).WithDefaultContext().WithMockUser(true, adminId: null);
            const string action = "unexpectedString";
            var model = new EditDetailsViewModel();

            // When
            var result = myAccountController.EditDetails(model, action);

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }
    }
}
