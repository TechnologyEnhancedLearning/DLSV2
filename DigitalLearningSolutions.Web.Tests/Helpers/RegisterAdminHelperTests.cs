namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class RegisterAdminHelperTests
    {
        private const int DefaultCentreId = 7;
        private const string DefaultCentreEmail = "centre@email.com";
        private IUserDataService userDataService = null!;
        private ICentresDataService centresDataService = null!;
        private IRegisterAdminHelper registerAdminHelper = null!;

        [SetUp]
        public void Setup()
        {
            userDataService = A.Fake<IUserDataService>();
            centresDataService = A.Fake<ICentresDataService>();
            registerAdminHelper = new RegisterAdminHelper(userDataService, centresDataService);
        }

        [Test]
        public void IsRegisterAdminAllowed_with_centre_autoregistered_true_returns_false()
        {
            // Given
            A.CallTo(() => userDataService.GetAdminsByCentreId(DefaultCentreId)).Returns(new List<AdminEntity>());
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((true, DefaultCentreEmail));

            // When
            var result = registerAdminHelper.IsRegisterAdminAllowed(DefaultCentreId);

            // Then
            A.CallTo(() => userDataService.GetAdminsByCentreId(DefaultCentreId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .MustHaveHappenedOnceExactly();
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
            var result = registerAdminHelper.IsRegisterAdminAllowed(DefaultCentreId);

            // Then
            A.CallTo(() => userDataService.GetAdminsByCentreId(DefaultCentreId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .MustHaveHappenedOnceExactly();
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
            var result = registerAdminHelper.IsRegisterAdminAllowed(DefaultCentreId);

            // Then
            A.CallTo(() => userDataService.GetAdminsByCentreId(DefaultCentreId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .MustHaveHappenedOnceExactly();
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
            var result = registerAdminHelper.IsRegisterAdminAllowed(DefaultCentreId);

            // Then
            A.CallTo(() => userDataService.GetAdminsByCentreId(DefaultCentreId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .MustHaveHappenedOnceExactly();
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
            var result = registerAdminHelper.IsRegisterAdminAllowed(DefaultCentreId);

            // Then
            A.CallTo(() => userDataService.GetAdminsByCentreId(DefaultCentreId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => centresDataService.GetCentreDetailsById(DefaultCentreId)).MustHaveHappenedOnceExactly();
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
            var result = registerAdminHelper.IsRegisterAdminAllowed(DefaultCentreId);

            // Then
            A.CallTo(() => userDataService.GetAdminsByCentreId(DefaultCentreId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => centresDataService.GetCentreDetailsById(DefaultCentreId)).MustHaveHappenedOnceExactly();
            result.Should().BeTrue();
        }
    }
}
