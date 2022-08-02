namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class RegisterAdminServiceTests
    {
        private const int DefaultCentreId = 7;
        private const string DefaultCentreEmail = "centre@email.com";
        private IUserDataService userDataService = null!;
        private ICentresDataService centresDataService = null!;
        private IRegisterAdminService registerAdminService = null!;

        [SetUp]
        public void Setup()
        {
            userDataService = A.Fake<IUserDataService>();
            centresDataService = A.Fake<ICentresDataService>();
            registerAdminService = new RegisterAdminService(userDataService, centresDataService);
        }

        [Test]
        public void IsRegisterAdminAllowed_with_centre_autoregistered_true_returns_false()
        {
            // Given
            A.CallTo(() => userDataService.GetAdminsByCentreId(DefaultCentreId)).Returns(new List<AdminEntity>());
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((true, DefaultCentreEmail));

            // When
            var result = registerAdminService.IsRegisterAdminAllowed(DefaultCentreId);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void IsRegisterAdminAllowed_with_centre_autoregisteremail_null_returns_false()
        {
            // Given
            A.CallTo(() => userDataService.GetAdminsByCentreId(DefaultCentreId)).Returns(new List<AdminEntity>());
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((false, null));

            // When
            var result = registerAdminService.IsRegisterAdminAllowed(DefaultCentreId);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void IsRegisterAdminAllowed_with_centre_autoregisteremail_whitespace_returns_false()
        {
            // Given
            A.CallTo(() => userDataService.GetAdminsByCentreId(DefaultCentreId)).Returns(new List<AdminEntity>());
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((false, "   "));

            // When
            var result = registerAdminService.IsRegisterAdminAllowed(DefaultCentreId);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void IsRegisterAdminAllowed_with_active_centre_manager_returns_false()
        {
            // Given
            var adminEntity = UserTestHelper.GetDefaultAdminEntity(
                centreId: DefaultCentreId,
                isCentreManager: true,
                active: true
            );
            A.CallTo(() => userDataService.GetAdminsByCentreId(DefaultCentreId))
                .Returns(new List<AdminEntity> { adminEntity });
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((false, DefaultCentreEmail));

            // When
            var result = registerAdminService.IsRegisterAdminAllowed(DefaultCentreId);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void IsRegisterAdminAllowed_with_inactive_centre_returns_false()
        {
            // Given
            A.CallTo(() => userDataService.GetAdminsByCentreId(DefaultCentreId))
                .Returns(new List<AdminEntity>());
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((false, DefaultCentreEmail));
            A.CallTo(() => centresDataService.GetCentreDetailsById(DefaultCentreId))
                .Returns(CentreTestHelper.GetDefaultCentre(active: false));

            // When
            var result = registerAdminService.IsRegisterAdminAllowed(DefaultCentreId);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void IsRegisterAdminAllowed_with_logged_in_user_already_an_admin_of_the_centre_returns_false()
        {
            // Given
            const int loggedInUserId = 2;
            var adminAccount = UserTestHelper.GetDefaultAdminAccount(
                userId: loggedInUserId,
                centreId: DefaultCentreId
            );

            A.CallTo(() => userDataService.GetAdminsByCentreId(DefaultCentreId))
                .Returns(new List<AdminEntity>());
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((false, DefaultCentreEmail));
            A.CallTo(() => centresDataService.GetCentreDetailsById(DefaultCentreId))
                .Returns(CentreTestHelper.GetDefaultCentre(active: true));
            A.CallTo(() => userDataService.GetAdminAccountsByUserId(loggedInUserId))
                .Returns(new List<AdminAccount> { adminAccount });

            // When
            var result = registerAdminService.IsRegisterAdminAllowed(DefaultCentreId, loggedInUserId);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void IsRegisterAdminAllowed_with_correct_data_returns_true()
        {
            // Given
            A.CallTo(() => userDataService.GetAdminsByCentreId(DefaultCentreId))
                .Returns(new List<AdminEntity>());
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((false, DefaultCentreEmail));
            A.CallTo(() => centresDataService.GetCentreDetailsById(DefaultCentreId))
                .Returns(CentreTestHelper.GetDefaultCentre(active: true));

            // When
            var result = registerAdminService.IsRegisterAdminAllowed(DefaultCentreId);

            // Then
            result.Should().BeTrue();
        }
    }
}
