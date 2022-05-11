namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.Register;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using NUnit.Framework;

    public class RegisterAdminControllerTests
    {
        private ICentresDataService centresDataService = null!;
        private RegisterAdminController controller = null!;
        private ICryptoService cryptoService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IRegistrationService registrationService = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            centresDataService = A.Fake<ICentresDataService>();
            cryptoService = A.Fake<ICryptoService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            registrationService = A.Fake<IRegistrationService>();
            userDataService = A.Fake<IUserDataService>();
            controller = new RegisterAdminController(
                    centresDataService,
                    cryptoService,
                    jobGroupsDataService,
                    registrationService,
                    userDataService
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
            const int centreId = 7;
            A.CallTo(() => centresDataService.GetCentreName(centreId)).Returns(null);

            // When
            var result = controller.Index(centreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(centreId)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void IndexGet_with_centre_autoregistered_true_shows_AccessDenied_error()
        {
            // Given
            const int centreId = 7;
            A.CallTo(() => centresDataService.GetCentreName(centreId)).Returns("My centre");
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).Returns((true, "email@email"));
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(centreId)).Returns(new List<AdminUser>());

            // When
            var result = controller.Index(centreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(centreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(centreId)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void IndexGet_with_centre_autoregisteremail_null_shows_AccessDenied_error()
        {
            // Given
            const int centreId = 7;
            A.CallTo(() => centresDataService.GetCentreName(centreId)).Returns("Some centre");
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).Returns((false, null));
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(centreId)).Returns(new List<AdminUser>());

            // When
            var result = controller.Index(centreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(centreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(centreId)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void IndexGet_with_centre_autoregisteremail_empty_shows_AccessDenied_error()
        {
            // Given
            const int centreId = 7;
            A.CallTo(() => centresDataService.GetCentreName(centreId)).Returns("Some centre");
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).Returns((false, string.Empty));
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(centreId)).Returns(new List<AdminUser>());

            // When
            var result = controller.Index(centreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(centreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(centreId)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void IndexGet_with_centre_with_active_centre_manager_shows_AccessDenied_error()
        {
            // Given
            const int centreId = 7;
            A.CallTo(() => centresDataService.GetCentreName(centreId)).Returns("Some centre");
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).Returns((false, "email@email"));

            var centreManagerAdmin = new AdminUser { CentreId = centreId, IsCentreManager = true };
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(centreId))
                .Returns(new List<AdminUser> { centreManagerAdmin });

            // When
            var result = controller.Index(centreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(centreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(centreId)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void IndexGet_with_allowed_admin_registration_sets_data_correctly()
        {
            // Given
            const int centreId = 7;
            A.CallTo(() => centresDataService.GetCentreName(centreId)).Returns("Some centre");
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).Returns((false, "email@email"));
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(centreId)).Returns(new List<AdminUser>());

            // When
            var result = controller.Index(centreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(centreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(centreId)).MustHaveHappened(1, Times.Exactly);
            var data = controller.TempData.Peek<RegistrationData>()!;
            data.Centre.Should().Be(centreId);
            result.Should().BeRedirectToActionResult().WithActionName("PersonalInformation");
        }

        [Test]
        public void PersonalInformationPost_with_wrong_autoregisteremail_for_centre_fails_validation()
        {
            // Given
            const int centreId = 7;
            var model = new PersonalInformationViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = centreId,
                Email = "wrong@email",
            };
            var data = new RegistrationData(centreId);
            controller.TempData.Set(data);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).Returns((false, "right@email"));

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).MustHaveHappened(1, Times.Exactly);
            controller.ModelState[nameof(PersonalInformationViewModel.Email)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void PersonalInformationPost_with_email_already_in_use_fails_validation()
        {
            // Given
            const int centreId = 7;
            const string email = "right@email";
            var model = new PersonalInformationViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = centreId,
                Email = email,
            };
            var data = new RegistrationData(centreId);
            controller.TempData.Set(data);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).Returns((false, email));
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).Returns(new AdminUser());

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).MustHaveHappened(1, Times.Exactly);
            controller.ModelState[nameof(PersonalInformationViewModel.Email)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void PersonalInformationPost_with_correct_unique_email_is_allowed()
        {
            // Given
            const int centreId = 7;
            const string email = "right@email";
            var model = new PersonalInformationViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = centreId,
                Email = email,
            };
            var data = new RegistrationData(centreId);
            controller.TempData.Set(data);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).Returns((false, email));
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).Returns(null);

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeRedirectToActionResult().WithActionName("LearnerInformation");
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
            const int centreId = 7;
            var model = new SummaryViewModel
            {
                Terms = true,
            };
            var data = new RegistrationData { Centre = centreId, Email = userEmail };
            controller.TempData.Set(data);
            A.CallTo(() => centresDataService.GetCentreName(centreId)).Returns("My centre");
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId))
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
            const int centreId = 7;
            const string email = "right@email";
            var model = new SummaryViewModel
            {
                Terms = true,
            };
            var data = new RegistrationData { Centre = centreId, Email = email };
            controller.TempData.Set(data);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).Returns((false, email));
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).Returns(new AdminUser());

            // When
            var result = controller.Summary(model);

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        [Test]
        public void SummaryPost_with_valid_information_registers_expected_admin()
        {
            // Given
            const int centreId = 7;
            const int jobGroupId = 1;
            const string email = "right@email";
            const string professionalRegistrationNumber = "PRN1234";
            var model = new SummaryViewModel
            {
                Terms = true,
            };
            var data = new RegistrationData
            {
                FirstName = "First",
                LastName = "Name",
                Centre = centreId,
                JobGroup = jobGroupId,
                PasswordHash = "hash",
                Email = email,
                ProfessionalRegistrationNumber = professionalRegistrationNumber,
                HasProfessionalRegistrationNumber = true,
            };
            controller.TempData.Set(data);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).Returns((false, email));
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).Returns(null);
            A.CallTo(() => registrationService.RegisterCentreManager(A<AdminRegistrationModel>._, A<int>._))
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
                                a.Email == data.Email! &&
                                a.Centre == data.Centre!.Value &&
                                a.PasswordHash == data.PasswordHash! &&
                                a.Active &&
                                a.Approved &&
                                a.IsCentreAdmin &&
                                a.IsCentreManager &&
                                !a.IsContentManager &&
                                !a.ImportOnly &&
                                !a.IsContentCreator &&
                                !a.IsTrainer &&
                                !a.IsSupervisor &&
                                a.ProfessionalRegistrationNumber == professionalRegistrationNumber
                        ),
                        jobGroupId
                    )
                )
                .MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Confirmation");
        }
    }
}
