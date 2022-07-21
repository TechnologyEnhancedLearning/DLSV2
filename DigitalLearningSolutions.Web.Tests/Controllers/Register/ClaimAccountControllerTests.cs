namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.Register;
    using DigitalLearningSolutions.Web.Extensions;
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
        public void IndexGet_with_existing_user_returns_view_model()
        {
            // Given
            A.CallTo(
                () => userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    DefaultEmail,
                    DefaultCode
                )
            ).Returns((DefaultUserId, DefaultCentreId, DefaultCentreName));

            var expectedModel = GetViewModel();

            A.CallTo(
                () => claimAccountService.GetAccountDetailsForCompletingRegistration(
                    DefaultUserId,
                    DefaultCentreId,
                    DefaultCentreName,
                    DefaultEmail,
                    null
                )
            ).Returns(expectedModel);

            // When
            var result = controller.Index(DefaultEmail, DefaultCode);

            // Then
            result.Should().BeViewResult().ModelAs<ClaimAccountViewModel>().Should().BeEquivalentTo(expectedModel);
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
            var result = controller.Index(email, code);

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
        public void CompleteRegistrationGet_with_existing_user_returns_view_model()
        {
            // Given
            A.CallTo(
                () => userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    DefaultEmail,
                    DefaultCode
                )
            ).Returns((DefaultUserId, DefaultCentreId, DefaultCentreName));

            var expectedModel = GetViewModel();

            A.CallTo(
                () => claimAccountService.GetAccountDetailsForCompletingRegistration(
                    DefaultUserId,
                    DefaultCentreId,
                    DefaultCentreName,
                    DefaultEmail,
                    null
                )
            ).Returns(expectedModel);

            // When
            var result = controller.Index(DefaultEmail, DefaultCode);

            // Then
            result.Should().BeViewResult().ModelAs<ClaimAccountViewModel>().Should().BeEquivalentTo(expectedModel);
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
        public void CompleteRegistrationPost_with_email_in_use_returns_NotFound()
        {
            // Given
            controller.TempData.Set(GetViewModel());
            A.CallTo(() => userDataService.PrimaryEmailIsInUse(DefaultEmail)).Returns(true);

            // When
            var result = controller.CompleteRegistration();

            // Then
            A.CallTo(() => userDataService.PrimaryEmailIsInUse(DefaultEmail))
                .MustHaveHappenedOnceExactly();
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void
            CompleteRegistrationPost_with_primary_email_not_in_use_and_password_set_sets_expected_data_and_redirects_to_Confirmation()
        {
            // Given
            var model = GetViewModel(passwordSet: true);
            controller.TempData.Set(model);
            A.CallTo(() => userDataService.PrimaryEmailIsInUse(DefaultEmail)).Returns(false);

            // When
            var result = controller.CompleteRegistration();

            // Then
            A.CallTo(() => userDataService.PrimaryEmailIsInUse(DefaultEmail))
                .MustHaveHappenedOnceExactly();
            A.CallTo(
                () => claimAccountService.ConvertTemporaryUserToConfirmedUser(
                    DefaultUserId,
                    DefaultCentreId,
                    DefaultEmail
                )
            ).MustHaveHappenedOnceExactly();
            result.Should().BeViewResult()
                .WithViewName("Confirmation")
                .ModelAs<ClaimAccountViewModel>().Should().BeEquivalentTo(model);
        }

        [Test]
        public void CompleteRegistrationPost_with_logged_in_user_redirects_to_LinkDlsAccount()
        {
            // Given
            controllerWithLoggedInUser.TempData.Set(GetViewModel());

            // When
            var result = controllerWithLoggedInUser.CompleteRegistration();

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("LinkDlsAccount");
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
            var expectedModel = GetViewModel();
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount> { UserTestHelper.GetDefaultDelegateAccount(centreId: DefaultCentreId) }
            );

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
                    DefaultLoggedInUserId
                )
            ).Returns(expectedModel);

            A.CallTo(
                () => userService.GetUserById(DefaultLoggedInUserId)
            ).Returns(userEntity);

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
            var expectedModel = GetViewModel(emailIsTaken: true);

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
                    DefaultLoggedInUserId
                )
            ).Returns(expectedModel);

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccount(DefaultEmail, DefaultCode);

            // Then
            result.Should().BeRedirectToActionResult()
                .WithActionName("WrongUser");
        }

        [Test]
        public void LinkDlsAccountGet_with_existing_user_returns_view_model()
        {
            // Given
            var expectedModel = GetViewModel();

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
                    DefaultLoggedInUserId
                )
            ).Returns(expectedModel);

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccount(DefaultEmail, DefaultCode);

            // Then
            result.Should().BeViewResult().ModelAs<ClaimAccountViewModel>().Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void LinkDlsAccountPost_without_temp_date_redirects_to_AccessDenied()
        {
            // When
            var result = controllerWithLoggedInUser.LinkDlsAccount();

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void
            LinkDlsAccountPost_when_the_logged_in_user_already_has_a_delegate_account_at_the_centre_redirects_to_AccountAlreadyExists()
        {
            // Given
            var expectedModel = GetViewModel();
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount> { UserTestHelper.GetDefaultDelegateAccount(centreId: DefaultCentreId) }
            );

            controllerWithLoggedInUser.TempData.Set(expectedModel);

            A.CallTo(
                () => userService.GetUserById(DefaultLoggedInUserId)
            ).Returns(userEntity);

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccount();

            // Then
            result.Should().BeRedirectToActionResult()
                .WithActionName("AccountAlreadyExists");

            controllerWithLoggedInUser.TempData.Peek<ClaimAccountViewModel>()!.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void
            LinkDlsAccountPost_when_the_claim_email_address_matches_the_primary_email_of_another_user_redirects_to_WrongUser()
        {
            // Given
            var expectedModel = GetViewModel(emailIsTaken: true);

            controllerWithLoggedInUser.TempData.Set(expectedModel);

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccount();

            // Then
            result.Should().BeRedirectToActionResult()
                .WithActionName("WrongUser");

            controllerWithLoggedInUser.TempData.Peek<ClaimAccountViewModel>()!.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void
            LinkDlsAccountPost_with_valid_viewmodel_sets_expected_data_and_clears_temp_data_and_redirects_to_AccountsLinked()
        {
            // Given
            var expectedModel = GetViewModel();

            controllerWithLoggedInUser.TempData.Set(expectedModel);

            // When
            var result = controllerWithLoggedInUser.LinkDlsAccount();

            // Then
            A.CallTo(
                () => claimAccountService.LinkAccount(
                    DefaultUserId,
                    DefaultLoggedInUserId,
                    DefaultCentreId
                )
            ).MustHaveHappenedOnceExactly();

            result.Should().BeViewResult()
                .WithViewName("AccountsLinked")
                .ModelAs<ClaimAccountViewModel>().Should().BeEquivalentTo(expectedModel);

            controllerWithLoggedInUser.TempData.Should().BeEmpty();
        }

        [Test]
        public void
            WrongUser_clears_temp_data_and_displays_error_page()
        {
            // Given
            var expectedModel = GetViewModel();

            controllerWithLoggedInUser.TempData.Set(expectedModel);

            // When
            var result = controllerWithLoggedInUser.WrongUser();

            // Then
            result.Should().BeViewResult()
                .WithDefaultViewName()
                .ModelAs<ClaimAccountViewModel>().Should().BeEquivalentTo(expectedModel);

            controllerWithLoggedInUser.TempData.Should().BeEmpty();
        }

        [Test]
        public void
            WrongUser_with_no_temp_data_returns_NotFound()
        {
            // When
            var result = controllerWithLoggedInUser.WrongUser();

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void
            AccountAlreadyExists_clears_temp_data_and_displays_error_page()
        {
            // Given
            var expectedModel = GetViewModel();

            controllerWithLoggedInUser.TempData.Set(expectedModel);

            // When
            var result = controllerWithLoggedInUser.AccountAlreadyExists();

            // Then
            result.Should().BeViewResult()
                .WithDefaultViewName()
                .ModelAs<ClaimAccountViewModel>().Should().BeEquivalentTo(expectedModel);

            controllerWithLoggedInUser.TempData.Should().BeEmpty();
        }

        [Test]
        public void
            AccountAlreadyExists_with_no_temp_data_returns_NotFound()
        {
            // When
            var result = controllerWithLoggedInUser.AccountAlreadyExists();

            // Then
            result.Should().BeNotFoundResult();
        }

        private ClaimAccountController GetClaimAccountController()
        {
            return new ClaimAccountController(userService, userDataService, claimAccountService)
                .WithDefaultContext()
                .WithMockTempData();
        }

        private static ClaimAccountViewModel GetViewModel(
            int userId = DefaultUserId,
            int centreId = DefaultCentreId,
            string centreName = DefaultCentreName,
            string centreSpecificEmail = DefaultEmail,
            string registrationConfirmationHash = DefaultCode,
            string candidateNumber = DefaultCandidateNumber,
            string? supportEmail = null,
            bool emailIsTaken = false,
            bool emailIsTakenByActiveUser = false,
            bool passwordSet = false
        )
        {
            return new ClaimAccountViewModel
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
        }
    }
}
