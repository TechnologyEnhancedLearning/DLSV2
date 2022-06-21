namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
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

    public class RegisterAtNewCentreControllerTests
    {
        private const string IpAddress = "1.1.1.1";
        private const int SupervisorDelegateId = 1;
        private ICentresDataService centresDataService = null!;
        private RegisterAtNewCentreController controller = null!;
        private IFeatureManager featureManager = null!;

        private PromptsService promptsService = null!;
        private IRegistrationService registrationService = null!;
        private HttpRequest request = null!;
        private ISupervisorDelegateService supervisorDelegateService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            centresDataService = A.Fake<ICentresDataService>();
            registrationService = A.Fake<IRegistrationService>();
            userService = A.Fake<IUserService>();
            promptsService = A.Fake<PromptsService>();
            featureManager = A.Fake<IFeatureManager>();
            supervisorDelegateService = A.Fake<ISupervisorDelegateService>();
            request = A.Fake<HttpRequest>();

            controller = new RegisterAtNewCentreController(
                    centresDataService,
                    featureManager,
                    promptsService,
                    registrationService,
                    supervisorDelegateService,
                    userService
                )
                .WithDefaultContext()
                .WithMockRequestContext(request)
                .WithMockServices()
                .WithMockTempData()
                .WithMockUser(true);
        }

        [Test]
        public void PersonalInformationPost_with_invalid_emails_fails_validation()
        {
            // Given
            const int centreId = 3;
            controller.TempData.Set(new InternalDelegateRegistrationData());
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            var model = new InternalPersonalInformationViewModel
            {
                Centre = centreId,
                CentreSpecificEmail = "centre email",
            };
            A.CallTo(() => userService.EmailIsInUse(model.CentreSpecificEmail!))
                .Returns(true);
            A.CallTo(() => userService.GetUserById(userAccount.Id)).Returns(
                new UserEntity(userAccount, new List<AdminAccount>(), new[] { new DelegateAccount() })
            );

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => userService.EmailIsInUse(model.CentreSpecificEmail!))
                .MustHaveHappened();
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void PersonalInformationPost_with_valid_emails_is_allowed()
        {
            // Given
            controller.TempData.Set(new InternalDelegateRegistrationData());
            var model = new InternalPersonalInformationViewModel
            {
                Centre = ControllerContextHelper.CentreId + 1,
                CentreSpecificEmail = "centre email",
            };
            A.CallTo(() => userService.EmailIsInUse(model.CentreSpecificEmail!))
                .Returns(false);

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => userService.EmailIsInUse(model.CentreSpecificEmail!))
                .MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("LearnerInformation");
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
            var data = controller.TempData.Peek<InternalDelegateRegistrationData>()!;
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
            var data = controller.TempData.Peek<InternalDelegateRegistrationData>()!;
            data.Centre.Should().BeNull();
            data.IsCentreSpecificRegistration.Should().BeFalse();
            result.Should().BeRedirectToActionResult().WithActionName("PersonalInformation");
        }

        [Test]
        public async Task Summary_post_registers_delegate_with_expected_values()
        {
            // Given
            const string candidateNumber = "TN1";
            var data = RegistrationDataHelper.GetDefaultInternalDelegateRegistrationData();
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
            var result = await controller.SummaryPost();

            // Then
            A.CallTo(
                    () =>
                        registrationService.CreateDelegateAccountForExistingUser(
                            A<InternalDelegateRegistrationModel>.That.Matches(
                                d =>
                                    d.Centre == data.Centre &&
                                    d.Answer1 == data.Answer1 &&
                                    d.Answer2 == data.Answer2 &&
                                    d.Answer3 == data.Answer3 &&
                                    d.Answer4 == data.Answer4 &&
                                    d.Answer5 == data.Answer5 &&
                                    d.Answer6 == data.Answer6
                            ),
                            ControllerContextHelper.UserId,
                            IpAddress,
                            false,
                            null
                        )
                )
                .MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Confirmation");
        }

        [Test]
        public async Task Summary_post_returns_redirect_to_index_view_with_missing_centre()
        {
            // Given
            var data = RegistrationDataHelper.GetDefaultInternalDelegateRegistrationData(centre: null);
            controller.TempData.Set(data);

            // When
            var result = await controller.SummaryPost();

            // Then
            A.CallTo(
                    () =>
                        registrationService.CreateDelegateAccountForNewUser(
                            A<DelegateRegistrationModel>._,
                            IpAddress,
                            false,
                            false,
                            SupervisorDelegateId
                        )
                )
                .MustNotHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public async Task Summary_post_returns_500_error_with_unexpected_register_error()
        {
            // Given
            var data = RegistrationDataHelper.GetDefaultInternalDelegateRegistrationData();
            controller.TempData.Set(data);
            A.CallTo(
                    () => registrationService.CreateDelegateAccountForExistingUser(
                        A<InternalDelegateRegistrationModel>._,
                        A<int>._,
                        A<string>._,
                        A<bool>._,
                        A<int?>._
                    )
                )
                .Throws(new DelegateCreationFailedException(DelegateCreationError.UnexpectedError));
            A.CallTo(() => request.Headers).Returns(
                new HeaderDictionary(
                    new Dictionary<string, StringValues> { { "X-Forwarded-For", new StringValues(IpAddress) } }
                )
            );

            // When
            var result = await controller.SummaryPost();

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        [Test]
        public async Task Summary_post_returns_redirect_to_index_with_email_in_use_register_error()
        {
            // Given
            var data = RegistrationDataHelper.GetDefaultInternalDelegateRegistrationData();
            controller.TempData.Set(data);
            A.CallTo(
                    () => registrationService.CreateDelegateAccountForExistingUser(
                        A<InternalDelegateRegistrationModel>._,
                        A<int>._,
                        A<string>._,
                        A<bool>._,
                        A<int?>._
                    )
                )
                .Throws(new DelegateCreationFailedException(DelegateCreationError.EmailAlreadyInUse));
            A.CallTo(() => request.Headers).Returns(
                new HeaderDictionary(
                    new Dictionary<string, StringValues> { { "X-Forwarded-For", new StringValues(IpAddress) } }
                )
            );

            // When
            var result = await controller.SummaryPost();

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }
    }
}
