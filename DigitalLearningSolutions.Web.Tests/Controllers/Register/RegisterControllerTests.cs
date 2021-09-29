namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.Register;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.FeatureManagement;
    using NUnit.Framework;

    public class RegisterControllerTests
    {
        private CentreCustomPromptHelper centreCustomPromptHelper = null!;
        private ICentresDataService centresDataService = null!;
        private RegisterController controller = null!;
        private ICryptoService cryptoService = null!;
        private IFeatureManager featureManager = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IRegistrationService registrationService = null!;
        private IUserService userService = null!;
        private ISupervisorDelegateService supervisorDelegateService = null!;

        [SetUp]
        public void Setup()
        {
            centresDataService = A.Fake<ICentresDataService>();
            cryptoService = A.Fake<ICryptoService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            registrationService = A.Fake<IRegistrationService>();
            userService = A.Fake<IUserService>();
            centreCustomPromptHelper = A.Fake<CentreCustomPromptHelper>();
            featureManager = A.Fake<IFeatureManager>();
            supervisorDelegateService = A.Fake<ISupervisorDelegateService>();

            controller = new RegisterController(
                    centresDataService,
                    jobGroupsDataService,
                    registrationService,
                    cryptoService,
                    userService,
                    centreCustomPromptHelper,
                    featureManager,
                    supervisorDelegateService
                )
                .WithDefaultContext()
                .WithMockTempData();
        }

        [Test]
        public void PersonalInformationPost_with_existing_user_for_centre_fails_validation()
        {
            // Given
            var duplicateUser = UserTestHelper.GetDefaultDelegateUser();
            var model = new PersonalInformationViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = duplicateUser.CentreId,
                Email = duplicateUser.EmailAddress
            };
            A.CallTo(() => userService.IsDelegateEmailValidForCentre(model.Email!, model.Centre.Value))
                .Returns(false);

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => userService.IsDelegateEmailValidForCentre(model.Email!, model.Centre.Value)).MustHaveHappened();
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void PersonalInformationPost_with_existing_user_for_different_centre_is_allowed()
        {
            // Given
            controller.TempData.Set(new DelegateRegistrationData());
            var duplicateUser = UserTestHelper.GetDefaultDelegateUser();
            var model = new PersonalInformationViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = duplicateUser.CentreId + 1,
                Email = duplicateUser.EmailAddress
            };
            A.CallTo(() => userService.IsDelegateEmailValidForCentre(model.Email!, model.Centre.Value))
                .Returns(true);

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => userService.IsDelegateEmailValidForCentre(model.Email!, model.Centre.Value)).MustHaveHappened();
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
    }
}
