namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    internal class ViewDelegateControllerTests
    {
        private IUserDataService userDataService = null!;
        private ViewDelegateController viewDelegateController = null!;

        [SetUp]
        public void SetUp()
        {
            var centreCustomPromptsService = A.Fake<ICentreCustomPromptsService>();
            var centreCustomPromptsHelper = new CentreCustomPromptHelper(centreCustomPromptsService);
            userDataService = A.Fake<IUserDataService>();
            var courseService = A.Fake<ICourseService>();
            var passwordResetService = A.Fake<IPasswordResetService>();

            viewDelegateController = new ViewDelegateController(
                    userDataService,
                    centreCustomPromptsHelper,
                    courseService,
                    passwordResetService
                )
                .WithDefaultContext()
                .WithMockUser(true);
        }

        [Test]
        public void Deactivating_delegate_returns_redirect()
        {
            // given
            A.CallTo(() => userDataService.GetDelegateUserCardById(1))
                .Returns(new DelegateUserCard { CentreId = 2, Id = 1 });

            // when
            var result = viewDelegateController.DeactivateDelegate(1);

            // then
            result.Should().BeRedirectToActionResult();
        }
    }
}
