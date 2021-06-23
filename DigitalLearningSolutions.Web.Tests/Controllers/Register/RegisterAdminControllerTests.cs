namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class RegisterAdminControllerTests
    {
        private ICentresDataService centresDataService = null!;
        private RegisterAdminController controller = null!;
        private ICryptoService cryptoService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            centresDataService = A.Fake<ICentresDataService>();
            cryptoService = A.Fake<ICryptoService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            userService = A.Fake<IUserService>();
            controller = new RegisterAdminController(centresDataService, cryptoService, jobGroupsDataService)
                .WithDefaultContext()
                .WithMockTempData();
        }

        [Test]
        public void IndexGet_with_no_centreId_param_shows_error()
        {
            // When
            var result = controller.Index();

            // Then
            result.Should().BeNotFoundResult();
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
        public void IndexGet_with_centre_autoregistered_true_shows_error()
        {
            // Given
            const int centreId = 7;
            A.CallTo(() => centresDataService.GetCentreName(centreId)).Returns("My centre");
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).Returns((true, "email@email"));

            // When
            var result = controller.Index(centreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(centreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void IndexGet_with_centre_autoregisteremail_null_shows_error()
        {
            // Given
            const int centreId = 7;
            A.CallTo(() => centresDataService.GetCentreName(centreId)).Returns("Some centre");
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).Returns((false, null));

            // When
            var result = controller.Index(centreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(centreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void IndexGet_with_centre_autoregisteremail_empty_shows_error()
        {
            // Given
            const int centreId = 7;
            A.CallTo(() => centresDataService.GetCentreName(centreId)).Returns("Some centre");
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).Returns((false, string.Empty));

            // When
            var result = controller.Index(centreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(centreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void IndexGet_with_allowed_admin_registration_sets_data_correctly()
        {
            // Given
            const int centreId = 7;
            A.CallTo(() => centresDataService.GetCentreName(centreId)).Returns("Some centre");
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).Returns((false, "email@email"));

            // When
            var result = controller.Index(centreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(centreId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(centreId)).MustHaveHappened(1, Times.Exactly);
            var data = controller.TempData.Peek<RegistrationData>()!;
            data.Centre.Should().Be(centreId);
            result.Should().BeRedirectToActionResult().WithActionName("PersonalInformation");
        }
    }
}
