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
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class RegisterDelegateByCentreControllerTests
    {
        private RegisterDelegateByCentreController controller = null!;
        private ICryptoService cryptoService = null!;
        private CustomPromptHelper customPromptHelper = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IRegistrationService registrationService = null!;
        private IUserDataService userDataService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            userService = A.Fake<IUserService>();
            userDataService = A.Fake<IUserDataService>();
            customPromptHelper = A.Fake<CustomPromptHelper>();
            cryptoService = A.Fake<ICryptoService>();
            registrationService = A.Fake<IRegistrationService>();
            controller = new RegisterDelegateByCentreController(
                    jobGroupsDataService,
                    userService,
                    customPromptHelper,
                    cryptoService,
                    userDataService,
                    registrationService
                )
                .WithDefaultContext()
                .WithMockTempData();
        }

        [Test]
        public void PersonalInformationPost_with_duplicate_email_for_centre_fails_validation()
        {
            // Given
            var duplicateUser = UserTestHelper.GetDefaultDelegateUser();
            var model = new PersonalInformationViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = duplicateUser.CentreId,
                Email = duplicateUser.EmailAddress,
                Alias = "testUser"
            };
            A.CallTo(() => userService.GetUsersByEmailAddress(duplicateUser.EmailAddress!))
                .Returns((null, new List<DelegateUser> { duplicateUser }));

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => userService.GetUsersByEmailAddress(duplicateUser.EmailAddress!)).MustHaveHappened();
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void PersonalInformationPost_with_duplicate_email_for_different_centre_is_allowed()
        {
            // Given
            controller.TempData.Set(new DelegateRegistrationByCentreData());
            var duplicateUser = UserTestHelper.GetDefaultDelegateUser();
            var model = new PersonalInformationViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = duplicateUser.CentreId + 1,
                Email = duplicateUser.EmailAddress,
                Alias = "testUser"
            };
            A.CallTo(() => userService.GetUsersByEmailAddress(duplicateUser.EmailAddress!))
                .Returns((null, new List<DelegateUser> { duplicateUser }));

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => userService.GetUsersByEmailAddress(duplicateUser.EmailAddress!)).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("LearnerInformation");
        }

        [Test]
        public void PersonalInformationPost_with_duplicate_alias_for_centre_fails_validation()
        {
            // Given
            const string duplicateAlias = "alias1";
            var duplicateUser = UserTestHelper.GetDefaultDelegateUser();
            var model = new PersonalInformationViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = duplicateUser.CentreId,
                Email = "unique@email",
                Alias = duplicateAlias
            };
            A.CallTo(() => userDataService.GetAllDelegateUsersByUsername(duplicateAlias))
                .Returns(new List<DelegateUser> { duplicateUser });

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => userDataService.GetAllDelegateUsersByUsername(duplicateAlias)).MustHaveHappened();
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void PersonalInformationPost_with_duplicate_alias_for_different_centre_is_allowed()
        {
            // Given
            const string duplicateAlias = "alias1";
            controller.TempData.Set(new DelegateRegistrationByCentreData());
            var duplicateUser = UserTestHelper.GetDefaultDelegateUser();
            var model = new PersonalInformationViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = duplicateUser.CentreId + 1,
                Email = duplicateUser.EmailAddress,
                Alias = duplicateAlias
            };
            A.CallTo(() => userDataService.GetAllDelegateUsersByUsername(duplicateAlias))
                .Returns(new List<DelegateUser> { duplicateUser });

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => userDataService.GetAllDelegateUsersByUsername(duplicateAlias)).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("LearnerInformation");
        }
    }
}
