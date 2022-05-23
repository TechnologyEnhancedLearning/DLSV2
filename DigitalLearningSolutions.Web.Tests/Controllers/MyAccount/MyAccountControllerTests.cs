﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.MyAccount
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
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
        private PromptsService promptsService = null!;
        private ICentreRegistrationPromptsService centreRegistrationPromptsService = null!;
        private IImageResizeService imageResizeService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            centreRegistrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            userService = A.Fake<IUserService>();
            imageResizeService = A.Fake<ImageResizeService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            promptsService = new PromptsService(centreRegistrationPromptsService);
        }

        [Test]
        public void EditDetailsPostSave_with_invalid_model_doesnt_call_services()
        {
            // Given
            var myAccountController = new MyAccountController(
                centreRegistrationPromptsService,
                userService,
                imageResizeService,
                jobGroupsDataService,
                promptsService
            ).WithDefaultContext().WithMockUser(true);
            var formData = new MyAccountEditDetailsFormData();
            var expectedModel = new MyAccountEditDetailsViewModel(
                formData,
                new List<(int id, string name)>(),
                new List<EditDelegateRegistrationPromptViewModel>(),
                DlsSubApplication.Default
            );
            myAccountController.ModelState.AddModelError(nameof(MyAccountEditDetailsFormData.Email), "Required");

            // When
            var result = myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(() => userService.NewEmailAddressIsValid(A<string>._, A<int?>._, A<int?>._, A<int?>._))
                .MustNotHaveHappened();
            result.As<ViewResult>().Model.As<MyAccountEditDetailsViewModel>().Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void EditDetailsPostSave_with_missing_delegate_answers_fails_validation()
        {
            // Given
            var myAccountController = new MyAccountController(
                centreRegistrationPromptsService,
                userService,
                imageResizeService,
                jobGroupsDataService,
                promptsService
            ).WithDefaultContext().WithMockUser(true, adminId: null);
            var customPromptLists = new List<CentreRegistrationPrompt>
                { PromptsTestHelper.GetDefaultCentreRegistrationPrompt(1, mandatory: true) };
            A.CallTo
                (() => centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(2)).Returns(
                PromptsTestHelper.GetDefaultCentreRegistrationPrompts(customPromptLists, 2)
            );
            var formData = new MyAccountEditDetailsFormData();
            var expectedPrompt = new EditDelegateRegistrationPromptViewModel(1, "Custom Prompt", true, new List<string>(), null);
            var expectedModel = new MyAccountEditDetailsViewModel(
                formData,
                new List<(int id, string name)>(),
                new List<EditDelegateRegistrationPromptViewModel> { expectedPrompt },
                DlsSubApplication.Default
            );

            // When
            var result = myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(() => userService.NewEmailAddressIsValid(A<string>._, A<int?>._, A<int?>._, A<int?>._))
                .MustNotHaveHappened();
            result.As<ViewResult>().Model.As<MyAccountEditDetailsViewModel>().Should().BeEquivalentTo(expectedModel);
            myAccountController.ModelState[nameof(MyAccountEditDetailsFormData.Answer1)].ValidationState.Should().Be
                (ModelValidationState.Invalid);
        }

        [Test]
        public void EditDetailsPostSave_for_admin_user_with_missing_delegate_answers_doesnt_fail_validation()
        {
            // Given
            var myAccountController = new MyAccountController(
                centreRegistrationPromptsService,
                userService,
                imageResizeService,
                jobGroupsDataService,
                promptsService
            ).WithDefaultContext().WithMockUser(true, delegateId: null);
            A.CallTo(() => userService.IsPasswordValid(7, null, "password")).Returns(true);
            A.CallTo(() => userService.NewEmailAddressIsValid(Email, 7, null, 2)).Returns(true);
            A.CallTo(() => userService.UpdateUserAccountDetailsForAllUsers(A<int>._, A<MyAccountDetailsData>._, null))
                .DoesNothing();
            var model = new MyAccountEditDetailsFormData
            {
                FirstName = "Test",
                LastName = "User",
                Email = Email,
            };
            var parameterName = typeof(MyAccountController).GetMethod("Index")?.GetParameters()
                .SingleOrDefault(p => p.ParameterType == typeof(DlsSubApplication))?.Name;

            // When
            var result = myAccountController.EditDetails(model, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(() => userService.NewEmailAddressIsValid(Email, 7, null, 2)).MustHaveHappened();
            A.CallTo(() => userService.UpdateUserAccountDetailsForAllUsers(A<int>._, A<MyAccountDetailsData>._, null))
                .MustHaveHappened();

            result.Should().BeRedirectToActionResult().WithActionName("Index").WithRouteValue(
                parameterName,
                DlsSubApplication.Default.UrlSegment
            );
        }

        [Test]
        public void EditDetailsPostSave_without_previewing_profile_image_fails_validation()
        {
            // Given
            var myAccountController = new MyAccountController(
                centreRegistrationPromptsService,
                userService,
                imageResizeService,
                jobGroupsDataService,
                promptsService
            ).WithDefaultContext().WithMockUser(true, adminId: null);
            var customPromptLists = new List<CentreRegistrationPrompt>
                { PromptsTestHelper.GetDefaultCentreRegistrationPrompt(1, mandatory: true) };
            A.CallTo
                (() => centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(2)).Returns(
                PromptsTestHelper.GetDefaultCentreRegistrationPrompts(customPromptLists, 2)
            );
            var formData = new MyAccountEditDetailsFormData
            {
                ProfileImageFile = A.Fake<FormFile>(),
            };
            var expectedPrompt = new EditDelegateRegistrationPromptViewModel(1, "Custom Prompt", true, new List<string>(), null);
            var expectedModel = new MyAccountEditDetailsViewModel(
                formData,
                new List<(int id, string name)>(),
                new List<EditDelegateRegistrationPromptViewModel> { expectedPrompt },
                DlsSubApplication.Default
            );

            // When
            var result = myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(() => userService.NewEmailAddressIsValid(A<string>._, A<int?>._, A<int?>._, A<int?>._))
                .MustNotHaveHappened();
            result.As<ViewResult>().Model.As<MyAccountEditDetailsViewModel>().Should().BeEquivalentTo(expectedModel);
            myAccountController.ModelState[nameof(MyAccountEditDetailsFormData.ProfileImageFile)].ValidationState
                .Should().Be(ModelValidationState.Invalid);
        }

        [Test]
        public void EditDetailsPost_returns_error_with_unexpected_action()
        {
            // Given
            var myAccountController = new MyAccountController(
                centreRegistrationPromptsService,
                userService,
                imageResizeService,
                jobGroupsDataService,
                promptsService
            ).WithDefaultContext().WithMockUser(true, adminId: null);
            const string action = "unexpectedString";
            var model = new MyAccountEditDetailsFormData();

            // When
            var result = myAccountController.EditDetails(model, action, DlsSubApplication.Default);

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }
    }
}
