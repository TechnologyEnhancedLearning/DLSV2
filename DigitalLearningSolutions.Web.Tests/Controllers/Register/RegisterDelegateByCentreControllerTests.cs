namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
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
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;
    using SummaryViewModel = DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre.SummaryViewModel;

    public class RegisterDelegateByCentreControllerTests
    {
        private IConfiguration config = null!;
        private RegisterDelegateByCentreController controller = null!;
        private ICryptoService cryptoService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private PromptsService promptsService = null!;
        private IRegistrationService registrationService = null!;
        private IUserDataService userDataService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            userService = A.Fake<IUserService>();
            userDataService = A.Fake<IUserDataService>();
            promptsService = A.Fake<PromptsService>();
            cryptoService = A.Fake<ICryptoService>();
            registrationService = A.Fake<IRegistrationService>();
            config = A.Fake<IConfiguration>();

            controller = new RegisterDelegateByCentreController(
                    jobGroupsDataService,
                    userService,
                    promptsService,
                    cryptoService,
                    userDataService,
                    registrationService,
                    config
                )
                .WithDefaultContext()
                .WithMockTempData();
        }

        [Test]
        public void PersonalInformationPost_with_duplicate_email_for_centre_fails_validation()
        {
            // Given
            var duplicateUser = UserTestHelper.GetDefaultDelegateUser();
            var model = new RegisterDelegatePersonalInformationViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = duplicateUser.CentreId,
                CentreSpecificEmail = duplicateUser.EmailAddress,
            };
            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentre(
                        model.CentreSpecificEmail!,
                        model.Centre.Value,
                        null
                    )
                )
                .Returns(true);

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentre(
                        model.CentreSpecificEmail!,
                        model.Centre.Value,
                        null
                    )
                )
                .MustHaveHappened();
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void PersonalInformationPost_with_duplicate_email_for_different_centre_is_allowed()
        {
            // Given
            controller.TempData.Set(new DelegateRegistrationByCentreData());
            var duplicateUser = UserTestHelper.GetDefaultDelegateUser();
            var model = new RegisterDelegatePersonalInformationViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = duplicateUser.CentreId + 1,
                CentreSpecificEmail = duplicateUser.EmailAddress,
            };
            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentre(
                        model.CentreSpecificEmail!,
                        model.Centre.Value,
                        null
                    )
                )
                .Returns(false);

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentre(
                        model.CentreSpecificEmail!,
                        model.Centre.Value,
                        null
                    )
                )
                .MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("LearnerInformation");
        }

        [Test]
        public void PersonalInformationPost_updates_tempdata_correctly()
        {
            // Given
            const string firstName = "Test";
            const string lastName = "User";
            const string email = "test@email.com";

            controller.TempData.Set(new DelegateRegistrationByCentreData());
            var model = new RegisterDelegatePersonalInformationViewModel
            {
                FirstName = firstName,
                LastName = lastName,
                CentreSpecificEmail = email,
                Centre = 1,
            };
            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentre(
                        model.CentreSpecificEmail!,
                        model.Centre.Value,
                        null
                    )
                )
                .Returns(false);

            // When
            controller.PersonalInformation(model);

            // Then
            var data = controller.TempData.Peek<DelegateRegistrationByCentreData>()!;
            data.FirstName.Should().Be(firstName);
            data.LastName.Should().Be(lastName);
            data.CentreSpecificEmail.Should().Be(email);
        }

        [Test]
        public void LearnerInformationPost_updates_tempdata_correctly()
        {
            // Given
            const int jobGroupId = 3;
            const string answer1 = "answer1";
            const string answer2 = "answer2";
            const string answer3 = "answer3";
            const string answer4 = "answer4";
            const string answer5 = "answer5";
            const string answer6 = "answer6";
            const string professionalRegistrationNumber = "PRN1234";

            controller.TempData.Set(new DelegateRegistrationByCentreData { Centre = 1 });
            var model = new LearnerInformationViewModel
            {
                JobGroup = jobGroupId,
                Answer1 = answer1,
                Answer2 = answer2,
                Answer3 = answer3,
                Answer4 = answer4,
                Answer5 = answer5,
                Answer6 = answer6,
                HasProfessionalRegistrationNumber = true,
                ProfessionalRegistrationNumber = professionalRegistrationNumber,
            };

            // When
            controller.LearnerInformation(model);

            // Then
            var data = controller.TempData.Peek<DelegateRegistrationByCentreData>()!;
            data.JobGroup.Should().Be(jobGroupId);
            data.Answer1.Should().Be(answer1);
            data.Answer2.Should().Be(answer2);
            data.Answer3.Should().Be(answer3);
            data.Answer4.Should().Be(answer4);
            data.Answer5.Should().Be(answer5);
            data.Answer6.Should().Be(answer6);
            data.HasProfessionalRegistrationNumber.Should().BeTrue();
            data.ProfessionalRegistrationNumber.Should().Be(professionalRegistrationNumber);
        }

        [Test]
        public void WelcomeEmailPost_updates_tempdata_correctly()
        {
            // Given
            controller.TempData.Set(new DelegateRegistrationByCentreData { PasswordHash = "hash" });
            var date = new DateTime(2200, 7, 7);
            var model = new WelcomeEmailViewModel
            {
                Day = date.Day,
                Month = date.Month,
                Year = date.Year,
            };

            // When
            controller.WelcomeEmail(model);

            // Then
            var data = controller.TempData.Peek<DelegateRegistrationByCentreData>()!;
            data.WelcomeEmailDate.Should().Be(date);
            data.IsPasswordSet.Should().BeFalse();
            data.PasswordHash.Should().BeNull();
        }

        [Test]
        public void PasswordPost_with_no_password_updates_tempdata_correctly()
        {
            // Given
            controller.TempData.Set(new DelegateRegistrationByCentreData());
            var model = new PasswordViewModel { Password = null };
            A.CallTo(() => cryptoService.GetPasswordHash(A<string>._)).Returns("hash");

            // When
            controller.Password(model);

            // Then
            A.CallTo(() => cryptoService.GetPasswordHash(A<string>._)).MustNotHaveHappened();
            var data = controller.TempData.Peek<DelegateRegistrationByCentreData>()!;
            data.PasswordHash.Should().BeNull();
        }

        [Test]
        public void PasswordPost_with_password_updates_tempdata_correctly()
        {
            // Given
            controller.TempData.Set(new DelegateRegistrationByCentreData());
            var model = new PasswordViewModel { Password = "pwd" };
            const string passwordHash = "hash";
            A.CallTo(() => cryptoService.GetPasswordHash(A<string>._)).Returns(passwordHash);

            // When
            controller.Password(model);

            // Then
            A.CallTo(() => cryptoService.GetPasswordHash(A<string>._)).MustHaveHappened(1, Times.Exactly);
            var data = controller.TempData.Peek<DelegateRegistrationByCentreData>()!;
            data.PasswordHash.Should().Be(passwordHash);
        }

        [Test]
        public void SummaryPost_updates_tempdata_correctly()
        {
            // Given
            const string sampleDelegateNumber = "CR7";
            var data = new DelegateRegistrationByCentreData
            {
                FirstName = "Test",
                LastName = "User",
                PrimaryEmail = "test@mail.com",
                Centre = 5,
                JobGroup = 0,
                WelcomeEmailDate = new DateTime(2200, 7, 7),
            };
            controller.TempData.Set(data);
            var model = new SummaryViewModel();
            A.CallTo(
                    () => registrationService.RegisterDelegateByCentre(
                        A<DelegateRegistrationModel>._,
                        A<string>._,
                        A<bool>._
                    )
                )
                .Returns(sampleDelegateNumber);

            // When
            controller.Summary(model);

            // Then
            var delegateNumber = (string?)controller.TempData.Peek("delegateNumber");
            var passwordSet = (bool)controller.TempData.Peek("passwordSet");
            delegateNumber.Should().Be(sampleDelegateNumber);
            passwordSet.Should().Be(data.IsPasswordSet);
        }

        [Test]
        public void Summary_post_registers_delegate_with_expected_values()
        {
            // Given
            const string candidateNumber = "TN1";
            var data = RegistrationDataHelper.GetDefaultDelegateRegistrationByCentreData(
                welcomeEmailDate: DateTime.Now
            );
            controller.TempData.Set(data);
            A.CallTo(
                    () => registrationService.RegisterDelegateByCentre(
                        A<DelegateRegistrationModel>._,
                        A<string>._,
                        A<bool>._
                    )
                )
                .Returns(candidateNumber);

            // When
            var result = controller.Summary(new SummaryViewModel());

            // Then
            A.CallTo(
                    () =>
                        registrationService.RegisterDelegateByCentre(
                            A<DelegateRegistrationModel>.That.Matches(
                                d =>
                                    d.FirstName == data.FirstName &&
                                    d.LastName == data.LastName &&
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
                                    !d.UserIsActive &&
                                    d.Approved &&
                                    !d.IsSelfRegistered &&
                                    d.NotifyDate == data.WelcomeEmailDate &&
                                    d.ProfessionalRegistrationNumber == data.ProfessionalRegistrationNumber
                            ),
                            A<string>._,
                            false
                        )
                )
                .MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Confirmation");
        }

        [Test]
        public void Summary_post_returns_500_error_with_unexpected_register_error()
        {
            // Given
            var data = RegistrationDataHelper.GetDefaultDelegateRegistrationByCentreData();
            controller.TempData.Set(data);
            A.CallTo(
                    () => registrationService.RegisterDelegateByCentre(
                        A<DelegateRegistrationModel>._,
                        A<string>._,
                        A<bool>._
                    )
                )
                .Throws(new DelegateCreationFailedException(DelegateCreationError.UnexpectedError));

            // When
            var result = controller.Summary(new SummaryViewModel());

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        [Test]
        public void Summary_post_returns_redirect_to_index_with_email_in_use_register_error()
        {
            // Given
            var data = RegistrationDataHelper.GetDefaultDelegateRegistrationByCentreData();
            controller.TempData.Set(data);
            A.CallTo(
                    () => registrationService.RegisterDelegateByCentre(
                        A<DelegateRegistrationModel>._,
                        A<string>._,
                        A<bool>._
                    )
                )
                .Throws(new DelegateCreationFailedException(DelegateCreationError.EmailAlreadyInUse));

            // When
            var result = controller.Summary(new SummaryViewModel());

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }
    }
}
