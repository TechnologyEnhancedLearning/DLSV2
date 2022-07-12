namespace DigitalLearningSolutions.Web.Tests.Controllers.MyAccount
{
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
            var myAccountController = new MyAccountController(
                centreRegistrationPromptsService,
                userService,
                userDataService,
                imageResizeService,
                jobGroupsDataService,
                promptsService,
                logger,
                config
            ).WithDefaultContext().WithMockUser(true);

            // When
            var result = myAccountController.Index(DlsSubApplication.Default);

            // Then
            const string expectedReturnUrl = "/Home/Welcome";
            result.As<ViewResult>().Model.As<MyAccountViewModel>().SwitchCentreReturnUrl.Should()
                .BeEquivalentTo(expectedReturnUrl);
        }

        [Test]
        public void EditDetailsPostSave_with_invalid_model_doesnt_call_services()
        {
            // Given
            var myAccountController = new MyAccountController(
                centreRegistrationPromptsService,
                userService,
                userDataService,
                imageResizeService,
                jobGroupsDataService,
                promptsService,
                logger,
                config
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
            A.CallTo(() => userDataService.PrimaryEmailIsInUseByOtherUser(A<string>._, A<int>._, A<IDbTransaction?>._))
                .MustNotHaveHappened();
            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    A<string>._,
                    A<int>._,
                    A<int>._,
                    A<IDbTransaction?>._
                )
            ).MustNotHaveHappened();
            result.As<ViewResult>().Model.As<MyAccountEditDetailsViewModel>().Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void EditDetailsPostSave_with_missing_delegate_answers_fails_validation()
        {
            // Given
            var myAccountController = new MyAccountController(
                centreRegistrationPromptsService,
                userService,
                userDataService,
                imageResizeService,
                jobGroupsDataService,
                promptsService,
                logger,
                config
            ).WithDefaultContext().WithMockUser(true, adminId: null);
            var customPromptLists = new List<CentreRegistrationPrompt>
                { PromptsTestHelper.GetDefaultCentreRegistrationPrompt(1, mandatory: true) };
            A.CallTo
                (() => centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(2)).Returns(
                PromptsTestHelper.GetDefaultCentreRegistrationPrompts(customPromptLists, 2)
            );
            var testUserEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new AdminAccount[] { },
                new[] { UserTestHelper.GetDefaultDelegateAccount() }
            );
            A.CallTo
                (() => userService.GetUserById(A<int>._)).Returns(testUserEntity);
            var formData = new MyAccountEditDetailsFormData();
            var expectedPrompt = new EditDelegateRegistrationPromptViewModel(
                1,
                "Custom Prompt",
                true,
                new List<string>(),
                null
            );
            var expectedModel = new MyAccountEditDetailsViewModel(
                formData,
                new List<(int id, string name)>(),
                new List<EditDelegateRegistrationPromptViewModel> { expectedPrompt },
                DlsSubApplication.Default
            );

            // When
            var result = myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(() => userDataService.PrimaryEmailIsInUseByOtherUser(A<string>._, A<int>._, A<IDbTransaction?>._))
                .MustNotHaveHappened();
            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    A<string>._,
                    A<int>._,
                    A<int>._,
                    A<IDbTransaction?>._
                )
            ).MustNotHaveHappened();
            result.As<ViewResult>().Model.As<MyAccountEditDetailsViewModel>().Should().BeEquivalentTo(expectedModel);
            myAccountController.ModelState[nameof(MyAccountEditDetailsFormData.Answer1)].ValidationState.Should().Be
                (ModelValidationState.Invalid);
        }

        [Test]
        [TestCase("primary@email.com", null, true)]
        [TestCase("primary@email.com", null, false)]
        [TestCase("primary@email.com", "centre@email.com", true, false)]
        [TestCase("primary@email.com", "centre@email.com", false, true)]
        [TestCase("primary@email.com", "centre@email.com", true, true)]
        [TestCase("primary@email.com", "centre@email.com", false, false)]
        public void EditDetailsPostSave_with_duplicate_email_fails_validation(
            string primaryEmail,
            string? centreSpecificEmail,
            bool primaryEmailIsDuplicate,
            bool centreEmailIsDuplicate = false
        )
        {
            // Given
            const int userId = 2;
            const int centreId = 2;
            var myAccountController = new MyAccountController(
                    centreRegistrationPromptsService,
                    userService,
                    userDataService,
                    imageResizeService,
                    jobGroupsDataService,
                    promptsService,
                    logger,
                    config
                ).WithDefaultContext()
                .WithMockUser(true, centreId, userId: userId, delegateId: null);
            var parameterName = typeof(MyAccountController).GetMethod("Index")?.GetParameters()
                .SingleOrDefault(p => p.ParameterType == typeof(DlsSubApplication))?.Name;

            A.CallTo(() => userDataService.PrimaryEmailIsInUseByOtherUser(primaryEmail, userId, A<IDbTransaction?>._))
                .Returns(primaryEmailIsDuplicate);

            if (centreSpecificEmail != null)
            {
                A.CallTo(
                        () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                            centreSpecificEmail,
                            centreId,
                            userId,
                            A<IDbTransaction?>._
                        )
                    )
                    .Returns(centreEmailIsDuplicate);
            }

            var formData = new MyAccountEditDetailsFormData
            {
                FirstName = "Test",
                LastName = "User",
                Email = primaryEmail,
                CentreSpecificEmail = centreSpecificEmail,
                JobGroupId = 1,
                HasProfessionalRegistrationNumber = false,
            };

            var expectedModel = new MyAccountEditDetailsViewModel(
                formData,
                new List<(int id, string name)>(),
                new List<EditDelegateRegistrationPromptViewModel>(),
                DlsSubApplication.Default
            );

            // When
            var result = myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(
                () => userDataService.PrimaryEmailIsInUseByOtherUser(primaryEmail, userId, A<IDbTransaction?>._)
            ).MustHaveHappened();

            if (centreSpecificEmail == null)
            {
                A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        A<string>._,
                        A<int>._,
                        A<int>._,
                        A<IDbTransaction?>._
                    )
                ).MustNotHaveHappened();
            }
            else
            {
                A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        centreSpecificEmail,
                        centreId,
                        userId,
                        A<IDbTransaction?>._
                    )
                ).MustHaveHappened();
            }

            if (primaryEmailIsDuplicate)
            {
                myAccountController.ModelState[nameof(MyAccountEditDetailsFormData.Email)].ValidationState.Should().Be
                    (ModelValidationState.Invalid);

                if (centreEmailIsDuplicate)
                {
                    myAccountController.ModelState[nameof(MyAccountEditDetailsFormData.CentreSpecificEmail)]
                        .ValidationState.Should().Be
                            (ModelValidationState.Invalid);
                }
            }

            if (primaryEmailIsDuplicate || centreEmailIsDuplicate)
            {
                result.As<ViewResult>().Model.As<MyAccountEditDetailsViewModel>().Should()
                    .BeEquivalentTo(expectedModel);
            }
            else
            {
                result.Should().BeRedirectToActionResult().WithActionName("Index").WithRouteValue(
                    parameterName,
                    DlsSubApplication.Default.UrlSegment
                );
            }
        }

        [Test]
        public void
            EditDetailsPostSave_for_admin_only_user_with_missing_delegate_answers_doesnt_fail_validation_or_update_delegate()
        {
            // Given
            const int userId = 2;
            const int centreId = 2;
            var myAccountController = new MyAccountController(
                centreRegistrationPromptsService,
                userService,
                userDataService,
                imageResizeService,
                jobGroupsDataService,
                promptsService,
                logger,
                config
            ).WithDefaultContext().WithMockUser(true, userId: userId, centreId: centreId, delegateId: null);
            A.CallTo(() => userDataService.PrimaryEmailIsInUseByOtherUser(Email, userId, null)).Returns(false);
            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        Email,
                        centreId,
                        userId,
                        null
                    )
                )
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
            var testUserEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new[] { UserTestHelper.GetDefaultAdminAccount() },
                new DelegateAccount[] { }
            );
            A.CallTo
                (() => userService.GetUserById(A<int>._)).Returns(testUserEntity);
            const string centreSpecificEmail = "centre@email.com";
            var model = new MyAccountEditDetailsFormData
            {
                FirstName = "Test",
                LastName = "User",
                Email = Email,
                CentreSpecificEmail = centreSpecificEmail,
                JobGroupId = 1,
                HasProfessionalRegistrationNumber = false,
            };
            var parameterName = typeof(MyAccountController).GetMethod("Index")?.GetParameters()
                .SingleOrDefault(p => p.ParameterType == typeof(DlsSubApplication))?.Name;

            // When
            var result = myAccountController.EditDetails(model, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(() => userDataService.PrimaryEmailIsInUseByOtherUser(Email, userId, A<IDbTransaction>._))
                .MustHaveHappened();
            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        centreSpecificEmail,
                        centreId,
                        userId,
                        A<IDbTransaction>._
                    )
                )
                .MustHaveHappened();
            A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        null,
                        A<string?>._,
                        A<int>._,
                        true
                    )
                )
                .MustHaveHappened();

            result.Should().BeRedirectToActionResult().WithActionName("Index").WithRouteValue(
                parameterName,
                DlsSubApplication.Default.UrlSegment
            );
        }

        [Test]
        public void EditDetailsPostSave_with_valid_info_and_valid_return_url_redirects_to_return_url()
        {
            // Given
            const int userId = 2;
            const int centreId = 2;
            const string returnUrl = "/TrackingSystem/Centre/Dashboard";
            var myAccountController = new MyAccountController(
                    centreRegistrationPromptsService,
                    userService,
                    userDataService,
                    imageResizeService,
                    jobGroupsDataService,
                    promptsService,
                    logger,
                    config
                ).WithDefaultContext()
                .WithMockUser(true, centreId, userId: userId, delegateId: null)
                .WithMockUrlHelper(urlHelper);
            A.CallTo(() => userDataService.PrimaryEmailIsInUseByOtherUser(Email, userId, A<IDbTransaction?>._))
                .Returns(false);
            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        Email,
                        centreId,
                        userId,
                        A<IDbTransaction?>._
                    )
                )
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
            var model = new MyAccountEditDetailsFormData
            {
                FirstName = "Test",
                LastName = "User",
                Email = Email,
                JobGroupId = 1,
                HasProfessionalRegistrationNumber = false,
                ReturnUrl = returnUrl,
            };

            // When
            var result = myAccountController.EditDetails(model, "save", DlsSubApplication.Default);

            // Then
            result.Should().BeRedirectResult().WithUrl(returnUrl);
        }

        [Test]
        public void EditDetailsPostSave_with_valid_info_and_invalid_return_url_redirects_to_index()
        {
            // Given
            const int userId = 2;
            const int centreId = 2;
            var myAccountController = new MyAccountController(
                    centreRegistrationPromptsService,
                    userService,
                    userDataService,
                    imageResizeService,
                    jobGroupsDataService,
                    promptsService,
                    logger,
                    config
                ).WithDefaultContext()
                .WithMockUser(true, centreId, userId: userId, delegateId: null)
                .WithMockUrlHelper(urlHelper);
            A.CallTo(() => userDataService.PrimaryEmailIsInUseByOtherUser(Email, userId, A<IDbTransaction?>._))
                .Returns(false);
            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        Email,
                        centreId,
                        userId,
                        A<IDbTransaction?>._
                    )
                )
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
            A.CallTo(() => urlHelper.IsLocalUrl(A<string>._)).Returns(false);
            var model = new MyAccountEditDetailsFormData
            {
                FirstName = "Test",
                LastName = "User",
                Email = Email,
                JobGroupId = 1,
                HasProfessionalRegistrationNumber = false,
                ReturnUrl = "/TrackingSystem/Centre/Dashboard",
            };
            var parameterName = typeof(MyAccountController).GetMethod("Index")?.GetParameters()
                .SingleOrDefault(p => p.ParameterType == typeof(DlsSubApplication))?.Name;

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
            var myAccountController = new MyAccountController(
                centreRegistrationPromptsService,
                userService,
                userDataService,
                imageResizeService,
                jobGroupsDataService,
                promptsService,
                logger,
                config
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
            var expectedPrompt = new EditDelegateRegistrationPromptViewModel(
                1,
                "Custom Prompt",
                true,
                new List<string>(),
                null
            );
            var expectedModel = new MyAccountEditDetailsViewModel(
                formData,
                new List<(int id, string name)>(),
                new List<EditDelegateRegistrationPromptViewModel> { expectedPrompt },
                DlsSubApplication.Default
            );

            // When
            var result = myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(
                    () => userDataService.PrimaryEmailIsInUseByOtherUser(
                        A<string>._,
                        A<int>._,
                        A<IDbTransaction?>._
                    )
                )
                .MustNotHaveHappened();
            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    A<string>._,
                    A<int>._,
                    A<int>._,
                    A<IDbTransaction?>._
                )
            ).MustNotHaveHappened();
            result.As<ViewResult>().Model.As<MyAccountEditDetailsViewModel>().Should()
                .BeEquivalentTo(expectedModel);
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
                userDataService,
                imageResizeService,
                jobGroupsDataService,
                promptsService,
                logger,
                config
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
