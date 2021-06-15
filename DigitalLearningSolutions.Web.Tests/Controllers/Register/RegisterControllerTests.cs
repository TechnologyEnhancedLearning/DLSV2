namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class RegisterControllerTests
    {
        private RegisterController controller = null!;
        private ICentresDataService centresDataService = null!;
        private ICryptoService cryptoService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IRegistrationService registrationService = null!;
        private IUserService userService = null!;
        private CustomPromptHelper customPromptHelper = null!;

        [SetUp]
        public void Setup()
        {
            centresDataService = A.Fake<ICentresDataService>();
            cryptoService = A.Fake<ICryptoService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            registrationService = A.Fake<IRegistrationService>();
            userService = A.Fake<IUserService>();
            customPromptHelper = A.Fake<CustomPromptHelper>();
            controller = new RegisterController
                    (centresDataService, jobGroupsDataService, registrationService, cryptoService, userService, customPromptHelper)
                .WithDefaultContext()
                .WithMockTempData();
        }

        [Test]
        public void IndexPost_with_existing_user_for_centre_fails_validation()
        {
            // Given
            var duplicateUser = UserTestHelper.GetDefaultDelegateUser();
            var model = new RegisterViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = duplicateUser.CentreId,
                Email = duplicateUser.EmailAddress
            };
            A.CallTo(() => userService.GetUsersByEmailAddress(duplicateUser.EmailAddress!))
                .Returns((null, new List<DelegateUser> {duplicateUser}));

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => userService.GetUsersByEmailAddress(duplicateUser.EmailAddress!)).MustHaveHappened();
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void IndexPost_with_existing_user_for_different_centre_is_allowed()
        {
            // Given
            controller.TempData.Set(new DelegateRegistrationData());
            var duplicateUser = UserTestHelper.GetDefaultDelegateUser();
            var model = new RegisterViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = duplicateUser.CentreId + 1,
                Email = duplicateUser.EmailAddress
            };
            A.CallTo(() => userService.GetUsersByEmailAddress(duplicateUser.EmailAddress!))
                .Returns((null, new List<DelegateUser> {duplicateUser}));

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => userService.GetUsersByEmailAddress(duplicateUser.EmailAddress!)).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("LearnerInformation");
        }
    }
}
