namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using System.Collections.Generic;
    using System.Data;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.Register;
    using DigitalLearningSolutions.Web.Extensions;
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
        private const string DefaultPasswordHash = "hash";
        private const string DefaultCandidateNumber = "CN777";
        private const string DefaultSupportEmail = "support@email.com";
        private IUserDataService userDataService = null!;
        private IConfigDataService configDataService = null!;
        private ClaimAccountController controller = null!;

        [SetUp]
        public void Setup()
        {
            userDataService = A.Fake<IUserDataService>();
            configDataService = A.Fake<IConfigDataService>();
            controller = new ClaimAccountController(userDataService, configDataService)
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
        public void CompleteRegistrationGet_with_no_invalid_email_or_code_redirects_to_AccessDenied(
            string? email,
            string? code
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
        [TestCase(false, false, "")]
        [TestCase(false, false, DefaultPasswordHash)]
        [TestCase(true, false, "")]
        [TestCase(true, false, DefaultPasswordHash)]
        [TestCase(true, true, "")]
        [TestCase(true, true, DefaultPasswordHash)]
        public void CompleteRegistrationGet_with_existing_user_returns_view_model(
            bool userExists,
            bool active,
            string hash
        )
        {
            // Given
            var existingUserAccount = userExists ? UserTestHelper.GetDefaultUserAccount(active: active) : null;
            var userClaimingAccount = UserTestHelper.GetDefaultUserAccount(passwordHash: hash);
            var delegateAccount = UserTestHelper.GetDefaultDelegateAccount(
                candidateNumber: DefaultCandidateNumber,
                centreId: DefaultCentreId
            );
            var expectedModel = GetViewModel(
                userExists: userExists,
                userActive: active,
                supportEmail: DefaultSupportEmail,
                passwordSet: !string.IsNullOrWhiteSpace(hash)
            );

            A.CallTo(
                () => userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                    DefaultEmail,
                    DefaultCode
                )
            ).Returns((DefaultUserId, DefaultCentreId, DefaultCentreName));
            A.CallTo(() => userDataService.GetUserAccountByEmailAddress(DefaultEmail)).Returns(existingUserAccount);
            A.CallTo(() => userDataService.GetUserAccountById(DefaultUserId)).Returns(userClaimingAccount);
            A.CallTo(() => userDataService.GetDelegateAccountsByUserId(DefaultUserId))
                .Returns(new List<DelegateAccount> { delegateAccount });
            A.CallTo(() => configDataService.GetConfigValue(ConfigDataService.SupportEmail))
                .Returns(DefaultSupportEmail);

            // When
            var result = controller.CompleteRegistration(DefaultEmail, DefaultCode);

            // Then
            result.Should().BeViewResult().ModelAs<ClaimAccountViewModel>();
            ((ViewResult)result).ViewData.Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void CompleteRegistrationPost_with_email_in_use_returns_NotFound()
        {
            // Given
            var model = GetViewModel();
            controller.TempData.Set(model);
            A.CallTo(() => userDataService.PrimaryEmailIsInUse(DefaultEmail, A<IDbTransaction?>._)).Returns(true);

            // When
            var result = controller.CompleteRegistration();

            // Then
            A.CallTo(() => userDataService.PrimaryEmailIsInUse(DefaultEmail, A<IDbTransaction?>._))
                .MustHaveHappenedOnceExactly();
            result.Should().BeNotFoundResult();
        }

        // TODO HEEDLS-975 Test that this redirect to SetPassword
        [Test]
        public void CompleteRegistrationPost_with_password_not_set_returns_NotFound()
        {
            // Given
            var model = GetViewModel();
            controller.TempData.Set(model);
            A.CallTo(() => userDataService.PrimaryEmailIsInUse(DefaultEmail, A<IDbTransaction?>._)).Returns(false);

            // When
            var result = controller.CompleteRegistration();

            // Then
            A.CallTo(() => userDataService.PrimaryEmailIsInUse(DefaultEmail, A<IDbTransaction?>._))
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
            A.CallTo(() => userDataService.PrimaryEmailIsInUse(DefaultEmail, A<IDbTransaction?>._)).Returns(false);
            A.CallTo(() => userDataService.SetPrimaryEmailAndActivate(DefaultUserId, DefaultEmail)).DoesNothing();
            A.CallTo(() => userDataService.SetCentreEmail(DefaultUserId, DefaultCentreId, null, A<IDbTransaction?>._))
                .DoesNothing();
            A.CallTo(() => userDataService.SetRegistrationConfirmationHash(DefaultUserId, DefaultCentreId, null))
                .DoesNothing();

            // When
            var result = controller.CompleteRegistration();

            // Then
            A.CallTo(() => userDataService.PrimaryEmailIsInUse(DefaultEmail, A<IDbTransaction?>._))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => userDataService.SetPrimaryEmailAndActivate(DefaultUserId, DefaultEmail))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => userDataService.SetCentreEmail(DefaultUserId, DefaultCentreId, null, A<IDbTransaction?>._))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => userDataService.SetRegistrationConfirmationHash(DefaultUserId, DefaultCentreId, null))
                .MustHaveHappenedOnceExactly();
            result.Should().BeRedirectToActionResult().WithActionName("Confirmation");
        }

        private static ClaimAccountViewModel GetViewModel(
            int userId = DefaultUserId,
            int centreId = DefaultCentreId,
            string centreName = DefaultCentreName,
            string centreSpecificEmail = DefaultEmail,
            string registrationConfirmationHash = DefaultCode,
            string candidateNumber = DefaultCandidateNumber,
            string? supportEmail = null,
            bool userExists = false,
            bool userActive = false,
            bool passwordSet = false
        )
        {
            return new ClaimAccountViewModel
            {
                UserId = userId,
                CentreId = centreId,
                CentreName = centreName,
                CentreSpecificEmail = centreSpecificEmail,
                RegistrationConfirmationHash = registrationConfirmationHash,
                CandidateNumber = candidateNumber,
                SupportEmail = supportEmail,
                UserExists = userExists,
                UserActive = userActive,
                PasswordSet = passwordSet,
            };
        }
    }
}
