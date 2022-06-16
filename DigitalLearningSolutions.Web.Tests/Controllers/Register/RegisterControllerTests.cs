namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.Register;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;
    using Microsoft.FeatureManagement;
    using NUnit.Framework;

    public class RegisterControllerTests
    {
        private const string IpAddress = "1.1.1.1";
        private const int SupervisorDelegateId = 1;
        private ICentresDataService centresDataService = null!;
        private RegisterController controller = null!;
        private ICryptoService cryptoService = null!;
        private IFeatureManager featureManager = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;

        private PromptsService promptsService = null!;
        private IRegistrationService registrationService = null!;
        private HttpRequest request = null!;
        private ISupervisorDelegateService supervisorDelegateService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            centresDataService = A.Fake<ICentresDataService>();
            cryptoService = A.Fake<ICryptoService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            registrationService = A.Fake<IRegistrationService>();
            userService = A.Fake<IUserService>();
            promptsService = A.Fake<PromptsService>();
            featureManager = A.Fake<IFeatureManager>();
            supervisorDelegateService = A.Fake<ISupervisorDelegateService>();
            request = A.Fake<HttpRequest>();

            controller = new RegisterController(
                    centresDataService,
                    jobGroupsDataService,
                    registrationService,
                    cryptoService,
                    userService,
                    promptsService,
                    featureManager,
                    supervisorDelegateService
                )
                .WithDefaultContext()
                .WithMockRequestContext(request)
                .WithMockServices()
                .WithMockTempData();
        }

        [Test]
        public void PersonalInformationPost_does_not_continue_to_next_page_with_invalid_model()
        {
            // Given
            controller.TempData.Set(new DelegateRegistrationData());
            var model = new PersonalInformationViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = 7,
                PrimaryEmail = "primary@email",
                CentreSpecificEmail = "centre@email",
            };
            controller.ModelState.AddModelError(nameof(PersonalInformationViewModel.PrimaryEmail), "error message");

            // When
            var result = controller.PersonalInformation(model);

            // Then
            result.Should().BeViewResult().ModelAs<PersonalInformationViewModel>();
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Test]
        public void IndexGet_with_invalid_centreId_param_shows_error()
        {
            // Given
            const int centreId = 7;
            A.CallTo(() => centresDataService.GetCentreName(centreId)).Returns(null);

            // When
            var result = controller.Index(centreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(centreId)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void IndexGet_with_valid_centreId_param_sets_data_correctly()
        {
            // Given
            const int centreId = 7;
            A.CallTo(() => centresDataService.GetCentreName(centreId)).Returns("Some centre");

            // When
            var result = controller.Index(centreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(centreId)).MustHaveHappened(1, Times.Exactly);
            var data = controller.TempData.Peek<DelegateRegistrationData>()!;
            data.Centre.Should().Be(centreId);
            data.IsCentreSpecificRegistration.Should().BeTrue();
            result.Should().BeRedirectToActionResult().WithActionName("PersonalInformation");
        }

        [Test]
        public void IndexGet_with_no_centreId_param_allows_normal_registration()
        {
            // When
            var result = controller.Index();

            // Then
            A.CallTo(() => centresDataService.GetCentreName(A<int>._)).MustNotHaveHappened();
            var data = controller.TempData.Peek<DelegateRegistrationData>()!;
            data.Centre.Should().BeNull();
            data.IsCentreSpecificRegistration.Should().BeFalse();
            result.Should().BeRedirectToActionResult().WithActionName("PersonalInformation");
        }

        [Test]
        public void IndexGet_while_logged_in_redirects_to_register_at_new_centre_journey()
        {
            // Given
            var authenticatedController = new RegisterController(
                centresDataService,
                jobGroupsDataService,
                registrationService,
                cryptoService,
                userService,
                promptsService,
                featureManager,
                supervisorDelegateService
            ).WithDefaultContext().WithMockUser(true);

            // When
            var result = authenticatedController.Index();

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("RegisterAtNewCentre")
                .WithActionName("Index");
        }

        [Test]
        public async Task Summary_post_registers_delegate_with_expected_values()
        {
            // Given
            const string candidateNumber = "TN1";
            var data = RegistrationDataHelper.GetDefaultDelegateRegistrationData();
            controller.TempData.Set(data);
            A.CallTo(
                    () => registrationService.CreateDelegateAccountForNewUser(
                        A<DelegateRegistrationModel>._,
                        A<string>._,
                        A<bool>._,
                        A<bool>._,
                        A<int>._
                    )
                )
                .Returns((candidateNumber, true));
            A.CallTo(() => request.Headers).Returns(
                new HeaderDictionary(
                    new Dictionary<string, StringValues> { { "X-Forwarded-For", new StringValues(IpAddress) } }
                )
            );

            // When
            var result = await controller.Summary(new SummaryViewModel());

            // Then
            A.CallTo(
                    () =>
                        registrationService.CreateDelegateAccountForNewUser(
                            A<DelegateRegistrationModel>.That.Matches(
                                d =>
                                    d.FirstName == data.FirstName &&
                                    d.LastName == data.LastName &&
                                    d.PrimaryEmail == data.PrimaryEmail &&
                                    d.CentreSpecificEmail == data.CentreSpecificEmail &&
                                    d.Centre == data.Centre &&
                                    d.JobGroup == data.JobGroup &&
                                    d.PasswordHash == data.PasswordHash &&
                                    d.Answer1 == data.Answer1 &&
                                    d.Answer2 == data.Answer2 &&
                                    d.Answer3 == data.Answer3 &&
                                    d.Answer4 == data.Answer4 &&
                                    d.Answer5 == data.Answer5 &&
                                    d.Answer6 == data.Answer6 &&
                                    d.Active &&
                                    d.IsSelfRegistered &&
                                    d.NotifyDate != null &&
                                    d.AliasId == null
                            ),
                            IpAddress,
                            false,
                            true,
                            SupervisorDelegateId
                        )
                )
                .MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Confirmation");
        }

        [Test]
        public async Task Summary_post_returns_redirect_to_index_view_with_missing_centre()
        {
            // Given
            var data = RegistrationDataHelper.GetDefaultDelegateRegistrationData(centre: null);
            controller.TempData.Set(data);
            controller.ModelState.AddModelError("", "");

            // When
            var result = await controller.Summary(new SummaryViewModel());

            // Then
            A.CallTo(
                    () =>
                        registrationService.CreateDelegateAccountForNewUser(
                            A<DelegateRegistrationModel>._,
                            IpAddress,
                            false,
                            true,
                            SupervisorDelegateId
                        )
                )
                .MustNotHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public async Task Summary_post_returns_redirect_to_index_view_with_missing_job_group()
        {
            // Given
            var data = RegistrationDataHelper.GetDefaultDelegateRegistrationData(jobGroup: null);
            controller.TempData.Set(data);
            controller.ModelState.AddModelError("", "");

            // When
            var result = await controller.Summary(new SummaryViewModel());

            // Then
            A.CallTo(
                    () =>
                        registrationService.CreateDelegateAccountForNewUser(
                            A<DelegateRegistrationModel>._,
                            IpAddress,
                            false,
                            true,
                            SupervisorDelegateId
                        )
                )
                .MustNotHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public async Task Summary_post_returns_default_view_with_invalid_model()
        {
            // Given
            var data = RegistrationDataHelper.GetDefaultDelegateRegistrationData();
            controller.TempData.Set(data);
            controller.ModelState.AddModelError("", "");

            // When
            var result = await controller.Summary(new SummaryViewModel());

            // Then
            A.CallTo(
                    () =>
                        registrationService.CreateDelegateAccountForNewUser(
                            A<DelegateRegistrationModel>._,
                            IpAddress,
                            false,
                            true,
                            SupervisorDelegateId
                        )
                )
                .MustNotHaveHappened();
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public async Task Summary_post_returns_500_error_with_unexpected_register_error()
        {
            // Given
            var data = RegistrationDataHelper.GetDefaultDelegateRegistrationData();
            controller.TempData.Set(data);
            A.CallTo(
                    () => registrationService.CreateDelegateAccountForNewUser(
                        A<DelegateRegistrationModel>._,
                        A<string>._,
                        A<bool>._,
                        A<bool>._,
                        A<int>._
                    )
                )
                .Throws(new DelegateCreationFailedException(DelegateCreationError.UnexpectedError));
            A.CallTo(() => request.Headers).Returns(
                new HeaderDictionary(
                    new Dictionary<string, StringValues> { { "X-Forwarded-For", new StringValues(IpAddress) } }
                )
            );

            // When
            var result = await controller.Summary(new SummaryViewModel());

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        [Test]
        public async Task Summary_post_returns_redirect_to_index_with_email_in_use_register_error()
        {
            // Given
            var data = RegistrationDataHelper.GetDefaultDelegateRegistrationData();
            controller.TempData.Set(data);
            A.CallTo(
                    () => registrationService.CreateDelegateAccountForNewUser(
                        A<DelegateRegistrationModel>._,
                        A<string>._,
                        A<bool>._,
                        A<bool>._,
                        A<int>._
                    )
                )
                .Throws(new DelegateCreationFailedException(DelegateCreationError.EmailAlreadyInUse));
            A.CallTo(() => request.Headers).Returns(
                new HeaderDictionary(
                    new Dictionary<string, StringValues> { { "X-Forwarded-For", new StringValues(IpAddress) } }
                )
            );

            // When
            var result = await controller.Summary(new SummaryViewModel());

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }
    }
}
