namespace DigitalLearningSolutions.Web.Tests.Controllers.MyAccount
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class MyAccountControllerTests
    {
        private const string Email = "test@user.com";
        private ICentreRegistrationPromptsService centreRegistrationPromptsService = null!;
        private IConfiguration config = null!;
        private IImageResizeService imageResizeService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private ILogger<MyAccountController> logger = null!;
        private PromptsService promptsService = null!;
        private IUrlHelper urlHelper = null!;
        private IUserService userService = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            centreRegistrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            config = A.Fake<IConfiguration>();
            userService = A.Fake<IUserService>();
            userDataService = A.Fake<IUserDataService>();
            imageResizeService = A.Fake<ImageResizeService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            promptsService = new PromptsService(centreRegistrationPromptsService);
            logger = A.Fake<ILogger<MyAccountController>>();
            urlHelper = A.Fake<IUrlHelper>();

            A.CallTo(() => config["AppRootPath"]).Returns("https://www.test.com");
        }

        [Test]
        public void Index_sets_switch_centre_return_url_correctly()
        {
            // Given
            var myAccountController = GetMyAccountController().WithMockUser(true);
            const string expectedReturnUrl = "/Home/Welcome";

            // When
            var result = myAccountController.Index(DlsSubApplication.Default);

            // Then
            result.As<ViewResult>().Model.As<MyAccountViewModel>().SwitchCentreReturnUrl.Should()
                .BeEquivalentTo(expectedReturnUrl);
        }

        [Test]
        public void EditDetailsPostSave_with_invalid_model_doesnt_call_services()
        {
            // Given
            var myAccountController = GetMyAccountController().WithMockUser(true);
            var formData = new MyAccountEditDetailsFormData();
            var expectedModel = GetBasicMyAccountEditDetailsViewModel(formData);

            myAccountController.ModelState.AddModelError(nameof(MyAccountEditDetailsFormData.Email), "Required");

            // When
            var result = myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(
                () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                    A<EditAccountDetailsData>._,
                    A<DelegateDetailsData?>._,
                    A<string?>._,
                    A<int>._,
                    A<bool>._
                )
            ).MustNotHaveHappened();

            A.CallTo(
                () => userService.UpdateUserDetails(
                    A<EditAccountDetailsData>._,
                    A<bool>._,
                    A<DateTime?>._
                )
            ).MustNotHaveHappened();

            A.CallTo(
                () => userService.SetCentreEmails(A<int>._, A<Dictionary<int, string?>>._)
            ).MustNotHaveHappened();

            result.As<ViewResult>().Model.As<MyAccountEditDetailsViewModel>().Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void EditDetailsPostSave_with_missing_delegate_answers_fails_validation()
        {
            // Given
            var myAccountController = GetMyAccountController().WithMockUser(true, adminId: null);

            var customPromptLists = new List<CentreRegistrationPrompt>
            {
                PromptsTestHelper.GetDefaultCentreRegistrationPrompt(1, mandatory: true)
            };

            var testUserEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new AdminAccount[] { },
                new[] { UserTestHelper.GetDefaultDelegateAccount() }
            );

            var formData = new MyAccountEditDetailsFormData();
            var expectedPrompt = new EditDelegateRegistrationPromptViewModel(
                1,
                "Custom Prompt",
                true,
                new List<string>(),
                null
            );

            var expectedModel = GetBasicMyAccountEditDetailsViewModel(formData);
            expectedModel.DelegateRegistrationPrompts.Add(expectedPrompt);

            A.CallTo(() => userService.GetUserById(A<int>._)).Returns(testUserEntity);
            A.CallTo(() => centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(2)).Returns(
                PromptsTestHelper.GetDefaultCentreRegistrationPrompts(customPromptLists, 2)
            );

            // When
            var result = myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(() => userDataService.PrimaryEmailIsInUse(A<string>._, A<IDbTransaction?>._))
                .MustNotHaveHappened();

            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentre(A<string>._, A<int>._, A<IDbTransaction?>._)
            ).MustNotHaveHappened();

            result.As<ViewResult>().Model.As<MyAccountEditDetailsViewModel>().Should().BeEquivalentTo(expectedModel);

            myAccountController.ModelState[nameof(MyAccountEditDetailsFormData.Answer1)].ValidationState.Should().Be
                (ModelValidationState.Invalid);
        }

        [Test]
        public void
            EditDetailsPostSave_for_admin_only_user_with_missing_delegate_answers_doesnt_fail_validation_or_update_delegate()
        {
            // Given
            const int userId = 2;
            var myAccountController = GetMyAccountController().WithMockUser(true, delegateId: null, userId: userId);
            var model = GetBasicMyAccountEditDetailsFormData();

            var testUserEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new[] { UserTestHelper.GetDefaultAdminAccount() },
                new DelegateAccount[] { }
            );

            A.CallTo(() => userService.GetUserById(A<int>._)).Returns(testUserEntity);
            A.CallTo(() => userDataService.PrimaryEmailIsInUse(Email, null)).Returns(false);
            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre(Email, 2, null)).Returns(false);

            // When
            var result = myAccountController.EditDetails(model, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        null, // null delegateDetailsData -> delegate account is not updated
                        A<string?>._,
                        A<int>._,
                        true
                    )
                )
                .MustHaveHappened();

            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void EditDetailsPostSave_with_valid_info_and_valid_return_url_redirects_to_return_url()
        {
            // Given
            const string returnUrl = "/TrackingSystem/Centre/Dashboard";
            var myAccountController = GetMyAccountController()
                .WithMockUser(true, delegateId: null)
                .WithMockUrlHelper(urlHelper);

            var model = GetBasicMyAccountEditDetailsFormData();
            model.ReturnUrl = returnUrl;

            A.CallTo(() => userDataService.PrimaryEmailIsInUse(Email, A<IDbTransaction?>._)).Returns(false);
            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre(Email, 2, A<IDbTransaction?>._))
                .Returns(false);
            A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        A<DelegateDetailsData>._,
                        A<string?>._,
                        A<int>._,
                        A<bool>._
                    )
                )
                .DoesNothing();
            A.CallTo(() => urlHelper.IsLocalUrl(returnUrl)).Returns(true);

            // When
            var result = myAccountController.EditDetails(model, "save", DlsSubApplication.Default);

            // Then
            result.Should().BeRedirectResult().WithUrl(returnUrl);
        }

        [Test]
        public void EditDetailsPostSave_with_valid_info_and_invalid_return_url_redirects_to_index()
        {
            // Given
            var myAccountController = GetMyAccountController()
                .WithMockUser(true, delegateId: null)
                .WithMockUrlHelper(urlHelper);

            var model = GetBasicMyAccountEditDetailsFormData();
            model.ReturnUrl = "/TrackingSystem/Centre/Dashboard";

            var parameterName = typeof(MyAccountController).GetMethod("Index")?.GetParameters()
                .SingleOrDefault(p => p.ParameterType == typeof(DlsSubApplication))?.Name;

            A.CallTo(() => urlHelper.IsLocalUrl(A<string>._)).Returns(false);
            A.CallTo(() => userDataService.PrimaryEmailIsInUse(Email, A<IDbTransaction?>._)).Returns(false);
            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre(Email, 2, A<IDbTransaction?>._))
                .Returns(false);

            A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        A<DelegateDetailsData>._,
                        A<string?>._,
                        A<int>._,
                        A<bool>._
                    )
                )
                .DoesNothing();

            // When
            var result = myAccountController.EditDetails(model, "save", DlsSubApplication.Default);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Index").WithRouteValue(
                parameterName,
                DlsSubApplication.Default.UrlSegment
            );
        }

        [Test]
        public void EditDetailsPostSave_without_previewing_profile_image_fails_validation()
        {
            // Given
            var myAccountController = GetMyAccountController().WithMockUser(true, adminId: null);

            var customPromptLists = new List<CentreRegistrationPrompt>
            {
                PromptsTestHelper.GetDefaultCentreRegistrationPrompt(1, mandatory: true),
            };

            var formData = new MyAccountEditDetailsFormData
            {
                ProfileImageFile = A.Fake<FormFile>(),
            };

            var expectedPrompt = new EditDelegateRegistrationPromptViewModel(
                1,
                "Custom Prompt",
                true,
                new List<string>(),
                null
            );

            var expectedModel = GetBasicMyAccountEditDetailsViewModel(formData);
            expectedModel.DelegateRegistrationPrompts.Add(expectedPrompt);

            A.CallTo(() => centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(2)).Returns(
                PromptsTestHelper.GetDefaultCentreRegistrationPrompts(customPromptLists, 2)
            );

            // When
            var result = myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

            // Then
            result.As<ViewResult>().Model.As<MyAccountEditDetailsViewModel>().Should().BeEquivalentTo(expectedModel);

            myAccountController.ModelState[nameof(MyAccountEditDetailsFormData.ProfileImageFile)].ValidationState
                .Should().Be(ModelValidationState.Invalid);
        }

        [Test]
        public void EditDetailsPost_with_unexpected_action_returns_error()
        {
            // Given
            var myAccountController = GetMyAccountController().WithMockUser(true);
            const string action = "unexpectedString";
            var model = new MyAccountEditDetailsFormData();

            // When
            var result = myAccountController.EditDetails(model, action, DlsSubApplication.Default);

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        [Test]
        public void EditDetailsPost_with_no_centreId_updates_user_details_and_all_centre_specific_emails()
        {
            // Given
            const int userId = 2;
            var centreSpecificEmailsByCentreId = new Dictionary<int, string?>
            {
                { 1, "email@centre1.com" },
                { 2, "email@centre2.com" },
                { 3, null },
            };

            var (myAccountController, formData) =
                GetCentrelessControllerAndFormData(userId, centreSpecificEmailsByCentreId);

            A.CallTo(() => userDataService.PrimaryEmailIsInUse(Email, A<IDbTransaction?>._)).Returns(false);

            A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        A<DelegateDetailsData>._,
                        A<string?>._,
                        A<int>._,
                        A<bool>._
                    )
                )
                .DoesNothing();

            // When
            var result = myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        A<DelegateDetailsData?>._,
                        A<string>._,
                        A<int>._,
                        A<bool>._
                    )
                )
                .MustNotHaveHappened();

            A.CallTo(
                () => userService.UpdateUserDetails(
                    A<EditAccountDetailsData>.That.Matches(
                        e =>
                            e.FirstName == formData.FirstName &&
                            e.Surname == formData.LastName &&
                            e.Email == formData.Email &&
                            e.UserId == userId &&
                            e.JobGroupId == formData.JobGroupId &&
                            e.ProfessionalRegistrationNumber == formData.ProfessionalRegistrationNumber &&
                            e.ProfileImage == formData.ProfileImage
                    ),
                    true,
                    null
                )
            ).MustHaveHappened();

            A.CallTo(
                () => userService.SetCentreEmails(
                    userId,
                    A<Dictionary<int, string?>>.That.IsSameSequenceAs(centreSpecificEmailsByCentreId)
                )
            ).MustHaveHappened();

            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void EditDetailsPost_with_no_centreId_and_bad_centre_specific_emails_fails_validation()
        {
            // Given
            const int userId = 2;
            var centreSpecificEmailsByCentreId = new Dictionary<int, string?>
            {
                { 1, "email @centre1.com" },
                { 2, "email2" },
            };

            var (myAccountController, formData) =
                GetCentrelessControllerAndFormData(userId, centreSpecificEmailsByCentreId);
            var expectedModel = GetBasicMyAccountEditDetailsViewModel(formData);

            myAccountController.ModelState.AddModelError(nameof(MyAccountEditDetailsFormData.Email), "Required");

            // When
            var result = myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

            // Then
            result.As<ViewResult>().Model.As<MyAccountEditDetailsViewModel>().Should().BeEquivalentTo(expectedModel);

            myAccountController
                .ModelState[$"{nameof(MyAccountEditDetailsFormData.AllCentreSpecificEmailsDictionary)}_1"]
                .ValidationState
                .Should().Be
                    (ModelValidationState.Invalid);

            myAccountController
                .ModelState[$"{nameof(MyAccountEditDetailsFormData.AllCentreSpecificEmailsDictionary)}_2"]
                .ValidationState
                .Should().Be
                    (ModelValidationState.Invalid);

            A.CallTo(
                () => userService.UpdateUserDetails(
                    A<EditAccountDetailsData>._,
                    A<bool>._,
                    A<DateTime?>._
                )
            ).MustNotHaveHappened();

            A.CallTo(
                () => userService.SetCentreEmails(A<int>._, A<Dictionary<int, string?>>._)
            ).MustNotHaveHappened();
        }

        private MyAccountController GetMyAccountController()
        {
            return new MyAccountController(
                centreRegistrationPromptsService,
                userService,
                userDataService,
                imageResizeService,
                jobGroupsDataService,
                promptsService,
                logger,
                config
            ).WithDefaultContext();
        }

        private MyAccountEditDetailsFormData GetBasicMyAccountEditDetailsFormData()
        {
            return new MyAccountEditDetailsFormData
            {
                FirstName = "Test",
                LastName = "User",
                Email = Email,
                JobGroupId = 1,
                HasProfessionalRegistrationNumber = false,
            };
        }

        private (MyAccountController, MyAccountEditDetailsFormData) GetCentrelessControllerAndFormData(
            int userId,
            Dictionary<int, string?> centreSpecificEmailsByCentreId
        )
        {
            var myAccountController = GetMyAccountController().WithMockUser(true, null, null, null, userId: userId);

            var formData = GetBasicMyAccountEditDetailsFormData();
            formData.AllCentreSpecificEmailsDictionary = centreSpecificEmailsByCentreId.ToDictionary(
                row => row.Key.ToString(),
                row => row.Value
            );

            return (myAccountController, formData);
        }

        private static MyAccountEditDetailsViewModel GetBasicMyAccountEditDetailsViewModel(
            MyAccountEditDetailsFormData formData
        )
        {
            return new MyAccountEditDetailsViewModel(
                formData,
                new List<(int id, string name)>(),
                new List<EditDelegateRegistrationPromptViewModel>(),
                new List<(int, string, string?)>(),
                DlsSubApplication.Default
            );
        }
    }
}
