namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Controllers.Register;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class RegisterAdminControllerTests
    {
        private const int DefaultCentreId = 7;
        private const string DefaultPrimaryEmail = "primary@email.com";
        private const string DefaultCentreSpecificEmail = "centre@email.com";
        private ICentresDataService centresDataService = null!;
        private ICentresService centresService = null!;
        private RegisterAdminController controller = null!;
        private ICryptoService cryptoService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IRegistrationService registrationService = null!;
        private IUserDataService userDataService = null!;
        private IRegisterAdminService registerAdminService = null!;

        [SetUp]
        public void Setup()
        {
            centresDataService = A.Fake<ICentresDataService>();
            centresService = A.Fake<ICentresService>();
            cryptoService = A.Fake<ICryptoService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            registrationService = A.Fake<IRegistrationService>();
            userDataService = A.Fake<IUserDataService>();
            registerAdminService = A.Fake<IRegisterAdminService>();
            controller = new RegisterAdminController(
                    centresDataService,
                    centresService,
                    cryptoService,
                    jobGroupsDataService,
                    registrationService,
                    userDataService,
                    registerAdminService
                )
                .WithDefaultContext()
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
            A.CallTo(() => registerAdminService.IsRegisterAdminAllowed(DefaultCentreId)).Returns(false);

            // When
            var result = controller.Index(DefaultCentreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => registerAdminService.IsRegisterAdminAllowed(DefaultCentreId)).MustHaveHappenedOnceExactly();
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void IndexGet_with_allowed_admin_registration_sets_data_correctly()
        {
            // Given
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).Returns("Some centre");
            A.CallTo(() => registerAdminService.IsRegisterAdminAllowed(DefaultCentreId)).Returns(true);

            // When
            var result = controller.Index(DefaultCentreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => registerAdminService.IsRegisterAdminAllowed(DefaultCentreId)).MustHaveHappenedOnceExactly();
            var data = controller.TempData.Peek<RegistrationData>()!;
            data.Centre.Should().Be(DefaultCentreId);
            result.Should().BeRedirectToActionResult().WithActionName("PersonalInformation");
        }

        [Test]
        public void PersonalInformationPost_does_not_continue_to_next_page_with_invalid_model()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModelAndSetTempData();
            controller.ModelState.AddModelError(nameof(PersonalInformationViewModel.PrimaryEmail), "error message");

            // When
            var result = controller.PersonalInformation(model);

            // Then
            result.Should().BeViewResult().ModelAs<PersonalInformationViewModel>();
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [TestCase(true, "correct@email", "correct@email")]
        [TestCase(false, null, "correct@email")]
        [TestCase(false, "", null)]
        [TestCase(false, null, null)]
        [TestCase(false, "correct@email", "wrong@email")]
        public void SummaryPost_with_admin_registration_not_allowed_throws_error(
            bool autoRegistered,
            string autoRegisterManagerEmail,
            string userEmail
        )
        {
            // Given
            var model = new SummaryViewModel
            {
                Terms = true,
            };
            var data = new RegistrationData { Centre = DefaultCentreId, PrimaryEmail = userEmail };
            controller.TempData.Set(data);
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).Returns("My centre");
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((autoRegistered, autoRegisterManagerEmail));

            // When
            var result = controller.Summary(model);

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        [Test]
        public void SummaryPost_with_email_already_in_use_fails_validation()
        {
            // Given
            var model = new SummaryViewModel
            {
                Terms = true,
            };
            var data = new RegistrationData { Centre = DefaultCentreId, PrimaryEmail = DefaultPrimaryEmail };
            controller.TempData.Set(data);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((false, DefaultPrimaryEmail));
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(DefaultPrimaryEmail)).Returns(new AdminUser());

            // When
            var result = controller.Summary(model);

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        [Test]
        [TestCase("primary@email", null)]
        [TestCase("primary@email", "centre@email")]
        public void SummaryPost_with_valid_information_registers_expected_admin(
            string primaryEmail,
            string? centreSpecificEmail
        )
        {
            // Given
            const int jobGroupId = 1;
            var centreEmailOrPrimaryIfNull = centreSpecificEmail ?? primaryEmail;
            const string professionalRegistrationNumber = "PRN1234";
            var model = new SummaryViewModel
            {
                Terms = true,
            };
            var data = new RegistrationData
            {
                FirstName = "First",
                LastName = "Name",
                Centre = DefaultCentreId,
                JobGroup = jobGroupId,
                PasswordHash = "hash",
                PrimaryEmail = primaryEmail,
                CentreSpecificEmail = centreSpecificEmail,
                ProfessionalRegistrationNumber = professionalRegistrationNumber,
                HasProfessionalRegistrationNumber = true,
            };
            controller.TempData.Set(data);
            A.CallTo(() => registerAdminService.IsRegisterAdminAllowed(DefaultCentreId)).Returns(true);
            if (centreSpecificEmail != null)
            {
                A.CallTo(() => userDataService.GetAdminUserByEmailAddress(centreSpecificEmail)).Returns(null);
            }

            A.CallTo(() => centresService.DoEmailsMatchCentre(primaryEmail, centreSpecificEmail, DefaultCentreId))
                .Returns(true);
            A.CallTo(
                    () => registrationService.RegisterCentreManager(
                        A<AdminRegistrationModel>._,
                        false
                    )
                )
                .DoesNothing();

            // When
            var result = controller.Summary(model);

            // Then
            A.CallTo(
                    () => registrationService.RegisterCentreManager(
                        A<AdminRegistrationModel>.That.Matches(
                            a =>
                                a.FirstName == data.FirstName &&
                                a.LastName == data.LastName &&
                                a.PrimaryEmail == data.PrimaryEmail! &&
                                a.CentreSpecificEmail == data.CentreSpecificEmail &&
                                a.Centre == data.Centre.Value &&
                                a.PasswordHash == data.PasswordHash! &&
                                a.CentreAccountIsActive &&
                                a.Approved &&
                                a.IsCentreAdmin &&
                                a.IsCentreManager &&
                                !a.IsContentManager &&
                                !a.ImportOnly &&
                                !a.IsContentCreator &&
                                !a.IsTrainer &&
                                !a.IsSupervisor &&
                                a.ProfessionalRegistrationNumber == professionalRegistrationNumber &&
                                a.JobGroup == jobGroupId
                        ),
                        true
                    )
                )
                .MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Confirmation");
        }

        private PersonalInformationViewModel GetDefaultPersonalInformationViewModelAndSetTempData(
            string? centreSpecificEmail = DefaultCentreSpecificEmail
        )
        {
            var model = new PersonalInformationViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = DefaultCentreId,
                PrimaryEmail = DefaultPrimaryEmail,
                CentreSpecificEmail = centreSpecificEmail,
            };

            var data = new RegistrationData(DefaultCentreId);
            controller.TempData.Set(data);

            return model;
        }
    }
}
