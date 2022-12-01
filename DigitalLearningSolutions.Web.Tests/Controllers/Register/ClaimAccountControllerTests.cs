namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using DigitalLearningSolutions.Data.Utilities;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.Register;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Register.ClaimAccount;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class ClaimAccountControllerTests
    {
        private const string DefaultEmail = "test@email.com";
        private const string DefaultCode = "code";
        private const int DefaultUserId = 2;
        private const int DefaultLoggedInUserId = 3;
        private const int DefaultCentreId = 7;
        private const string DefaultCentreName = "Centre";
        private const string DefaultCandidateNumber = "CN777";
        private const string Password = "password";
        private IUserService userService = null!;
        private IUserDataService userDataService = null!;
        private IClaimAccountService claimAccountService = null!;
        private IEmailService emailService = null!;
        private IClockUtility clockUtility=null!;
        private ClaimAccountController controller = null!;
        private ClaimAccountController controllerWithLoggedInUser = null!;

        [SetUp]
        public void Setup()
        {
            userService = A.Fake<IUserService>();
            userDataService = A.Fake<IUserDataService>();
            claimAccountService = A.Fake<IClaimAccountService>();
            emailService = A.Fake<IEmailService>();
            clockUtility = A.Fake<IClockUtility>();
            controller = GetClaimAccountController();
            controllerWithLoggedInUser = GetClaimAccountController().WithMockUser(
                true,
                DefaultCentreId,
                userId: DefaultLoggedInUserId
            );
        }

        [Test]
        public void IndexGet_with_existing_user_returns_view_model()
        {
            // Given
            var model = GivenClaimAccountViewModel();

            // When
            var result = controller.Index(model.Email, model.RegistrationConfirmationHash);

            // Then
            result.Should().BeViewResult().ModelAs<ClaimAccountViewModel>().Should().BeEquivalentTo(model);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("   ", null)]
        [TestCase(null, "   ")]
        [TestCase("   ", "   ")]
        [TestCase(null, DefaultCode)]
        [TestCase("   ", DefaultCode)]
        [TestCase(DefaultEmail, null)]
        [TestCase(DefaultEmail, "   ")]
        public void IndexGet_with_invalid_email_or_code_redirects_to_AccessDenied(
            string email,
            string code
        )
        {
            // When
            var result = controller.Index(email, code);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void IndexGet_with_no_user_to_be_claimed_redirects_to_AccessDenied()
        {
            // Given
            GivenEmailAndCodeDoNotMatchAUserToBeClaimed(DefaultEmail, DefaultCode);

            // When
            var result = controller.Index(DefaultEmail, DefaultCode);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void IndexGet_with_logged_in_user_redirects_to_LinkDlsAccount()
        {
            // When
            var result = controllerWithLoggedInUser.Index("email", "code");

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("LinkDlsAccount");
        }

        [Test]
        [TestCase(true, "CompleteRegistrationWithoutPassword")]
        [TestCase(false, "CompleteRegistration")]
        public void CompleteRegistrationGet_with_existing_user_returns_correct_view_model(
            bool wasPasswordSetByAdmin,
            string expectedViewName
        )
        {
            // Given
            var model = GivenClaimAccountViewModel(wasPasswordSetByAdmin: wasPasswordSetByAdmin);
            var formData = new ClaimAccountCompleteRegistrationViewModel
            {
                Email = model.Email,
                Code = model.RegistrationConfirmationHash,
                CentreName = model.CentreName,
                WasPasswordSetByAdmin = model.WasPasswordSetByAdmin,
            };

            // When
            var result = controller.CompleteRegistration(model.Email, model.RegistrationConfirmationHash);

            // Then
            result.Should().BeViewResult()
                .WithViewName(expectedViewName)
                .ModelAs<ClaimAccountCompleteRegistrationViewModel>().Should().BeEquivalentTo(formData);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("   ", null)]
        [TestCase(null, "   ")]
        [TestCase("   ", "   ")]
        [TestCase(null, DefaultCode)]
        [TestCase("   ", DefaultCode)]
        [TestCase(DefaultEmail, null)]
        [TestCase(DefaultEmail, "   ")]
        public void CompleteRegistrationGet_with_invalid_email_or_code_redirects_to_AccessDenied(
            string email,
            string code
        )
        {
            // When
            var result = controller.CompleteRegistration(email, code);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void CompleteRegistrationGet_with_no_user_to_be_claimed_redirects_to_AccessDenied()
        {
            // Given
            GivenEmailAndCodeDoNotMatchAUserToBeClaimed(DefaultEmail, DefaultCode);

            // When
            var result = controller.CompleteRegistration(DefaultEmail, DefaultCode);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void CompleteRegistrationGet_with_logged_in_user_redirects_to_LinkDlsAccount()
        {
            // When
            var result = controllerWithLoggedInUser.CompleteRegistration("email", "code");

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("LinkDlsAccount");
        }

        [Test]
        public async Task
            CompleteRegistrationWithoutPassword_with_primary_email_not_in_use_and_password_set_by_admin_sets_expected_data_and_redirects_to_Confirmation()
        {
            // Given
            var model = GivenClaimAccountViewModel(wasPasswordSetByAdmin: true);

            A.CallTo(() => userDataService.PrimaryEmailIsInUse(model.Email)).Returns(false);

            // When
            var result = await controller.CompleteRegistrationWithoutPassword(
                model.Email,
                model.RegistrationConfirmationHash
            );

            // Then
            A.CallTo(
                () => claimAccountService.ConvertTemporaryUserToConfirmedUser(
                    model.UserId,
                    model.CentreId,
                    model.Email,
                    null
                )
            ).MustHaveHappenedOnceExactly();

            controller.TempData.Peek<ClaimAccountConfirmationViewModel>().Should().BeEquivalentTo(
                new ClaimAccountConfirmationViewModel
                {
                    Email = model.Email,
                    CentreName = model.CentreName,
                    CandidateNumber = model.CandidateNumber,
                    WasPasswordSetByAdmin = true,
                }
            );

            result.Should().BeRedirectToActionResult()
                .WithActionName("Confirmation");
        }

        [Test]
        public async Task CompleteRegistrationWithoutPassword_with_email_in_use_returns_NotFound()
        {
            // Given
            var model = GivenClaimAccountViewModel(wasPasswordSetByAdmin: true);

            A.CallTo(() => userDataService.PrimaryEmailIsInUse(model.Email)).Returns(true);

            // When
            var result = await controller.CompleteRegistrationWithoutPassword(
                model.Email,
                model.RegistrationConfirmationHash
            );

            // Then
            ACallToConvertTemporaryUserToConfirmedUserMustNotHaveHappened();

            result.Should().BeNotFoundResult();
        }

        [Test]
        public async Task CompleteRegistrationWithoutPassword_with_password_not_set_by_admin_returns_NotFound()
        {
            // Given
            var model = GivenClaimAccountViewModel(wasPasswordSetByAdmin: false);

            // When
            var result = await controller.CompleteRegistrationWithoutPassword(
                model.Email,
                model.RegistrationConfirmationHash
            );

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("   ", null)]
        [TestCase(null, "   ")]
        [TestCase("   ", "   ")]
        [TestCase(null, DefaultCode)]
        [TestCase("   ", DefaultCode)]
        [TestCase(DefaultEmail, null)]
        [TestCase(DefaultEmail, "   ")]
        public async Task CompleteRegistrationWithoutPassword_with_invalid_email_or_code_redirects_to_AccessDenied(
            string email,
            string code
        )
        {
            // Given
            var model = GivenClaimAccountViewModel(
                wasPasswordSetByAdmin: true,
                email: email,
                registrationConfirmationHash: code
            );

            // When
            var result = await controller.CompleteRegistrationWithoutPassword(
                model.Email,
                model.RegistrationConfirmationHash
            );

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public async Task CompleteRegistrationWithoutPassword_with_no_user_to_be_claimed_redirects_to_AccessDenied()
        {
            // Given
            var model = GivenClaimAccountViewModel(wasPasswordSetByAdmin: true);

            GivenEmailAndCodeDoNotMatchAUserToBeClaimed(model.Email, model.RegistrationConfirmationHash);

            // When
            var result = await controller.CompleteRegistrationWithoutPassword(
                model.Email,
                model.RegistrationConfirmationHash
            );

            // Then
            ACallToConvertTemporaryUserToConfirmedUserMustNotHaveHappened();

            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public async Task CompleteRegistrationWithoutPassword_with_logged_in_user_redirects_to_LinkDlsAccount()
        {
            // Given
            var model = GivenClaimAccountViewModel(wasPasswordSetByAdmin: true);

            // When
            var result = await controllerWithLoggedInUser.CompleteRegistrationWithoutPassword(
                model.Email,
                model.RegistrationConfirmationHash
            );

            // Then
            ACallToConvertTemporaryUserToConfirmedUserMustNotHaveHappened();

            result.Should().BeRedirectToActionResult().WithActionName("LinkDlsAccount");
        }

        [Test]
        public async Task
            CompleteRegistrationPost_with_primary_email_not_in_use_and_password_not_set_by_admin_and_password_provided_sets_expected_data_and_redirects_to_Confirmation()
        {
            // Given
            var model = GivenClaimAccountViewModel(wasPasswordSetByAdmin: false);
            var passwordFormData = new ConfirmPasswordViewModel { Password = Password };

            A.CallTo(() => userDataService.PrimaryEmailIsInUse(model.Email)).Returns(false);

            // When
            var result = await controller.CompleteRegistration(
                passwordFormData,
                model.Email,
                model.RegistrationConfirmationHash
            );

            // Then
            A.CallTo(
                () => claimAccountService.ConvertTemporaryUserToConfirmedUser(
                    model.UserId,
                    model.CentreId,
                    model.Email,
                    Password
                )
            ).MustHaveHappenedOnceExactly();

            controller.TempData.Peek<ClaimAccountConfirmationViewModel>().Should().BeEquivalentTo(
                new ClaimAccountConfirmationViewModel
                {
                    Email = model.Email,
                    CentreName = model.CentreName,
                    CandidateNumber = model.CandidateNumber,
                    WasPasswordSetByAdmin = false,
                }
            );

            result.Should().BeRedirectToActionResult()
                .WithActionName("Confirmation");
        }

        [Test]
        public async Task CompleteRegistrationPost_with_email_in_use_returns_NotFound()
        {
            // Given
            var model = GivenClaimAccountViewModel(wasPasswordSetByAdmin: false);

            A.CallTo(() => userDataService.PrimaryEmailIsInUse(model.Email)).Returns(true);

            // When
            var result = await controller.CompleteRegistration(
                new ConfirmPasswordViewModel(),
                model.Email,
                model.RegistrationConfirmationHash
            );

            // Then
            ACallToConvertTemporaryUserToConfirmedUserMustNotHaveHappened();

            result.Should().BeNotFoundResult();
        }

        [Test]
        public async Task CompleteRegistrationPost_with_password_set_by_admin_returns_NotFound()
        {
            // Given
            var model = GivenClaimAccountViewModel(wasPasswordSetByAdmin: true);

            // When
            var result = await controller.CompleteRegistration(
                new ConfirmPasswordViewModel(),
                model.Email,
                model.RegistrationConfirmationHash
            );

            // Then
            ACallToConvertTemporaryUserToConfirmedUserMustNotHaveHappened();

            result.Should().BeNotFoundResult();
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("   ", null)]
        [TestCase(null, "   ")]
        [TestCase("   ", "   ")]
        [TestCase(null, DefaultCode)]
        [TestCase("   ", DefaultCode)]
        [TestCase(DefaultEmail, null)]
        [TestCase(DefaultEmail, "   ")]
        public async Task CompleteRegistrationPost_with_invalid_email_or_code_redirects_to_AccessDenied(
            string email,
            string code
        )
        {
            // Given
            var model = GivenClaimAccountViewModel(
                wasPasswordSetByAdmin: false,
                email: email,
                registrationConfirmationHash: code
            );

            // When
            var result = await controller.CompleteRegistration(
                new ConfirmPasswordViewModel(),
                model.Email,
                model.RegistrationConfirmationHash
            );

            // Then
            ACallToConvertTemporaryUserToConfirmedUserMustNotHaveHappened();

            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public async Task CompleteRegistrationPost_with_no_user_to_be_claimed_redirects_to_AccessDenied()
        {
            // Given
            var model = GivenClaimAccountViewModel(wasPasswordSetByAdmin: false);

            GivenEmailAndCodeDoNotMatchAUserToBeClaimed(model.Email, model.RegistrationConfirmationHash);

            // When
            var result = await controller.CompleteRegistration(
                new ConfirmPasswordViewModel(),
                model.Email,
                model.RegistrationConfirmationHash
            );

            // Then
            ACallToConvertTemporaryUserToConfirmedUserMustNotHaveHappened();

            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public async Task CompleteRegistrationPost_with_logged_in_user_redirects_to_LinkDlsAccount()
        {
            // Given
            var model = GivenClaimAccountViewModel(wasPasswordSetByAdmin: false);

            // When
            var result = await controllerWithLoggedInUser.CompleteRegistration(
                new ConfirmPasswordViewModel(),
                model.Email,
                model.RegistrationConfirmationHash
            );

            // Then
            ACallToConvertTemporaryUserToConfirmedUserMustNotHaveHappened();

            result.Should().BeRedirectToActionResult().WithActionName("LinkDlsAccount");
        }

        [Test]
        public async Task CompleteRegistrationPost_with_invalid_model_display_with_validation_errors()
        {
            // Given
            var model = GivenClaimAccountViewModel(wasPasswordSetByAdmin: false);
            var completeRegistrationViewModel = new ClaimAccountCompleteRegistrationViewModel
            {
                Email = model.Email,
                Code = model.RegistrationConfirmationHash,
                CentreName = model.CentreName,
                WasPasswordSetByAdmin = model.WasPasswordSetByAdmin,
            };

            controller.ModelState.AddModelError("ConfirmPassword", "Required");

            A.CallTo(() => userDataService.PrimaryEmailIsInUse(model.Email)).Returns(false);

            // When
            var result = await controller.CompleteRegistration(
                new ConfirmPasswordViewModel { Password = Password },
                model.Email,
                model.RegistrationConfirmationHash
            );

            // Then
            ACallToConvertTemporaryUserToConfirmedUserMustNotHaveHappened();

            result.Should().BeViewResult()
                .WithDefaultViewName()
                .ModelAs<ClaimAccountCompleteRegistrationViewModel>().Should()
                .BeEquivalentTo(completeRegistrationViewModel);

            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [Test]
        public void Confirmation_clears_TempData_and_returns_view_model()
        {
            // Given
            var model = new ClaimAccountConfirmationViewModel
            {
                Email = DefaultEmail,
                CentreName = DefaultCentreName,
                CandidateNumber = DefaultCandidateNumber,
                WasPasswordSetByAdmin = true,
            };

            controller.TempData.Set(model);

            // When
            var result = controller.Confirmation();

            // Then
            controller.TempData.Peek<ClaimAccountConfirmationViewModel>().Should().BeNull();

            result.Should().BeViewResult()
                .WithDefaultViewName()
                .ModelAs<ClaimAccountConfirmationViewModel>().Should().BeEquivalentTo(model);
        }

        [Test]
        public void LinkDlsAccountGet_with_existing_account_to_be_claimed_returns_view_model()
        {
            // Given
            var model = GivenClaimAccountViewModel(loggedInUserId: DefaultLoggedInUserId);

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccount(model.Email, model.RegistrationConfirmationHash);

            // Then
            result.Should().BeViewResult().ModelAs<ClaimAccountViewModel>().Should().BeEquivalentTo(model);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("   ", null)]
        [TestCase(null, "   ")]
        [TestCase("   ", "   ")]
        [TestCase(null, DefaultCode)]
        [TestCase("   ", DefaultCode)]
        [TestCase(DefaultEmail, null)]
        [TestCase(DefaultEmail, "   ")]
        public void LinkDlsAccountGet_with_invalid_email_or_code_redirects_to_AccessDenied(
            string email,
            string code
        )
        {
            // When
            var result = controllerWithLoggedInUser.LinkDlsAccount(email, code);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void LinkDlsAccountGet_with_no_user_to_be_claimed_redirects_to_AccessDenied()
        {
            // Given
            GivenEmailAndCodeDoNotMatchAUserToBeClaimed(DefaultEmail, DefaultCode);

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccount(DefaultEmail, DefaultCode);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void
            LinkDlsAccountGet_when_the_logged_in_user_already_has_a_delegate_account_at_the_centre_redirects_to_AccountAlreadyExists()
        {
            // Given
            var model = GivenClaimAccountViewModel(loggedInUserId: DefaultLoggedInUserId, centreId: DefaultCentreId);
            GivenLoggedInUserWithDelegateAccountAtCentre(DefaultLoggedInUserId, DefaultCentreId);

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccount(model.Email, model.RegistrationConfirmationHash);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("AccountAlreadyExists");
        }

        [Test]
        public void
            LinkDlsAccountGet_when_the_claim_email_address_matches_the_primary_email_of_another_user_redirects_to_WrongUser()
        {
            // Given
            var model = GivenClaimAccountViewModel(loggedInUserId: 3, idOfUserMatchingEmailIfAny: 2);

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccount(model.Email, model.RegistrationConfirmationHash);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("WrongUser");
        }

        [Test]
        public void LinkDlsAccountPost_with_valid_viewmodel_sets_expected_data_and_redirects_to_AccountsLinked()
        {
            // Given
            var model = GivenClaimAccountViewModel(loggedInUserId: DefaultLoggedInUserId);

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccountPost(
                model.Email,
                model.RegistrationConfirmationHash
            );

            // Then
            A.CallTo(
                () => claimAccountService.LinkAccount(model.UserId, DefaultLoggedInUserId, model.CentreId)
            ).MustHaveHappenedOnceExactly();

            result.Should().BeRedirectToActionResult()
                .WithActionName("AccountsLinked")
                .WithRouteValue("centreName", model.CentreName);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("   ", null)]
        [TestCase(null, "   ")]
        [TestCase("   ", "   ")]
        [TestCase(null, DefaultCode)]
        [TestCase("   ", DefaultCode)]
        [TestCase(DefaultEmail, null)]
        [TestCase(DefaultEmail, "   ")]
        public void LinkDlsAccountPost_with_invalid_email_or_code_redirects_to_AccessDenied(
            string email,
            string code
        )
        {
            // When
            var result = controllerWithLoggedInUser.LinkDlsAccountPost(email, code);

            // Then
            ACallToLinkAccountMustNotHaveHappened();

            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void LinkDlsAccountPost_with_no_user_to_be_claimed_redirects_to_AccessDenied()
        {
            // Given
            GivenEmailAndCodeDoNotMatchAUserToBeClaimed(DefaultEmail, DefaultCode);

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccountPost(DefaultEmail, DefaultCode);

            // Then
            ACallToLinkAccountMustNotHaveHappened();

            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void
            LinkDlsAccountPost_when_the_logged_in_user_already_has_a_delegate_account_at_the_centre_redirects_to_AccountAlreadyExists()
        {
            // Given
            var model = GivenClaimAccountViewModel(loggedInUserId: DefaultLoggedInUserId, centreId: DefaultCentreId);
            GivenLoggedInUserWithDelegateAccountAtCentre(DefaultLoggedInUserId, DefaultCentreId);

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccountPost(
                model.Email,
                model.RegistrationConfirmationHash
            );

            // Then
            ACallToLinkAccountMustNotHaveHappened();

            result.Should().BeRedirectToActionResult().WithActionName("AccountAlreadyExists");
        }

        [Test]
        public void
            LinkDlsAccountPost_when_the_claim_email_address_matches_the_primary_email_of_another_user_redirects_to_WrongUser()
        {
            // Given
            var model = GivenClaimAccountViewModel(loggedInUserId: 3, idOfUserMatchingEmailIfAny: 2);

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccountPost(
                model.Email,
                model.RegistrationConfirmationHash
            );

            // Then
            ACallToLinkAccountMustNotHaveHappened();

            result.Should().BeRedirectToActionResult().WithActionName("WrongUser");
        }

        [Test]
        public void AccountsLinked_returns_view_model()
        {
            // Given
            var model = new ClaimAccountViewModel { CentreName = DefaultCentreName };

            // When
            var result = controller.AccountsLinked(model.CentreName);

            // Then
            result.Should().BeViewResult()
                .WithDefaultViewName()
                .ModelAs<ClaimAccountViewModel>().Should().BeEquivalentTo(model);
        }

        [Test]
        public void WrongUser_returns_view_model()
        {
            // Given
            var model = new ClaimAccountViewModel { Email = DefaultEmail, CentreName = DefaultCentreName };

            // When
            var result = controllerWithLoggedInUser.WrongUser(model.Email, model.CentreName);

            // Then
            result.Should().BeViewResult()
                .WithDefaultViewName()
                .ModelAs<ClaimAccountViewModel>().Should().BeEquivalentTo(model);
        }

        [Test]
        public void AccountAlreadyExists_returns_view_model()
        {
            // Given
            var model = new ClaimAccountViewModel { Email = DefaultEmail, CentreName = DefaultCentreName };

            // When
            var result = controllerWithLoggedInUser.AccountAlreadyExists(model.Email, model.CentreName);

            // Then
            result.Should().BeViewResult()
                .WithDefaultViewName()
                .ModelAs<ClaimAccountViewModel>().Should().BeEquivalentTo(model);
        }

        private ClaimAccountViewModel GivenClaimAccountViewModel(
            int userId = DefaultUserId,
            int centreId = DefaultCentreId,
            string centreName = DefaultCentreName,
            string email = DefaultEmail,
            string registrationConfirmationHash = DefaultCode,
            string candidateNumber = DefaultCandidateNumber,
            string? supportEmail = null,
            int? idOfUserMatchingEmailIfAny = null,
            bool userMatchingEmailIsActive = false,
            bool wasPasswordSetByAdmin = false,
            int? loggedInUserId = null
        )
        {
            var model = new ClaimAccountViewModel
            {
                UserId = userId,
                CentreId = centreId,
                CentreName = centreName,
                Email = email,
                RegistrationConfirmationHash = registrationConfirmationHash,
                CandidateNumber = candidateNumber,
                SupportEmail = supportEmail,
                IdOfUserMatchingEmailIfAny = idOfUserMatchingEmailIfAny,
                UserMatchingEmailIsActive = userMatchingEmailIsActive,
                WasPasswordSetByAdmin = wasPasswordSetByAdmin,
            };

            A.CallTo(
                () => userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    email,
                    registrationConfirmationHash
                )
            ).Returns((userId, centreId, centreName));

            A.CallTo(
                () => claimAccountService.GetAccountDetailsForClaimAccount(
                    userId,
                    centreId,
                    centreName,
                    email,
                    loggedInUserId
                )
            ).Returns(model);

            return model;
        }

        private void GivenLoggedInUserWithDelegateAccountAtCentre(int userId, int centreIdForDelegateAccount)
        {
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount>
                    { UserTestHelper.GetDefaultDelegateAccount(centreId: centreIdForDelegateAccount) }
            );

            A.CallTo(
                () => userService.GetUserById(userId)
            ).Returns(userEntity);
        }

        private void GivenEmailAndCodeDoNotMatchAUserToBeClaimed(string email, string code)
        {
            A.CallTo(
                () => userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    email,
                    code
                )
            ).Returns((null, null, null));
        }

        private void ACallToConvertTemporaryUserToConfirmedUserMustNotHaveHappened()
        {
            A.CallTo(
                () => claimAccountService.ConvertTemporaryUserToConfirmedUser(
                    A<int>._,
                    A<int>._,
                    A<string>._,
                    A<string?>._
                )
            ).MustNotHaveHappened();
        }

        private void ACallToLinkAccountMustNotHaveHappened()
        {
            A.CallTo(
                () => claimAccountService.LinkAccount(A<int>._, A<int>._, A<int>._)
            ).MustNotHaveHappened();
        }

        private ClaimAccountController GetClaimAccountController()
        {
            return new ClaimAccountController(userService, userDataService, claimAccountService,emailService,clockUtility)
                .WithDefaultContext()
                .WithMockTempData();
        }
    }
}
