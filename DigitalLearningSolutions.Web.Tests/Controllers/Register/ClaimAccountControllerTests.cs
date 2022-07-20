namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.Controllers.Register;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class ClaimAccountControllerTests
    {
        private const string DefaultEmail = "test@email.com";
        private const string DefaultCode = "code";
        private const int DefaultUserId = 2;
        private const int DefaultCentreId = 7;
        private const string DefaultCentreName = "Centre";
        private const string DefaultCandidateNumber = "CN777";
        private IUserService userService = null!;
        private IUserDataService userDataService = null!;
        private IClaimAccountService claimAccountService = null!;
        private ClaimAccountController controller = null!;

        [SetUp]
        public void Setup()
        {
            userService = A.Fake<IUserService>();
            userDataService = A.Fake<IUserDataService>();
            claimAccountService = A.Fake<IClaimAccountService>();
            controller = new ClaimAccountController(userService, userDataService, claimAccountService)
                .WithDefaultContext()
                .WithMockTempData();
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
            result.Should().BeViewResult().ModelAs<ClaimAccountViewModel>();
            ((ViewResult)result).ViewData.Model.Should().BeEquivalentTo(expectedModel);
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
            result.Should().BeViewResult().ModelAs<ClaimAccountViewModel>();
            ((ViewResult)result).ViewData.Model.Should().BeEquivalentTo(expectedModel);
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
            result.Should().BeViewResult().ModelAs<ClaimAccountViewModel>();
            ((ViewResult)result).ViewData.Model.Should().BeEquivalentTo(model);
            ((ViewResult)result).ViewName.Should().Be("Confirmation");
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
