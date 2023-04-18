namespace DigitalLearningSolutions.Web.Tests.Controllers.MyAccount
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authentication;
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
        private IEmailVerificationService emailVerificationService = null!;
        private IImageResizeService imageResizeService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private ILogger<MyAccountController> logger = null!;
        private PromptsService promptsService = null!;
        private IUrlHelper urlHelper = null!;
        private IUserDataService userDataService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            centreRegistrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            config = A.Fake<IConfiguration>();
            userService = A.Fake<IUserService>();
            userDataService = A.Fake<IUserDataService>();
            imageResizeService = A.Fake<ImageResizeService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            emailVerificationService = A.Fake<IEmailVerificationService>();
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
        public async Task EditDetailsPostSave_with_invalid_model_doesnt_call_services()
        {
            // Given
            var myAccountController = GetMyAccountController().WithMockUser(true, null);
            var formData = new MyAccountEditDetailsFormData();
            var expectedModel = GetBasicMyAccountEditDetailsViewModel(formData, null);

            myAccountController.ModelState.AddModelError(nameof(MyAccountEditDetailsFormData.Email), "Required");

            // When
            var result = await myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(
                () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                    A<EditAccountDetailsData>._,
                    A<DelegateDetailsData?>._,
                    A<string?>._,
                    A<int>._,
                    A<bool>._,
                    A<bool>._,
                    A<bool>._
                )
            ).MustNotHaveHappened();

            A.CallTo(
                () => userService.UpdateUserDetails(
                    A<EditAccountDetailsData>._,
                    A<bool>._,
                    A<bool>._,
                    A<DateTime?>._
                )
            ).MustNotHaveHappened();

            A.CallTo(
                () => userService.SetCentreEmails(A<int>._, A<Dictionary<int, string?>>._, A<List<UserCentreDetails>>._)
            ).MustNotHaveHappened();

            A.CallTo(
                () => emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                    A<UserAccount>._,
                    A<List<string>>._,
                    A<string>._
                )
            ).MustNotHaveHappened();

            result.As<ViewResult>().Model.As<MyAccountEditDetailsViewModel>().Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public async Task EditDetailsPostSave_with_missing_delegate_answers_fails_validation()
        {
            // Given
            const int centreId = 2;
            var myAccountController = GetMyAccountController().WithMockUser(true, centreId, null);

            var customPromptLists = new List<CentreRegistrationPrompt>
            {
                PromptsTestHelper.GetDefaultCentreRegistrationPrompt(1, mandatory: true),
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

            var expectedModel = GetBasicMyAccountEditDetailsViewModel(formData, centreId);
            expectedModel.DelegateRegistrationPrompts.Add(expectedPrompt);

            A.CallTo(() => userService.GetUserById(A<int>._)).Returns(testUserEntity);
            A.CallTo(() => centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(2)).Returns(
                PromptsTestHelper.GetDefaultCentreRegistrationPrompts(customPromptLists, 2)
            );

            // When
            var result = await myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(() => userDataService.PrimaryEmailIsInUseByOtherUser(A<string>._, A<int>._))
                .MustNotHaveHappened();

            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    A<string>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();

            result.As<ViewResult>().Model.As<MyAccountEditDetailsViewModel>().Should().BeEquivalentTo(expectedModel);

            myAccountController.ModelState[nameof(MyAccountEditDetailsFormData.Answer1)].ValidationState.Should().Be
                (ModelValidationState.Invalid);
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public async Task EditDetailsPostSave_with_duplicate_email_fails_validation(
            bool centreSpecificEmailIsNull,
            bool primaryEmailIsDuplicate,
            bool centreEmailIsDuplicate = false
        )
        {
            // Given
            const string primaryEmail = "primary@email.com";
            const int userId = 2;
            const int centreId = 2;
            var centreSpecificEmail = centreSpecificEmailIsNull ? null : "centre@email.com";
            var myAccountController = GetMyAccountController().WithMockUser(
                true,
                centreId,
                userId: userId,
                delegateId: null
            );

            GetAuthenticationServiceAuthenticateAsyncReturnsSuccess(myAccountController, false);

            A.CallTo(() => userService.GetUserById(userId)).Returns(
                new UserEntity(
                    UserTestHelper.GetDefaultUserAccount(),
                    new AdminAccount[] { },
                    new[] { UserTestHelper.GetDefaultDelegateAccount() }
                )
            );

            A.CallTo(() => userDataService.PrimaryEmailIsInUseByOtherUser(primaryEmail, userId))
                .Returns(primaryEmailIsDuplicate);

            if (centreSpecificEmail != null)
            {
                A.CallTo(
                        () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                            centreSpecificEmail,
                            centreId,
                            userId
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

            var expectedModel = GetBasicMyAccountEditDetailsViewModel(formData, centreId);

            // When
            var result = await myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

            // Then
            if (primaryEmailIsDuplicate)
            {
                myAccountController.ModelState[nameof(MyAccountEditDetailsFormData.Email)].ValidationState.Should().Be
                    (ModelValidationState.Invalid);
            }

            if (centreEmailIsDuplicate)
            {
                myAccountController.ModelState[nameof(MyAccountEditDetailsFormData.CentreSpecificEmail)]
                    .ValidationState.Should().Be
                        (ModelValidationState.Invalid);
            }

            if (primaryEmailIsDuplicate || centreEmailIsDuplicate)
            {
                result.As<ViewResult>().Model.As<MyAccountEditDetailsViewModel>().Should()
                    .BeEquivalentTo(expectedModel);
            }
            else
            {
                result.Should().BeRedirectToActionResult().WithActionName("Index");
            }
        }

        [Test]
        public async Task EditDetailsPostSave_validates_duplicate_centre_specific_emails()
        {
            // Given
            const string primaryEmail = "primary@email.com";
            const int userId = 2;
            var myAccountController = GetMyAccountController()
                .WithMockUser(true, null, userId: userId, delegateId: null);

            var allCentreSpecificEmailsDictionary = new Dictionary<string, string?>
            {
                { "2", null },
                { "3", "email@centre3.com" },
                { "4", "reused_email@centre4.com" },
            };

            A.CallTo(() => userDataService.PrimaryEmailIsInUseByOtherUser(primaryEmail, userId)).Returns(false);

            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        "email@centre3.com",
                        3,
                        userId
                    )
                )
                .Returns(false);

            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        "reused_email@centre4.com",
                        4,
                        userId
                    )
                )
                .Returns(true);

            var formData = new MyAccountEditDetailsFormData
            {
                FirstName = "Test",
                LastName = "User",
                AllCentreSpecificEmailsDictionary = allCentreSpecificEmailsDictionary,
                JobGroupId = 1,
                HasProfessionalRegistrationNumber = false,
            };

            var expectedModel = GetBasicMyAccountEditDetailsViewModel(formData, null);

            // When
            var result = await myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        A<string>._,
                        2,
                        A<int>._
                    )
                )
                .MustNotHaveHappened();

            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        "email@centre3.com",
                        3,
                        userId
                    )
                )
                .MustHaveHappenedOnceExactly();

            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        "reused_email@centre4.com",
                        4,
                        userId
                    )
                )
                .MustHaveHappenedOnceExactly();

            myAccountController.ModelState[$"{nameof(formData.AllCentreSpecificEmailsDictionary)}_4"]
                .ValidationState.Should().Be
                    (ModelValidationState.Invalid);

            //myAccountController.ModelState.Count.Should().Be(1); // The values for centres 2 and 3 are not invalid
            myAccountController.ModelState.Count().Should().Be(2); //Since we are expecting 2 errors
            result.As<ViewResult>().Model.As<MyAccountEditDetailsViewModel>().Should()
                .BeEquivalentTo(expectedModel);

            var errorMessageSameEmail = result.As<ViewResult>().ViewData.ModelState.Select(x => x.Value.Errors)
                .Where(y => y.Count > 0).ToList().First().First().ErrorMessage;
            var errorMessageEmailAlreadyInUse = result.As<ViewResult>().ViewData.ModelState.Select(x => x.Value.Errors)
                .Where(y => y.Count > 0).ToList().Last().Last().ErrorMessage;
            errorMessageSameEmail.Should().BeEquivalentTo("Centre email is the same as primary email");
            errorMessageEmailAlreadyInUse.Should().BeEquivalentTo("This email is already in use by another user at the centre");
        }

        [Test]
        public async Task
            EditDetailsPostSave_for_admin_only_user_with_missing_delegate_answers_doesnt_fail_validation_or_update_delegate()
        {
            // Given
            const int userId = 2;
            const int centreId = 2;
            var myAccountController = GetMyAccountController().WithMockUser(
                true,
                userId: userId,
                centreId: centreId,
                delegateId: null
            );

            GetAuthenticationServiceAuthenticateAsyncReturnsSuccess(myAccountController, false);

            var model = GetBasicMyAccountEditDetailsFormData();

            var testUserEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new[] { UserTestHelper.GetDefaultAdminAccount() },
                new DelegateAccount[] { }
            );

            A.CallTo(() => userService.GetUserById(A<int>._)).Returns(testUserEntity);
            A.CallTo(() => userDataService.PrimaryEmailIsInUseByOtherUser(Email, userId)).Returns(false);
            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(Email, centreId, userId))
                .Returns(false);

            // When
            var result = await myAccountController.EditDetails(model, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        null, // null delegateDetailsData -> delegate account is not updated
                        A<string?>._,
                        A<int>._,
                        true,
                        false,
                        true
                    )
                )
                .MustHaveHappened();

            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public async Task EditDetailsPostSave_redirects_to_VerifyYourEmail_when_email_needs_verification()
        {
            // Given
            const int userId = 2;
            const int centreId = 2;
            const string newEmail = "unverified_email@test.com";

            var myAccountController = GetMyAccountController()
                .WithMockUser(true, centreId, userId: userId, delegateId: null)
                .WithMockUrlHelper(urlHelper);

            GetAuthenticationServiceAuthenticateAsyncReturnsSuccess(myAccountController, false);

            var testUserEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(primaryEmail: Email),
                new AdminAccount[] { },
                new[] { UserTestHelper.GetDefaultDelegateAccount() }
            );

            A.CallTo(() => userService.GetUserById(userId)).Returns(testUserEntity);

            var model = new MyAccountEditDetailsFormData
            {
                FirstName = testUserEntity.UserAccount.FirstName,
                LastName = testUserEntity.UserAccount.LastName,
                Email = newEmail,
                JobGroupId = testUserEntity.UserAccount.JobGroupId,
                HasProfessionalRegistrationNumber = false,
            };

            A.CallTo(() => userDataService.PrimaryEmailIsInUseByOtherUser(Email, userId))
                .Returns(false);

            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    Email,
                    centreId,
                    userId
                )
            ).Returns(false);

            A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        A<DelegateDetailsData>._,
                        A<string?>._,
                        A<int>._,
                        A<bool>._,
                        A<bool>._,
                        A<bool>._
                    )
                )
                .DoesNothing();

            // When
            var result = await myAccountController.EditDetails(model, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(
                () => emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                    testUserEntity.UserAccount,
                    A<List<string>>.That.Matches(
                        list => ListTestHelper.ListOfStringsMatch(
                            list,
                            new List<string> { newEmail }
                        )
                    ),
                    A<string>._
                )
            ).MustHaveHappenedOnceExactly();

            result.Should().BeRedirectToActionResult().WithControllerName("VerifyYourEmail").WithActionName("Index")
                .WithRouteValue("emailVerificationReason", EmailVerificationReason.EmailChanged);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public async Task
            EditDetailsPostSave_logs_out_of_centre_account_when_email_needs_verification_preserves_isPersistent(
                bool isPersistent
            )
        {
            // Given
            const int userId = 2;
            const int centreId = 2;

            var myAccountController = GetMyAccountController()
                .WithMockUser(true, centreId, userId: userId, delegateId: null)
                .WithMockUrlHelper(urlHelper);

            var authenticationService =
                GetAuthenticationServiceAuthenticateAsyncReturnsSuccess(myAccountController, isPersistent);

            var testUserEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new AdminAccount[] { },
                new[] { UserTestHelper.GetDefaultDelegateAccount() }
            );

            A.CallTo(() => userService.GetUserById(userId)).Returns(testUserEntity);

            var model = GetBasicMyAccountEditDetailsFormData();

            A.CallTo(() => userDataService.PrimaryEmailIsInUseByOtherUser(Email, userId))
                .Returns(false);

            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    Email,
                    centreId,
                    userId
                )
            ).Returns(false);

            A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        A<DelegateDetailsData>._,
                        A<string?>._,
                        A<int>._,
                        A<bool>._,
                        A<bool>._,
                        A<bool>._
                    )
                )
                .DoesNothing();

            // When
            await myAccountController.EditDetails(model, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(
                () => authenticationService.SignOutAsync(
                    myAccountController.HttpContext,
                    A<string>._,
                    A<AuthenticationProperties>._
                )
            ).MustHaveHappenedOnceExactly();

            A.CallTo(
                () => authenticationService.SignInAsync(
                    myAccountController.HttpContext,
                    "Identity.Application",
                    A<ClaimsPrincipal>._,
                    A<AuthenticationProperties>.That.Matches(props => props.IsPersistent == isPersistent)
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task
            EditDetailsPostSave_with_valid_info_and_valid_return_url_redirects_to_return_url_when_email_does_not_require_verification()
        {
            // Given
            const int userId = 2;
            const int centreId = 2;
            const string returnUrl = "/TrackingSystem/Centre/Dashboard";

            var myAccountController = GetMyAccountController()
                .WithMockUser(true, centreId, userId: userId, delegateId: null)
                .WithMockUrlHelper(urlHelper);

            var testUserEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(primaryEmail: Email, emailVerified: true),
                new[] { UserTestHelper.GetDefaultAdminAccount() },
                new DelegateAccount[] { }
            );

            var model = new MyAccountEditDetailsFormData
            {
                FirstName = testUserEntity.UserAccount.FirstName,
                LastName = testUserEntity.UserAccount.LastName,
                Email = Email,
                JobGroupId = testUserEntity.UserAccount.JobGroupId,
                HasProfessionalRegistrationNumber = false,
                ReturnUrl = returnUrl,
            };

            A.CallTo(() => userService.GetUserById(A<int>._)).Returns(testUserEntity);

            A.CallTo(() => userDataService.PrimaryEmailIsInUseByOtherUser(Email, userId))
                .Returns(false);

            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    Email,
                    centreId,
                    userId
                )
            ).Returns(false);

            A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        A<DelegateDetailsData>._,
                        A<string?>._,
                        A<int>._,
                        A<bool>._,
                        A<bool>._,
                        A<bool>._
                    )
                )
                .DoesNothing();

            A.CallTo(() => urlHelper.IsLocalUrl(returnUrl)).Returns(true);

            // When
            var result = await myAccountController.EditDetails(model, "save", DlsSubApplication.Default);

            // Then
            result.Should().BeRedirectResult().WithUrl(returnUrl);
        }

        [Test]
        public async Task
            EditDetailsPostSave_with_valid_info_and_invalid_return_url_redirects_to_index_when_email_does_not_require_verification()
        {
            // Given
            const int userId = 2;
            const int centreId = 2;
            var myAccountController = GetMyAccountController()
                .WithMockUser(true, delegateId: null)
                .WithMockUrlHelper(urlHelper);

            var testUserEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(primaryEmail: Email, emailVerified: true),
                new[] { UserTestHelper.GetDefaultAdminAccount() },
                new DelegateAccount[] { }
            );

            var model = new MyAccountEditDetailsFormData
            {
                FirstName = testUserEntity.UserAccount.FirstName,
                LastName = testUserEntity.UserAccount.LastName,
                Email = Email,
                JobGroupId = testUserEntity.UserAccount.JobGroupId,
                HasProfessionalRegistrationNumber = false,
                ReturnUrl = "/TrackingSystem/Centre/Dashboard",
            };

            A.CallTo(() => userService.GetUserById(A<int>._)).Returns(testUserEntity);

            var parameterName = typeof(MyAccountController).GetMethod("Index")?.GetParameters()
                .SingleOrDefault(p => p.ParameterType == typeof(DlsSubApplication))?.Name;

            A.CallTo(() => userDataService.PrimaryEmailIsInUseByOtherUser(Email, userId))
                .Returns(false);

            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    Email,
                    centreId,
                    userId
                )
            ).Returns(false);

            A.CallTo(() => urlHelper.IsLocalUrl(A<string>._)).Returns(false);

            A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        A<DelegateDetailsData>._,
                        A<string?>._,
                        A<int>._,
                        A<bool>._,
                        A<bool>._,
                        A<bool>._
                    )
                )
                .DoesNothing();

            // When
            var result = await myAccountController.EditDetails(model, "save", DlsSubApplication.Default);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Index").WithRouteValue(
                parameterName,
                DlsSubApplication.Default.UrlSegment
            );
        }

        [Test]
        public async Task EditDetailsPostSave_without_previewing_profile_image_fails_validation()
        {
            // Given
            const int centreId = 2;
            var myAccountController = GetMyAccountController().WithMockUser(true, centreId, null);

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

            var expectedModel = GetBasicMyAccountEditDetailsViewModel(formData, centreId);
            expectedModel.DelegateRegistrationPrompts.Add(expectedPrompt);

            A.CallTo(() => centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(2)).Returns(
                PromptsTestHelper.GetDefaultCentreRegistrationPrompts(customPromptLists, centreId)
            );

            // When
            var result = await myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

            // Then
            result.As<ViewResult>().Model.As<MyAccountEditDetailsViewModel>().Should().BeEquivalentTo(expectedModel);

            myAccountController.ModelState[nameof(MyAccountEditDetailsFormData.ProfileImageFile)].ValidationState
                .Should().Be(ModelValidationState.Invalid);
        }

        [Test]
        public async Task EditDetailsPost_with_unexpected_action_returns_error()
        {
            // Given
            var myAccountController = GetMyAccountController().WithMockUser(true);
            const string action = "unexpectedString";
            var model = new MyAccountEditDetailsFormData();

            // When
            var result = await myAccountController.EditDetails(model, action, DlsSubApplication.Default);

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        [Test]
        public async Task EditDetailsPost_with_no_centreId_updates_user_details_and_all_centre_specific_emails()
        {
            // Given
            const int userId = 2;
            const string centreEmail1 = "email@centre1.com";
            const string centreEmail2 = "email@centre2.com";
            var centreSpecificEmailsByCentreId = new Dictionary<int, string?>
            {
                { 1, centreEmail1 },
                { 2, centreEmail2 },
                { 3, null },
            };

            var (myAccountController, formData) =
                GetCentrelessControllerAndFormData(userId, centreSpecificEmailsByCentreId);

            var testUserEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new AdminAccount[] { },
                new[] { UserTestHelper.GetDefaultDelegateAccount() }
            );

            A.CallTo(() => userDataService.PrimaryEmailIsInUseByOtherUser(Email, userId)).Returns(false);

            A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        A<DelegateDetailsData>._,
                        A<string?>._,
                        A<int>._,
                        A<bool>._,
                        A<bool>._,
                        A<bool>._
                    )
                )
                .DoesNothing();

            A.CallTo(() => userService.GetUserById(userId)).Returns(testUserEntity);

            // When
            var result = await myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

            // Then
            A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        A<DelegateDetailsData?>._,
                        A<string>._,
                        A<int>._,
                        A<bool>._,
                        A<bool>._,
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
                    true,
                    null
                )
            ).MustHaveHappened();

            A.CallTo(
                () => userService.SetCentreEmails(
                    userId,
                    A<Dictionary<int, string?>>.That.IsSameSequenceAs(centreSpecificEmailsByCentreId),
                    A<List<UserCentreDetails>>._
                )
            ).MustHaveHappened();

            A.CallTo(
                () => emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                    testUserEntity.UserAccount,
                    A<List<string>>.That.Matches(
                        list => ListTestHelper.ListOfStringsMatch(
                            list,
                            new List<string> { Email, centreEmail1, centreEmail2 }
                        )
                    ),
                    A<string>._
                )
            ).MustHaveHappened();

            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public async Task EditDetailsPost_with_no_centreId_and_bad_centre_specific_emails_fails_validation()
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
            var expectedModel = GetBasicMyAccountEditDetailsViewModel(formData, null);

            myAccountController.ModelState.AddModelError(nameof(MyAccountEditDetailsFormData.Email), "Required");

            // When
            var result = await myAccountController.EditDetails(formData, "save", DlsSubApplication.Default);

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
                    A<bool>._,
                    A<DateTime?>._
                )
            ).MustNotHaveHappened();

            A.CallTo(
                () => userService.SetCentreEmails(A<int>._, A<Dictionary<int, string?>>._, A<List<UserCentreDetails>>._)
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
                emailVerificationService,
                promptsService,
                logger,
                config
            ).WithDefaultContext().WithMockServices().WithMockTempData();
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
            var myAccountController = GetMyAccountController().WithMockUser(true, null, null, null, userId);

            var formData = GetBasicMyAccountEditDetailsFormData();
            formData.AllCentreSpecificEmailsDictionary = centreSpecificEmailsByCentreId.ToDictionary(
                row => row.Key.ToString(),
                row => row.Value
            );

            return (myAccountController, formData);
        }

        private static MyAccountEditDetailsViewModel GetBasicMyAccountEditDetailsViewModel(
            MyAccountEditDetailsFormData formData,
            int? centreId
        )
        {
            return new MyAccountEditDetailsViewModel(
                formData,
                centreId,
                new List<(int id, string name)>(),
                new List<EditDelegateRegistrationPromptViewModel>(),
                new List<(int, string, string?)>(),
                DlsSubApplication.Default
            );
        }

        private static IAuthenticationService GetAuthenticationServiceAuthenticateAsyncReturnsSuccess(
            MyAccountController controller,
            bool isPersistent
        )
        {
            var authenticationService =
                (IAuthenticationService)controller.HttpContext.RequestServices.GetService(
                    typeof(IAuthenticationService)
                );

            A.CallTo(() => authenticationService.AuthenticateAsync(A<HttpContext>._, A<string>._)).Returns(
                AuthenticateResult.Success(
                    new AuthenticationTicket(
                        new ClaimsPrincipal(),
                        new AuthenticationProperties { IsPersistent = isPersistent },
                        "test"
                    )
                )
            );

            return authenticationService;
        }
    }
}
