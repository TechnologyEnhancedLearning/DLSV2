namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using System;
    using System.Reflection;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Controllers.Register;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using GDS.MultiPageFormData;
    using GDS.MultiPageFormData.Enums;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;
    using SummaryViewModel = DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre.SummaryViewModel;

    public class RegisterDelegateByCentreControllerTests
    {
        private IConfiguration config = null!;
        private RegisterDelegateByCentreController controller = null!;
        private ICryptoService cryptoService = null!;
        private IJobGroupsService jobGroupsService = null!;
        private PromptsService promptsService = null!;
        private IRegistrationService registrationService = null!;
        private IUserDataService userDataService = null!;
        private IUserService userService = null!;
        private IClockUtility clockUtility = null!;
        private IMultiPageFormService multiPageFormService = null!;
        private IGroupsService groupsService = null!;

        [SetUp]
        public void Setup()
        {
            jobGroupsService = A.Fake<IJobGroupsService>();
            userDataService = A.Fake<IUserDataService>();
            userService = A.Fake<IUserService>();
            promptsService = A.Fake<PromptsService>();
            cryptoService = A.Fake<ICryptoService>();
            registrationService = A.Fake<IRegistrationService>();
            config = A.Fake<IConfiguration>();
            clockUtility = A.Fake<IClockUtility>();
            multiPageFormService = A.Fake<IMultiPageFormService>();
            groupsService = A.Fake<IGroupsService>();


            controller = new RegisterDelegateByCentreController(
                    jobGroupsService,
                    promptsService,
                    cryptoService,
                    userDataService,
                    registrationService,
                    config,
                    clockUtility,
                    userService,
                    multiPageFormService,
                    groupsService
                )
                .WithDefaultContext()
                .WithMockTempData()
                .WithMockUser(true, 101, 1, 1, 1);
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
                    () => userService.EmailIsHeldAtCentre(
                        model.CentreSpecificEmail!,
                        model.Centre.Value
                    )
                )
                .Returns(true);

            // When
            var result = controller.PersonalInformation(model);

            // Then
           result.Should().BeRedirectToActionResult().WithActionName("LearnerInformation"); 
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
                    () => userService.EmailIsHeldAtCentre(
                        model.CentreSpecificEmail!,
                        model.Centre.Value
                    )
                )
                .Returns(false);

            // When
            var result = controller.PersonalInformation(model);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("LearnerInformation");
        }

        [Test]
        public void PersonalInformationPost_updates_tempdata_correctly()
        {
            // Given
            const string firstName = "Test";
            const string lastName = "User";
            const string email = "test@email.com";

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
                        model.Centre.Value
                    )
                )
                .Returns(false);

            // When
            var result = controller.PersonalInformation(model);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => multiPageFormService.SetMultiPageFormData(
                        A<DelegateRegistrationByCentreData>.That.Matches(d => d.FirstName == firstName && d.LastName == lastName && d.CentreSpecificEmail == email && d.Centre == 1),
                        MultiPageFormDataFeature.CustomWebForm,
                        controller.TempData
                    )
                ).MustHaveHappenedOnceExactly();
                result.Should().BeRedirectToActionResult().WithActionName("LearnerInformation");
            }
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
            var result = controller.LearnerInformation(model);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => multiPageFormService.SetMultiPageFormData(
                        A<DelegateRegistrationByCentreData>._,
                        MultiPageFormDataFeature.CustomWebForm,
                        controller.TempData
                    )
                ).MustHaveHappenedOnceExactly();
                result.Should().BeRedirectToActionResult().WithActionName("AddToGroup");
            }
        }

        [Test]
        public void WelcomeEmailPost_updates_tempdata_correctly()
        {
            // Given
            var date = new DateTime(2200, 7, 7);
            var model = new WelcomeEmailViewModel
            {
                Day = date.Day,
                Month = date.Month,
                Year = date.Year,
            };

            // When
            var result = controller.WelcomeEmail(model);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => multiPageFormService.SetMultiPageFormData(
                        A<DelegateRegistrationByCentreData>.That.Matches(d => d.WelcomeEmailDate == date),
                        MultiPageFormDataFeature.CustomWebForm,
                        controller.TempData
                    )
                ).MustHaveHappenedOnceExactly();
                result.Should().BeRedirectToActionResult().WithActionName("Password");
            }
        }

        [Test]
        public void PasswordPost_with_no_password_updates_tempdata_correctly()
        {
            // Given
            var model = new PasswordViewModel { Password = null };
            A.CallTo(() => cryptoService.GetPasswordHash(A<string>._)).Returns("hash");

            // When
            var result = controller.Password(model);

            // Then
            A.CallTo(() => cryptoService.GetPasswordHash(A<string>._)).MustNotHaveHappened();
            using (new AssertionScope())
            {
                A.CallTo(
                   () => multiPageFormService.SetMultiPageFormData(
                       A<DelegateRegistrationByCentreData>._,
                       MultiPageFormDataFeature.CustomWebForm,
                       controller.TempData
                   )
               ).MustHaveHappenedOnceExactly();

                result.Should().BeRedirectToActionResult().WithActionName("Summary");
            }
        }

        [Test]
        public void PasswordPost_with_password_updates_tempdata_correctly()
        {
            // Given
            var model = new PasswordViewModel { Password = "pwd" };
            const string passwordHash = "hash";
            A.CallTo(() => cryptoService.GetPasswordHash(A<string>._)).Returns(passwordHash);

            // When
            var result = controller.Password(model);

            // Then
            A.CallTo(() => cryptoService.GetPasswordHash(A<string>._)).MustHaveHappened(1, Times.Exactly);
            using (new AssertionScope())
            {
                A.CallTo(
                    () => multiPageFormService.SetMultiPageFormData(
                        A<DelegateRegistrationByCentreData>.That.Matches(d => d.PasswordHash == passwordHash),
                        MultiPageFormDataFeature.CustomWebForm,
                        controller.TempData
                    )
                ).MustHaveHappenedOnceExactly();
                result.Should().BeRedirectToActionResult().WithActionName("Summary");
            }
        }

        [Test]
        public void Summary_post_registers_delegate_with_expected_values()
        {
            // Given
            const string candidateNumber = "TN1";
            var data = RegistrationDataHelper.GetDefaultDelegateRegistrationByCentreData(
                welcomeEmailDate: DateTime.Now
            );
            A.CallTo(() => multiPageFormService.GetMultiPageFormData<DelegateRegistrationByCentreData>(
               MultiPageFormDataFeature.AddCustomWebForm("DelegateRegistrationByCentreCWF"),
               controller.TempData
           )).Returns(data);
            A.CallTo(
                    () => registrationService.RegisterDelegateByCentre(
                        A<DelegateRegistrationModel>._,
                        A<string>._,
                        A<bool>._,
                        A<int>._,
                        A<int>._
                    )
                )
                .Returns(candidateNumber);

            // When
            var result = controller.Summary(new SummaryViewModel(data));

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
                                    d.UserIsActive &&
                                    d.Approved &&
                                    !d.IsSelfRegistered &&
                                    d.NotifyDate == data.WelcomeEmailDate &&
                                    d.ProfessionalRegistrationNumber == data.ProfessionalRegistrationNumber
                            ),
                            A<string>._,
                            false,
                            1,
                            null
                        )
                )
                .MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Confirmation");
        }

        [Test]
        public void Summary_post_creates_new_delegate_group_if_required()
        {
            // Given
            var data = RegistrationDataHelper.GetDefaultDelegateRegistrationByCentreData(
                welcomeEmailDate: DateTime.Now
            );
            var model = RegistrationMappingHelper.MapCentreRegistrationToDelegateRegistrationModel(data);
            data.AddToGroupOption = 2;
            data.NewGroupName = "Test delegate group";
            data.NewGroupDescription = "Test description";
            A.CallTo(() => multiPageFormService.GetMultiPageFormData<DelegateRegistrationByCentreData>(
               MultiPageFormDataFeature.AddCustomWebForm("DelegateRegistrationByCentreCWF"),
               controller.TempData
           )).Returns(data);
            // When
            var result = controller.Summary(new SummaryViewModel(data));

            // Then
            A.CallTo(() => groupsService.AddDelegateGroup(
                A<int>._,
                data.NewGroupName,
                data.NewGroupDescription,
                A<int>._,
                0,
                false,
                false,
                false
            )).MustHaveHappened();
        }

        [Test]
        public void Summary_post_adds_delegate_to_existing_group_if_chosen()
        {
            // Given
            var data = RegistrationDataHelper.GetDefaultDelegateRegistrationByCentreData(
                welcomeEmailDate: DateTime.Now
            );
            var model = RegistrationMappingHelper.MapCentreRegistrationToDelegateRegistrationModel(data);
            data.AddToGroupOption = 1;
            data.ExistingGroupId = 2;
            A.CallTo(() => multiPageFormService.GetMultiPageFormData<DelegateRegistrationByCentreData>(
               MultiPageFormDataFeature.AddCustomWebForm("DelegateRegistrationByCentreCWF"),
               controller.TempData
           )).Returns(data);
            // When
            var result = controller.Summary(new SummaryViewModel(data));

            // Then
            A.CallTo(() => registrationService.RegisterDelegateByCentre(
                A<DelegateRegistrationModel>._,
                A<string>._,
                false,
                A<int>._,
                2
            )).MustHaveHappened();
        }
    }
}
