﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Web.Controllers.Register;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;
    using Microsoft.FeatureManagement;
    using NUnit.Framework;

    public class RegisterControllerTests
    {
        private const string IpAddress = "1.1.1.1";
        private const int SupervisorDelegateId = 1;
        private ICentresService centresService = null!;
        private RegisterController controller = null!;
        private ICryptoService cryptoService = null!;
        private IFeatureManager featureManager = null!;
        private IJobGroupsService jobGroupsService = null!;
        private PromptsService promptsService = null!;
        private IRegistrationService registrationService = null!;
        private HttpRequest request = null!;
        private ISupervisorDelegateService supervisorDelegateService = null!;
        private IUserService userService = null!;
        private ISupervisorService supervisorService = null!;

        [SetUp]
        public void Setup()
        {
            centresService = A.Fake<ICentresService>();
            cryptoService = A.Fake<ICryptoService>();
            jobGroupsService = A.Fake<IJobGroupsService>();
            registrationService = A.Fake<IRegistrationService>();
            userService = A.Fake<IUserService>();
            promptsService = A.Fake<PromptsService>();
            featureManager = A.Fake<IFeatureManager>();
            supervisorDelegateService = A.Fake<ISupervisorDelegateService>();
            supervisorService = A.Fake<ISupervisorService>();
            request = A.Fake<HttpRequest>();

            controller = new RegisterController(
                    centresService,
                    jobGroupsService,
                    registrationService,
                    cryptoService,
                    promptsService,
                    featureManager,
                    supervisorDelegateService,
                    userService,
                    supervisorService
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
        public void IndexGet_with_no_centreId_shows_index_page()
        {
            // When
            var result = controller.Index();

            // Then
            A.CallTo(() => centresService.GetCentreName(A<int>._)).MustNotHaveHappened();

            using (new AssertionScope())
            {
                result.Should().BeViewResult().ModelAs<RegisterViewModel>().CentreId.Should().BeNull();
                result.Should().BeViewResult().ModelAs<RegisterViewModel>().CentreName.Should().BeNull();
            }
        }

        [Test]
        public void IndexGet_with_centreId_shows_index_page()
        {
            // Given
            const int centreId = 1;
            const string centreName = "centre";
            const string inviteId = "invite";
            A.CallTo(() => centresService.GetCentreName(centreId)).Returns(centreName);

            // When
            var result = controller.Index(centreId, inviteId);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult().ModelAs<RegisterViewModel>().CentreId.Should().Be(centreId);
                result.Should().BeViewResult().ModelAs<RegisterViewModel>().CentreName.Should().Be(centreName);
                result.Should().BeViewResult().ModelAs<RegisterViewModel>().InviteId.Should().Be(inviteId);
            }
        }

        [Test]
        public void IndexGet_with_invalid_centreId_param_shows_error()
        {
            // Given
            const int centreId = 7;
            A.CallTo(() => centresService.GetCentreName(centreId)).Returns(null);

            // When
            var result = controller.Index(centreId);

            // Then
            A.CallTo(() => centresService.GetCentreName(centreId)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void IndexGet_while_logged_in_redirects_to_register_at_new_centre_journey()
        {
            // Given
            const int centreId = 1;
            const string inviteId = "invite";
            var authenticatedController = new RegisterController(
                centresService,
                jobGroupsService,
                registrationService,
                cryptoService,
                promptsService,
                featureManager,
                supervisorDelegateService,
                userService,
                supervisorService
            ).WithDefaultContext().WithMockUser(true);

            // When
            var result = authenticatedController.Index(centreId, inviteId);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("RegisterAtNewCentre")
                .WithActionName("Index").WithRouteValue("centreId", centreId).WithRouteValue("inviteId", inviteId);
        }

        [Test]
        public void Start_with_invalid_centreId_param_shows_error()
        {
            // Given
            const int centreId = 7;
            A.CallTo(() => centresService.GetCentreName(centreId)).Returns(null);

            // When
            var result = controller.Start(centreId);

            // Then
            A.CallTo(() => centresService.GetCentreName(centreId)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Start_with_valid_centreId_param_sets_data_correctly()
        {
            // Given
            const int centreId = 7;
            A.CallTo(() => centresService.GetCentreName(centreId)).Returns("Some centre");

            // When
            var result = controller.Start(centreId);

            // Then
            A.CallTo(() => centresService.GetCentreName(centreId)).MustHaveHappened(1, Times.Exactly);
            var data = controller.TempData.Peek<DelegateRegistrationData>()!;
            data.Centre.Should().Be(centreId);
            data.IsCentreSpecificRegistration.Should().BeTrue();
            result.Should().BeRedirectToActionResult().WithActionName("PersonalInformation");
        }

        [Test]
        public void Start_with_no_centreId_param_allows_normal_registration()
        {
            // When
            var result = controller.Start();

            // Then
            A.CallTo(() => centresService.GetCentreName(A<int>._)).MustNotHaveHappened();
            var data = controller.TempData.Peek<DelegateRegistrationData>()!;
            data.Centre.Should().BeNull();
            data.IsCentreSpecificRegistration.Should().BeFalse();
            result.Should().BeRedirectToActionResult().WithActionName("PersonalInformation");
        }

        [Test]
        public void Start_while_logged_in_redirects_to_register_at_new_centre_journey()
        {
            // Given
            const int centreId = 1;
            const string centreName = "centre";
            const string inviteId = "invite";
            var authenticatedController = new RegisterController(
                centresService,
                jobGroupsService,
                registrationService,
                cryptoService,
                promptsService,
                featureManager,
                supervisorDelegateService,
                userService,
                supervisorService
            ).WithDefaultContext().WithMockUser(true);

            A.CallTo(() => centresService.GetCentreName(centreId)).Returns(centreName);

            // When
            var result = authenticatedController.Start(centreId, inviteId);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("RegisterAtNewCentre")
                .WithActionName("Index").WithRouteValue("centreId", centreId).WithRouteValue("inviteId", inviteId);
        }

        [Test]
        public async Task Summary_post_registers_delegate_with_expected_values()
        {
            // Given
            const string candidateNumber = "TN1";
            var data = RegistrationDataHelper.GetDefaultDelegateRegistrationData();

            var model = new SummaryViewModel
            {
                PrimaryEmail = data.PrimaryEmail,
                CentreSpecificEmail = data.CentreSpecificEmail,
            };

            SetUpFakesForSuccessfulRegistration(candidateNumber, data, 1);

            // When
            var result = await controller.Summary(model);

            // Then
            A.CallTo(
                    () =>
                        registrationService.RegisterDelegateForNewUser(
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
                                    d.CentreAccountIsActive &&
                                    d.IsSelfRegistered &&
                                    d.NotifyDate != null
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
                        registrationService.RegisterDelegateForNewUser(
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
                        registrationService.RegisterDelegateForNewUser(
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
        public async Task Check_registration_complete_returns_redirect_to_confirmation()
        {
            // Given
            const string candidateNumber = "TN1";
            var data = RegistrationDataHelper.GetDefaultDelegateRegistrationData();

            var model = new SummaryViewModel
            {
                PrimaryEmail = data.PrimaryEmail,
                CentreSpecificEmail = data.CentreSpecificEmail,
            };

            SetUpFakesForSuccessfulRegistration(candidateNumber, data, 1);

            // When
            var result = await controller.Summary(model);

            // Then
            A.CallTo(
                    () =>
                        registrationService.RegisterDelegateForNewUser(
                            A<DelegateRegistrationModel>.That.Matches(
                                d =>
                                    d.RegistrationConfirmationHash == null
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
                        registrationService.RegisterDelegateForNewUser(
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
                    () => registrationService.RegisterDelegateForNewUser(
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
                    () => registrationService.RegisterDelegateForNewUser(
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

        private void SetUpFakesForSuccessfulRegistration(
            string candidateNumber,
            DelegateRegistrationData data,
            int userId
        )
        {
            controller.TempData.Set(data);

            A.CallTo(
                    () => registrationService.RegisterDelegateForNewUser(
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

            A.CallTo(
                    () => userService.GetUserIdFromUsername(candidateNumber)
                )
                .Returns(userId);
        }
    }
}
