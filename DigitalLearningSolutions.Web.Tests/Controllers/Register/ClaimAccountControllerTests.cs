namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.Register;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Register;
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
        private IUserService userService = null!;
        private IUserDataService userDataService = null!;
        private IClaimAccountService claimAccountService = null!;
        private ClaimAccountController controller = null!;
        private ClaimAccountController controllerWithLoggedInUser = null!;

        [SetUp]
        public void Setup()
        {
            userService = A.Fake<IUserService>();
            userDataService = A.Fake<IUserDataService>();
            claimAccountService = A.Fake<IClaimAccountService>();
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
            var model = GivenValidViewModel();

            // When
            var result = controller.Index(DefaultEmail, DefaultCode);

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
        public void IndexGet_with_no_existing_user_redirects_to_AccessDenied()
        {
            // Given
            A.CallTo(
                () => userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    DefaultEmail,
                    DefaultCode
                )
            ).Returns((null, null, null));

            // When
            var result = controller.Index(DefaultEmail, DefaultCode);

            // Then
            A.CallTo(
                () => userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    DefaultEmail,
                    DefaultCode
                )
            ).MustHaveHappenedOnceExactly();

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
        public void CompleteRegistrationGet_with_existing_user_returns_view_model()
        {
            // Given
            var model = GivenValidViewModel();

            // When
            var result = controller.CompleteRegistration(DefaultEmail, DefaultCode);

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
        public void CompleteRegistrationGet_with_no_existing_user_redirects_to_AccessDenied()
        {
            // Given
            A.CallTo(
                () => userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    DefaultEmail,
                    DefaultCode
                )
            ).Returns((null, null, null));

            // When
            var result = controller.CompleteRegistration(DefaultEmail, DefaultCode);

            // Then
            A.CallTo(
                () => userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    DefaultEmail,
                    DefaultCode
                )
            ).MustHaveHappenedOnceExactly();

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
        public void
            CompleteRegistrationPost_with_primary_email_not_in_use_and_password_set_sets_expected_data_and_redirects_to_Confirmation()
        {
            // Given
            GivenValidViewModel(passwordSet: true);

            A.CallTo(() => userDataService.PrimaryEmailIsInUse(DefaultEmail)).Returns(false);

            // When
            var result = controller.CompleteRegistrationPost(DefaultEmail, DefaultCode);

            // Then
            A.CallTo(
                () => claimAccountService.ConvertTemporaryUserToConfirmedUser(
                    DefaultUserId,
                    DefaultCentreId,
                    DefaultEmail
                )
            ).MustHaveHappenedOnceExactly();

            result.Should().BeRedirectToActionResult()
                .WithActionName("Confirmation")
                .WithRouteValue("email", DefaultEmail)
                .WithRouteValue("centreName", DefaultCentreName)
                .WithRouteValue("candidateNumber", DefaultCandidateNumber);
        }

        [Test]
        public void CompleteRegistrationPost_with_email_in_use_returns_NotFound()
        {
            // Given
            GivenValidViewModel();

            A.CallTo(() => userDataService.PrimaryEmailIsInUse(DefaultEmail)).Returns(true);

            // When
            var result = controller.CompleteRegistrationPost(DefaultEmail, DefaultCode);

            // Then
            A.CallTo(() => userDataService.PrimaryEmailIsInUse(DefaultEmail))
                .MustHaveHappenedOnceExactly();

            result.Should().BeNotFoundResult();
        }

        // TODO HEEDLS-975 Replace this test with the new behaviour
        [Test]
        public void CompleteRegistrationPost_with_password_not_set_returns_NotFound()
        {
            // Given
            GivenValidViewModel(passwordSet: false);

            // When
            var result = controller.CompleteRegistrationPost(DefaultEmail, DefaultCode);

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
        public void CompleteRegistrationPost_with_invalid_email_or_code_redirects_to_AccessDenied(
            string email,
            string code
        )
        {
            // When
            var result = controller.CompleteRegistrationPost(email, code);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void CompleteRegistrationPost_with_no_existing_user_redirects_to_AccessDenied()
        {
            // Given
            A.CallTo(
                () => userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    DefaultEmail,
                    DefaultCode
                )
            ).Returns((null, null, null));

            // When
            var result = controller.CompleteRegistrationPost(DefaultEmail, DefaultCode);

            // Then
            A.CallTo(
                () => userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    DefaultEmail,
                    DefaultCode
                )
            ).MustHaveHappenedOnceExactly();

            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void CompleteRegistrationPost_with_logged_in_user_redirects_to_LinkDlsAccount()
        {
            // When
            var result = controllerWithLoggedInUser.CompleteRegistrationPost(DefaultEmail, DefaultCode);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("LinkDlsAccount");
        }

        [Test]
        public void Confirmation_returns_view_model()
        {
            // Given
            var model = new ClaimAccountViewModel
            {
                Email = DefaultEmail,
                CentreName = DefaultCentreName,
                CandidateNumber = DefaultCandidateNumber,
            };

            // When
            var result = controller.Confirmation(DefaultEmail, DefaultCentreName, DefaultCandidateNumber);

            // Then
            result.Should().BeViewResult()
                .WithDefaultViewName()
                .ModelAs<ClaimAccountViewModel>().Should().BeEquivalentTo(model);
        }

        [Test]
        public void LinkDlsAccountGet_with_existing_account_to_be_claimed_returns_view_model()
        {
            // Given
            var model = GivenValidViewModel(loggedInUserId: DefaultLoggedInUserId);

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccount(DefaultEmail, DefaultCode);

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
        public void LinkDlsAccountGet_with_no_existing_user_redirects_to_AccessDenied()
        {
            // Given
            A.CallTo(
                () => userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    DefaultEmail,
                    DefaultCode
                )
            ).Returns((null, null, null));

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccount(DefaultEmail, DefaultCode);

            // Then
            A.CallTo(
                () => userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    DefaultEmail,
                    DefaultCode
                )
            ).MustHaveHappenedOnceExactly();

            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void
            LinkDlsAccountGet_when_the_logged_in_user_already_has_a_delegate_account_at_the_centre_redirects_to_AccountAlreadyExists()
        {
            // Given
            GivenValidViewModel(loggedInUserId: DefaultLoggedInUserId);
            GivenUserEntity();

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccount(DefaultEmail, DefaultCode);

            // Then
            A.CallTo(
                () => userService.GetUserById(DefaultLoggedInUserId)
            ).MustHaveHappenedOnceExactly();

            result.Should().BeRedirectToActionResult()
                .WithActionName("AccountAlreadyExists");
        }

        [Test]
        public void
            LinkDlsAccountGet_when_the_claim_email_address_matches_the_primary_email_of_another_user_redirects_to_WrongUser()
        {
            // Given
            var model = GivenValidViewModel(loggedInUserId: DefaultLoggedInUserId);
            model.EmailIsTaken = true;

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccount(DefaultEmail, DefaultCode);

            // Then
            result.Should().BeRedirectToActionResult()
                .WithActionName("WrongUser");
        }

        [Test]
        public void LinkDlsAccountPost_with_valid_viewmodel_sets_expected_data_and_redirects_to_AccountsLinked()
        {
            // Given
            GivenValidViewModel(loggedInUserId: DefaultLoggedInUserId);

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccountPost(DefaultEmail, DefaultCode);

            // Then
            A.CallTo(
                () => claimAccountService.LinkAccount(DefaultUserId, DefaultLoggedInUserId, DefaultCentreId)
            ).MustHaveHappenedOnceExactly();

            result.Should().BeRedirectToActionResult()
                .WithActionName("AccountsLinked")
                .WithRouteValue("centreName", DefaultCentreName);
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
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void LinkDlsAccountPost_with_no_existing_user_redirects_to_AccessDenied()
        {
            // Given
            A.CallTo(
                () => userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    DefaultEmail,
                    DefaultCode
                )
            ).Returns((null, null, null));

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccountPost(DefaultEmail, DefaultCode);

            // Then
            A.CallTo(
                () => userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    DefaultEmail,
                    DefaultCode
                )
            ).MustHaveHappenedOnceExactly();

            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void
            LinkDlsAccountPost_when_the_logged_in_user_already_has_a_delegate_account_at_the_centre_redirects_to_AccountAlreadyExists()
        {
            // Given
            GivenValidViewModel(loggedInUserId: DefaultLoggedInUserId);
            GivenUserEntity();

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccountPost(DefaultEmail, DefaultCode);

            // Then
            A.CallTo(() => userService.GetUserById(DefaultLoggedInUserId)).MustHaveHappenedOnceExactly();

            result.Should().BeRedirectToActionResult().WithActionName("AccountAlreadyExists");
        }

        [Test]
        public void
            LinkDlsAccountPost_when_the_claim_email_address_matches_the_primary_email_of_another_user_redirects_to_WrongUser()
        {
            // Given
            GivenValidViewModel(loggedInUserId: DefaultLoggedInUserId, emailIsTaken: true);

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccountPost(DefaultEmail, DefaultCode);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("WrongUser");
        }

        [Test]
        public void AccountsLinked_returns_view_model()
        {
            // Given
            var model = new ClaimAccountViewModel { CentreName = DefaultCentreName };

            // When
            var result = controller.AccountsLinked(DefaultCentreName);

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
            var result = controllerWithLoggedInUser.WrongUser(DefaultEmail, DefaultCentreName);

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
            var result = controllerWithLoggedInUser.AccountAlreadyExists(DefaultEmail, DefaultCentreName);

            // Then
            result.Should().BeViewResult()
                .WithDefaultViewName()
                .ModelAs<ClaimAccountViewModel>().Should().BeEquivalentTo(model);
        }

        private ClaimAccountViewModel GivenValidViewModel(
            int userId = DefaultUserId,
            int centreId = DefaultCentreId,
            string centreName = DefaultCentreName,
            string centreSpecificEmail = DefaultEmail,
            string registrationConfirmationHash = DefaultCode,
            string candidateNumber = DefaultCandidateNumber,
            string? supportEmail = null,
            bool emailIsTaken = false,
            bool emailIsTakenByActiveUser = false,
            bool passwordSet = false,
            int? loggedInUserId = null
        )
        {
            var model = new ClaimAccountViewModel
            {
                UserId = userId,
                CentreId = centreId,
                CentreName = centreName,
                Email = centreSpecificEmail,
                RegistrationConfirmationHash = registrationConfirmationHash,
                CandidateNumber = candidateNumber,
                SupportEmail = supportEmail,
                EmailIsTaken = emailIsTaken,
                EmailIsTakenByActiveUser = emailIsTakenByActiveUser,
                PasswordSet = passwordSet,
            };

            A.CallTo(
                () => userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    DefaultEmail,
                    DefaultCode
                )
            ).Returns((DefaultUserId, DefaultCentreId, DefaultCentreName));

            A.CallTo(
                () => claimAccountService.GetAccountDetailsForCompletingRegistration(
                    DefaultUserId,
                    DefaultCentreId,
                    DefaultCentreName,
                    DefaultEmail,
                    loggedInUserId
                )
            ).Returns(model);

            return model;
        }

        private void GivenUserEntity()
        {
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount> { UserTestHelper.GetDefaultDelegateAccount(centreId: DefaultCentreId) }
            );

            A.CallTo(
                () => userService.GetUserById(DefaultLoggedInUserId)
            ).Returns(userEntity);
        }

        private ClaimAccountController GetClaimAccountController()
        {
            return new ClaimAccountController(userService, userDataService, claimAccountService)
                .WithDefaultContext()
                .WithMockTempData();
        }
    }
}
