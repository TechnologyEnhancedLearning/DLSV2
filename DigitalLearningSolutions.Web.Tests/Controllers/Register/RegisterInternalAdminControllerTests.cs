namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.Register;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
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
        private IUserDataService userDataService = null!;
        private IRegistrationService registrationService = null!;
        private IDelegateApprovalsService delegateApprovalsService = null!;
        private IFeatureManager featureManager = null!;
        private IRegisterAdminHelper registerAdminHelper = null!;
        private RegisterInternalAdminController controller = null!;
        private HttpRequest request = null!;

        [SetUp]
        public void Setup()
        {
            centresDataService = A.Fake<ICentresDataService>();
            centresService = A.Fake<ICentresService>();
            userDataService = A.Fake<IUserDataService>();
            registrationService = A.Fake<IRegistrationService>();
            delegateApprovalsService = A.Fake<IDelegateApprovalsService>();
            featureManager = A.Fake<IFeatureManager>();
            registerAdminHelper = A.Fake<IRegisterAdminHelper>();
            request = A.Fake<HttpRequest>();
            controller = new RegisterInternalAdminController(
                    centresDataService,
                    centresService,
                    userDataService,
                    registrationService,
                    delegateApprovalsService,
                    featureManager,
                    registerAdminHelper
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
            A.CallTo(() => registerAdminHelper.IsRegisterAdminAllowed(DefaultCentreId)).Returns(false);

            // When
            var result = controller.Index(DefaultCentreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => registerAdminHelper.IsRegisterAdminAllowed(DefaultCentreId)).MustHaveHappenedOnceExactly();
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void IndexGet_with_allowed_admin_registration_returns_view_model()
        {
            // Given
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).Returns("Some centre");
            A.CallTo(() => registerAdminHelper.IsRegisterAdminAllowed(DefaultCentreId)).Returns(true);

            // When
            var result = controller.Index(DefaultCentreId);

            // Then
            A.CallTo(() => registerAdminHelper.IsRegisterAdminAllowed(DefaultCentreId)).MustHaveHappenedOnceExactly();
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

            // When
            var result = await controller.Index(model);

            // Then
            result.Should().BeViewResult().ModelAs<InternalAdminInformationViewModel>();
            controller.ModelState.IsValid.Should().BeFalse();
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
            if (centreSpecificEmail != null)
            {
                A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentre(
                        centreSpecificEmail,
                        DefaultCentreId,
                        A<IDbTransaction?>._
                    )
                ).Returns(false);
                A.CallTo(() => userDataService.GetCentreEmail(DefaultUserId, DefaultCentreId)).Returns(null);
                A.CallTo(() => centresService.DoesEmailMatchCentre(centreSpecificEmail, DefaultCentreId)).Returns(true);
            }
            else
            {
                A.CallTo(() => centresService.DoesEmailMatchCentre(primaryEmail, DefaultCentreId)).Returns(true);
            }

            A.CallTo(
                () => registrationService.CreateCentreManagerForExistingUser(
                    DefaultUserId,
                    DefaultCentreId,
                    centreSpecificEmail
                )
            ).DoesNothing();

            var delegateAccount = UserTestHelper.GetDefaultDelegateAccount(
                id: DefaultDelegateId,
                userId: DefaultUserId,
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

                if (centreSpecificEmail != null)
                {
                    A.CallTo(
                        () => userDataService.SetCentreEmail(
                            DefaultUserId,
                            DefaultCentreId,
                            centreSpecificEmail,
                            A<IDbTransaction?>._
                        )
                    ).MustHaveHappenedOnceExactly();
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
                A.CallTo(() => userDataService.SetCentreEmail(A<int>._, A<int>._, A<string?>._, A<IDbTransaction?>._))
                    .MustNotHaveHappened();
            }

            result.Should().BeRedirectToActionResult().WithActionName("Confirmation");
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
    }
}
