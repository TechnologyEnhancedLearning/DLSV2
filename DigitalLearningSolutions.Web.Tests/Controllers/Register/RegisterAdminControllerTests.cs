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
        private const int DefaultCentreId = 7;
        private const string DefaultPrimaryEmail = "primary@email.com";
        private const string DefaultSecondaryEmail = "centre@email.com";
        private ICentresDataService centresDataService = null!;
        private ICentresService centresService = null!;
        private RegisterAdminController controller = null!;
        private ICryptoService cryptoService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
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
            userService = A.Fake<IUserService>();
            controller = new RegisterAdminController(
                    centresDataService,
                    centresService,
                    cryptoService,
                    jobGroupsDataService,
                    registrationService,
                    userDataService,
                    userService
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
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void IndexGet_with_centre_autoregistered_true_shows_AccessDenied_error()
        {
            // Given
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).Returns("My centre");
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((true, "email@email"));
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(DefaultCentreId)).Returns(new List<AdminUser>());

            // When
            var result = controller.Index(DefaultCentreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(DefaultCentreId)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void IndexGet_with_centre_autoregisteremail_null_shows_AccessDenied_error()
        {
            // Given
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).Returns("Some centre");
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId)).Returns((false, null));
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(DefaultCentreId)).Returns(new List<AdminUser>());

            // When
            var result = controller.Index(DefaultCentreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(DefaultCentreId)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void IndexGet_with_centre_autoregisteremail_empty_shows_AccessDenied_error()
        {
            // Given
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).Returns("Some centre");
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((false, string.Empty));
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(DefaultCentreId)).Returns(new List<AdminUser>());

            // When
            var result = controller.Index(DefaultCentreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(DefaultCentreId)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void IndexGet_with_centre_with_active_centre_manager_shows_AccessDenied_error()
        {
            // Given
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).Returns("Some centre");
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((false, "email@email"));

            var centreManagerAdmin = new AdminUser { CentreId = DefaultCentreId, IsCentreManager = true };
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(DefaultCentreId))
                .Returns(new List<AdminUser> { centreManagerAdmin });

            // When
            var result = controller.Index(DefaultCentreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(DefaultCentreId)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void IndexGet_with_allowed_admin_registration_sets_data_correctly()
        {
            // Given
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).Returns("Some centre");
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((false, "email@email"));
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(DefaultCentreId)).Returns(new List<AdminUser>());

            // When
            var result = controller.Index(DefaultCentreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(DefaultCentreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(DefaultCentreId)).MustHaveHappened(1, Times.Exactly);
            var data = controller.TempData.Peek<RegistrationData>()!;
            data.Centre.Should().Be(DefaultCentreId);
            result.Should().BeRedirectToActionResult().WithActionName("PersonalInformation");
        }

        [Test]
        public void PersonalInformationPost_with_wrong_primary_autoregisteremail_for_centre_fails_validation()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModelAndSetTempData(null);
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(false);
            A.CallTo(() => centresService.DoesEmailMatchCentre(DefaultPrimaryEmail, DefaultCentreId)).Returns(false);

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => centresService.DoesEmailMatchCentre(DefaultPrimaryEmail, DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            controller.ModelState[nameof(PersonalInformationViewModel.PrimaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void PersonalInformationPost_with_wrong_secondary_autoregisteremail_for_centre_fails_validation()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModelAndSetTempData();

            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(false);
            A.CallTo(() => userService.EmailIsInUse(DefaultSecondaryEmail)).Returns(false);
            A.CallTo(() => centresService.DoesEmailMatchCentre(DefaultSecondaryEmail, DefaultCentreId)).Returns(false);

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => centresService.DoesEmailMatchCentre(DefaultSecondaryEmail, DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            controller.ModelState[nameof(PersonalInformationViewModel.SecondaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void PersonalInformationPost_with_email_already_in_use_fails_validation()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModelAndSetTempData();

            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(true);
            A.CallTo(() => userService.EmailIsInUse(DefaultSecondaryEmail)).Returns(true);

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.EmailIsInUse(DefaultSecondaryEmail)).MustHaveHappenedOnceExactly();
            controller.ModelState[nameof(PersonalInformationViewModel.PrimaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            controller.ModelState[nameof(PersonalInformationViewModel.SecondaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void PersonalInformationPost_with_correct_unique_emails_are_allowed()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModelAndSetTempData();

            A.CallTo(
                () => centresService.DoesEmailMatchCentre(
                    DefaultSecondaryEmail,
                    DefaultCentreId
                )
            ).Returns(true);
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(false);
            A.CallTo(() => userService.EmailIsInUse(DefaultSecondaryEmail)).Returns(false);

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => centresService.DoesEmailMatchCentre(DefaultSecondaryEmail, DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(
                () => centresService.DoesEmailMatchCentre(
                    DefaultSecondaryEmail,
                    DefaultCentreId
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.EmailIsInUse(DefaultSecondaryEmail)).MustHaveHappenedOnceExactly();
            result.Should().BeRedirectToActionResult().WithActionName("LearnerInformation");
        }

        [Test]
        public void PersonalInformationPost_with_correct_unique_primary_email_only_is_allowed()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModelAndSetTempData(null);

            A.CallTo(
                () => centresService.DoesEmailMatchCentre(
                    DefaultPrimaryEmail,
                    DefaultCentreId
                )
            ).Returns(true);
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(false);

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => centresService.DoesEmailMatchCentre(DefaultPrimaryEmail, DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(
                () => centresService.DoesEmailMatchCentre(
                    DefaultPrimaryEmail,
                    DefaultCentreId
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).MustHaveHappenedOnceExactly();
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
        [TestCase("primary@email", "secondary@email")]
        public void SummaryPost_with_valid_information_registers_expected_admin(
            string primaryEmail,
            string? secondaryEmail
        )
        {
            // Given
            const int jobGroupId = 1;
            var centreEmailOrPrimaryIfNull = secondaryEmail ?? primaryEmail;
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
                SecondaryEmail = secondaryEmail,
                ProfessionalRegistrationNumber = professionalRegistrationNumber,
                HasProfessionalRegistrationNumber = true,
            };
            controller.TempData.Set(data);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((false, centreEmailOrPrimaryIfNull));
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(primaryEmail)).Returns(null);
            if (secondaryEmail != null)
            {
                A.CallTo(() => userDataService.GetAdminUserByEmailAddress(secondaryEmail)).Returns(null);
            }

            A.CallTo(() => centresService.DoesEmailMatchCentre(centreEmailOrPrimaryIfNull, DefaultCentreId))
                .Returns(true);
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
                                a.PrimaryEmail == data.PrimaryEmail! &&
                                a.SecondaryEmail == data.SecondaryEmail &&
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

        private PersonalInformationViewModel GetDefaultPersonalInformationViewModelAndSetTempData(
            string? secondaryEmail = DefaultSecondaryEmail
        )
        {
            var model = new PersonalInformationViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = DefaultCentreId,
                PrimaryEmail = DefaultPrimaryEmail,
                SecondaryEmail = secondaryEmail,
            };

            var data = new RegistrationData(DefaultCentreId);
            controller.TempData.Set(data);

            return model;
        }
    }
}
