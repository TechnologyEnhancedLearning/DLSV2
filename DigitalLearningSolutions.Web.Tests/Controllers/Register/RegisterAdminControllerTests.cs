namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.Register;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class RegisterAdminControllerTests
    {
        private const int DefaultCentreId = 7;
        private const int DefaultJobGroupId = 7;
        private const string DefaultPRN = "PRN1234";
        private const string DefaultPrimaryEmail = "primary@email.com";
        private const string DefaultCentreSpecificEmail = "centre@email.com";
        private ICentresDataService centresDataService = null!;
        private ICentresService centresService = null!;
        private IConfiguration config = null!;
        private RegisterAdminController controller = null!;
        private ICryptoService cryptoService = null!;
        private IEmailVerificationService emailVerificationService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IRegisterAdminService registerAdminService = null!;
        private IRegistrationService registrationService = null!;
        private IUserDataService userDataService = null!;
        private IUserService userService = null!;

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
            emailVerificationService = A.Fake<IEmailVerificationService>();
            userService = A.Fake<IUserService>();
            config = A.Fake<IConfiguration>();
            controller = new RegisterAdminController(
                    centresDataService,
                    centresService,
                    cryptoService,
                    jobGroupsDataService,
                    registrationService,
                    userDataService,
                    registerAdminService,
                    emailVerificationService,
                    userService,
                    config
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
            A.CallTo(() => registerAdminService.IsRegisterAdminAllowed(DefaultCentreId, null)).Returns(false);

            // When
            var result = controller.Index(DefaultCentreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => registerAdminService.IsRegisterAdminAllowed(DefaultCentreId, null))
                .MustHaveHappenedOnceExactly();
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void IndexGet_with_allowed_admin_registration_sets_data_correctly()
        {
            // Given
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).Returns("Some centre");
            A.CallTo(() => registerAdminService.IsRegisterAdminAllowed(DefaultCentreId, null)).Returns(true);

            // When
            var result = controller.Index(DefaultCentreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => registerAdminService.IsRegisterAdminAllowed(DefaultCentreId, null))
                .MustHaveHappenedOnceExactly();
            var data = controller.TempData.Peek<RegistrationData>()!;
            data.Centre.Should().Be(DefaultCentreId);
            result.Should().BeRedirectToActionResult().WithActionName("PersonalInformation");
        }

        [Test]
        public void IndexGet_with_logged_in_user_redirects_to_RegisterInternalAdmin()
        {
            // Given
            var controllerWithLoggedInUser = new RegisterAdminController(
                    centresDataService,
                    centresService,
                    cryptoService,
                    jobGroupsDataService,
                    registrationService,
                    userDataService,
                    registerAdminService,
                    emailVerificationService,
                    userService,
                    config
                )
                .WithDefaultContext()
                .WithMockUser(true);

            // When
            var result = controllerWithLoggedInUser.Index(DefaultCentreId);

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("RegisterInternalAdmin")
                .WithActionName("Index")
                .WithRouteValue("centreId", DefaultCentreId);
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
            var model = new SummaryViewModel
            {
                Terms = true,
            };

            var data = new RegistrationData
            {
                FirstName = "First",
                LastName = "Name",
                Centre = DefaultCentreId,
                JobGroup = DefaultJobGroupId,
                PasswordHash = "hash",
                PrimaryEmail = primaryEmail,
                CentreSpecificEmail = centreSpecificEmail,
                ProfessionalRegistrationNumber = DefaultPRN,
                HasProfessionalRegistrationNumber = true,
            };

            SetUpFakesForSuccessfulRegistration(primaryEmail, centreSpecificEmail, data, 1);

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
                                a.ProfessionalRegistrationNumber == DefaultPRN &&
                                a.JobGroup == DefaultJobGroupId
                        ),
                        true
                    )
                )
                .MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Confirmation");
        }

        [Test]
        public void
            SummaryPost_with_valid_information_sends_verification_email_to_primary_and_centre_emails()
        {
            // Given
            const string primaryEmail = "primary@email.com";
            const string? centreSpecificEmail = "centre@email.com";
            const int adminId = 1;
            const int userId = 1;

            var model = new SummaryViewModel
            {
                Terms = true,
                PrimaryEmail = primaryEmail,
                CentreSpecificEmail = centreSpecificEmail,
            };

            var data = new RegistrationData
            {
                FirstName = "First",
                LastName = "Name",
                Centre = DefaultCentreId,
                JobGroup = DefaultJobGroupId,
                PasswordHash = "hash",
                PrimaryEmail = primaryEmail,
                CentreSpecificEmail = centreSpecificEmail,
                ProfessionalRegistrationNumber = DefaultPRN,
                HasProfessionalRegistrationNumber = true,
            };

            SetUpFakesForSuccessfulRegistration(primaryEmail, centreSpecificEmail, data, adminId);

            // When
            controller.Summary(model);

            // Then
            A.CallTo(() => userDataService.GetUserIdByAdminId(adminId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.GetUserById(userId)).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                    A<UserAccount>._,
                    A<List<string>>.That.Matches(
                        list => ListTestHelper.ListOfStringsMatch(
                            list,
                            new List<string> { data.PrimaryEmail, data.CentreSpecificEmail }
                        )
                    ),
                    A<string>._
                )
            ).MustHaveHappenedOnceExactly();
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

        private void SetUpFakesForSuccessfulRegistration(
            string primaryEmail,
            string? centreSpecificEmail,
            RegistrationData data,
            int adminId
        )
        {
            const int userId = 1;

            controller.TempData.Set(data);
            A.CallTo(() => registerAdminService.IsRegisterAdminAllowed(DefaultCentreId, null)).Returns(true);
            if (centreSpecificEmail != null)
            {
                A.CallTo(() => userDataService.GetAdminUserByEmailAddress(centreSpecificEmail)).Returns(null);
            }

            A.CallTo(
                    () => centresService.IsAnEmailValidForCentreManager(
                        primaryEmail,
                        centreSpecificEmail,
                        DefaultCentreId
                    )
                )
                .Returns(true);
            A.CallTo(
                    () => registrationService.RegisterCentreManager(
                        A<AdminRegistrationModel>._,
                        true
                    )
                )
                .Returns(adminId);

            A.CallTo(
                    () => userDataService.GetUserIdByAdminId(adminId)
                )
                .Returns(userId);
            A.CallTo(
                    () => userService.GetUserById(userId)
                )
                .Returns(UserTestHelper.GetDefaultUserEntity(userId, primaryEmail, centreSpecificEmail));
            A.CallTo(
                    () => emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                        A<UserAccount>._,
                        A<List<string>>._,
                        A<string>._
                    )
                )
                .DoesNothing();
        }
    }
}
