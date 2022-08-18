namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.Register;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Primitives;
    using Microsoft.FeatureManagement;
    using NUnit.Framework;

    public class RegisterInternalAdminControllerTests
    {
        private const int DefaultCentreId = 7;
        private const string DefaultCentreName = "Centre";
        private const string DefaultPrimaryEmail = "primary@email.com";
        private const string DefaultCentreSpecificEmail = "centre@email.com";
        private const int DefaultUserId = 2;
        private const int DefaultDelegateId = 5;
        private ICentresDataService centresDataService = null!;
        private ICentresService centresService = null!;
        private IConfiguration config = null!;
        private RegisterInternalAdminController controller = null!;
        private IDelegateApprovalsService delegateApprovalsService = null!;
        private IEmailVerificationService emailVerificationService = null!;
        private IFeatureManager featureManager = null!;
        private IRegisterAdminService registerAdminService = null!;
        private IRegistrationService registrationService = null!;
        private HttpRequest request = null!;
        private IUserDataService userDataService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            centresDataService = A.Fake<ICentresDataService>();
            centresService = A.Fake<ICentresService>();
            userDataService = A.Fake<IUserDataService>();
            userService = A.Fake<IUserService>();
            registrationService = A.Fake<IRegistrationService>();
            delegateApprovalsService = A.Fake<IDelegateApprovalsService>();
            featureManager = A.Fake<IFeatureManager>();
            registerAdminService = A.Fake<IRegisterAdminService>();
            emailVerificationService = A.Fake<IEmailVerificationService>();
            config = A.Fake<IConfiguration>();
            request = A.Fake<HttpRequest>();
            controller = new RegisterInternalAdminController(
                    centresDataService,
                    centresService,
                    userDataService,
                    userService,
                    registrationService,
                    delegateApprovalsService,
                    featureManager,
                    registerAdminService,
                    emailVerificationService,
                    config
                )
                .WithDefaultContext()
                .WithMockRequestContext(request)
                .WithMockUser(true, userId: DefaultUserId)
                .WithMockServices()
                .WithMockTempData();
        }

        [Test]
        public void IndexGet_with_no_centreId_param_shows_notfound_error()
        {
            // When
            var result = controller.Index();

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void IndexGet_with_invalid_centreId_param_shows_notfound_error()
        {
            // Given
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).Returns(null);

            // When
            var result = controller.Index(DefaultCentreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).MustHaveHappenedOnceExactly();

            result.Should().BeNotFoundResult();
        }

        [Test]
        public void IndexGet_with_not_allowed_admin_registration_returns_access_denied()
        {
            // Given
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).Returns("Some centre");
            A.CallTo(() => registerAdminService.IsRegisterAdminAllowed(DefaultCentreId, DefaultUserId)).Returns(false);

            // When
            var result = controller.Index(DefaultCentreId);

            // Then
            A.CallTo(() => registerAdminService.IsRegisterAdminAllowed(DefaultCentreId, DefaultUserId))
                .MustHaveHappenedOnceExactly();

            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void IndexGet_with_allowed_admin_registration_returns_view_model()
        {
            // Given
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).Returns("Some centre");
            A.CallTo(() => registerAdminService.IsRegisterAdminAllowed(DefaultCentreId, DefaultUserId)).Returns(true);

            // When
            var result = controller.Index(DefaultCentreId);

            // Then
            result.Should().BeViewResult().ModelAs<InternalAdminInformationViewModel>();
        }

        [Test]
        public async Task IndexPost_does_not_continue_to_next_page_with_invalid_model()
        {
            // Given
            var model = GetDefaultInternalAdminInformationViewModel();

            controller.ModelState.AddModelError(
                nameof(InternalAdminInformationViewModel.CentreSpecificEmail),
                "error message"
            );

            A.CallTo(() => registerAdminService.IsRegisterAdminAllowed(DefaultCentreId, DefaultUserId)).Returns(true);

            // When
            var result = await controller.Index(model);

            // Then
            result.Should().BeViewResult().ModelAs<InternalAdminInformationViewModel>();
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Test]
        public async Task IndexPost_with_not_allowed_admin_registration_returns_access_denied()
        {
            // Given
            var model = GetDefaultInternalAdminInformationViewModel();

            A.CallTo(() => registerAdminService.IsRegisterAdminAllowed(DefaultCentreId, DefaultUserId)).Returns(false);

            // When
            var result = await controller.Index(model);

            // Then
            A.CallTo(() => registerAdminService.IsRegisterAdminAllowed(DefaultCentreId, DefaultUserId))
                .MustHaveHappenedOnceExactly();

            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        [TestCase(DefaultPrimaryEmail, null, false, false)]
        [TestCase(DefaultPrimaryEmail, DefaultCentreSpecificEmail, true, false)]
        [TestCase(DefaultPrimaryEmail, DefaultCentreSpecificEmail, true, true)]
        public async Task IndexPost_with_valid_information_registers_expected_admin_and_delegate(
            string primaryEmail,
            string? centreSpecificEmail,
            bool hasDelegateAccount,
            bool isDelegateApproved
        )
        {
            // Given
            var model = GetDefaultInternalAdminInformationViewModel(centreSpecificEmail);

            SetUpFakesForSuccessfulRegistration(
                primaryEmail,
                centreSpecificEmail,
                hasDelegateAccount,
                isDelegateApproved
            );

            // When
            var result = await controller.Index(model);

            // Then
            A.CallTo(
                () => registrationService.CreateCentreManagerForExistingUser(
                    DefaultUserId,
                    DefaultCentreId,
                    centreSpecificEmail
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(() => userDataService.GetDelegateAccountsByUserId(DefaultUserId)).MustHaveHappenedOnceExactly();

            if (hasDelegateAccount)
            {
                A.CallTo(
                    () => registrationService.CreateDelegateAccountForExistingUser(
                        A<InternalDelegateRegistrationModel>._,
                        A<int>._,
                        A<string>._,
                        A<bool>._,
                        A<int?>._
                    )
                ).MustNotHaveHappened();

                if (isDelegateApproved)
                {
                    A.CallTo(() => delegateApprovalsService.ApproveDelegate(A<int>._, A<int>._)).MustNotHaveHappened();
                }
                else
                {
                    A.CallTo(() => delegateApprovalsService.ApproveDelegate(A<int>._, A<int>._))
                        .MustHaveHappenedOnceExactly();
                }
            }
            else
            {
                A.CallTo(
                    () => registrationService.CreateDelegateAccountForExistingUser(
                        A<InternalDelegateRegistrationModel>._,
                        A<int>._,
                        A<string>._,
                        A<bool>._,
                        A<int?>._
                    )
                ).MustHaveHappenedOnceExactly();
                A.CallTo(() => delegateApprovalsService.ApproveDelegate(A<int>._, A<int>._)).MustNotHaveHappened();
                A.CallTo(
                        () => userDataService.SetCentreEmail(
                            A<int>._,
                            A<int>._,
                            A<string?>._,
                            A<DateTime?>._,
                            A<IDbTransaction?>._
                        )
                    )
                    .MustNotHaveHappened();
            }

            result.Should().BeRedirectToActionResult().WithActionName("Confirmation");
        }

        [Test]
        public async Task
            IndexPost_with_valid_information_sends_verification_email_to_centre_specific_email_if_unverified()
        {
            // Given
            const bool emailIsVerifiedForUser = false;
            var model = GetDefaultInternalAdminInformationViewModel();

            SetUpFakesForSuccessfulRegistration(DefaultPrimaryEmail, DefaultCentreSpecificEmail, false, true);

            A.CallTo(() => emailVerificationService.AccountEmailIsVerifiedForUser(A<int>._, A<string>._))
                .Returns(emailIsVerifiedForUser);
            A.CallTo(() => userService.GetUserByEmailAddress(DefaultPrimaryEmail))
                .Returns(UserTestHelper.GetDefaultUserAccount());
            A.CallTo(
                () => emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                    A<UserAccount>._,
                    A<List<string>>._,
                    A<string>._
                )
            ).DoesNothing();

            // When
            await controller.Index(model);

            // Then
            A.CallTo(() => emailVerificationService.AccountEmailIsVerifiedForUser(A<int>._, A<string>._))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.GetUserByEmailAddress(DefaultPrimaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                    A<UserAccount>._,
                    A<List<string>>.That.Matches(
                        list => ListTestHelper.ListOfStringsMatch(
                            list,
                            new List<string> { DefaultCentreSpecificEmail }
                        )
                    ),
                    A<string>._
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task
            IndexPost_with_valid_information_does_not_send_verification_email_if_centre_specific_email_is_already_verified_for_user()
        {
            // Given
            const bool emailIsVerifiedForUser = true;

            var model = GetDefaultInternalAdminInformationViewModel();

            SetUpFakesForSuccessfulRegistration(DefaultPrimaryEmail, DefaultCentreSpecificEmail, false, true);

            A.CallTo(() => emailVerificationService.AccountEmailIsVerifiedForUser(A<int>._, A<string>._))
                .Returns(emailIsVerifiedForUser);

            // When
            await controller.Index(model);

            // Then
            A.CallTo(() => emailVerificationService.AccountEmailIsVerifiedForUser(A<int>._, A<string>._))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.GetUserById(A<int>._)).MustNotHaveHappened();
            A.CallTo(
                () => emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                    A<UserAccount>._,
                    A<List<string>>._,
                    A<string>._
                )
            ).MustNotHaveHappened();
        }

        private InternalAdminInformationViewModel GetDefaultInternalAdminInformationViewModel(
            string? centreSpecificEmail = DefaultCentreSpecificEmail
        )
        {
            return new InternalAdminInformationViewModel
            {
                Centre = DefaultCentreId,
                CentreName = DefaultCentreName,
                PrimaryEmail = DefaultPrimaryEmail,
                CentreSpecificEmail = centreSpecificEmail,
            };
        }

        private void SetUpFakesForSuccessfulRegistration(
            string primaryEmail,
            string? centreSpecificEmail,
            bool hasDelegateAccount,
            bool isDelegateApproved
        )
        {
            if (centreSpecificEmail != null)
            {
                A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentre(centreSpecificEmail, DefaultCentreId)
                ).Returns(false);
                A.CallTo(() => userDataService.GetCentreEmail(DefaultUserId, DefaultCentreId)).Returns(null);
            }

            A.CallTo(() => registerAdminService.IsRegisterAdminAllowed(DefaultCentreId, DefaultUserId)).Returns(true);

            A.CallTo(
                    () => centresService.IsAnEmailValidForCentreManager(
                        primaryEmail,
                        centreSpecificEmail,
                        DefaultCentreId
                    )
                )
                .Returns(true);

            A.CallTo(
                () => registrationService.CreateCentreManagerForExistingUser(
                    DefaultUserId,
                    DefaultCentreId,
                    centreSpecificEmail
                )
            ).DoesNothing();

            var delegateAccount = UserTestHelper.GetDefaultDelegateAccount(
                DefaultDelegateId,
                DefaultUserId,
                centreId: DefaultCentreId,
                approved: isDelegateApproved
            );
            A.CallTo(() => userDataService.GetDelegateAccountsByUserId(DefaultUserId)).Returns(
                hasDelegateAccount ? new List<DelegateAccount> { delegateAccount } : new List<DelegateAccount>()
            );

            A.CallTo(
                () => registrationService.CreateDelegateAccountForExistingUser(
                    A<InternalDelegateRegistrationModel>._,
                    A<int>._,
                    A<string>._,
                    A<bool>._,
                    A<int?>._
                )
            ).Returns(("candidate", true, true));

            A.CallTo(
                    () => userDataService.SetCentreEmail(
                        DefaultUserId,
                        DefaultCentreId,
                        centreSpecificEmail,
                        A<DateTime?>._,
                        A<IDbTransaction?>._
                    )
                )
                .DoesNothing();
            A.CallTo(() => delegateApprovalsService.ApproveDelegate(DefaultDelegateId, DefaultCentreId)).DoesNothing();

            A.CallTo(() => request.Headers).Returns(
                new HeaderDictionary(
                    new Dictionary<string, StringValues> { { "X-Forwarded-For", new StringValues("1.1.1.1") } }
                )
            );
            A.CallTo(() => featureManager.IsEnabledAsync(A<string>._)).Returns(false);
        }
    }
}
