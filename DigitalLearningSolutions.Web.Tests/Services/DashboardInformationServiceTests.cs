namespace DigitalLearningSolutions.Web.Tests.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class DashboardInformationServiceTests
    {
        private const int CentreId = 1;
        private const int AdminId = 1;
        private ICentresDataService centresDataService = null!;
        private ICentresService centresService = null!;
        private ICourseDataService courseDataService = null!;
        private IDashboardInformationService dashboardInformationService = null!;
        private ISupportTicketDataService supportTicketDataService = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            supportTicketDataService = A.Fake<ISupportTicketDataService>();
            userDataService = A.Fake<IUserDataService>();
            centresDataService = A.Fake<ICentresDataService>();
            centresService = A.Fake<ICentresService>();
            courseDataService = A.Fake<ICourseDataService>();
            centresDataService = A.Fake<ICentresDataService>();
            dashboardInformationService = new DashboardInformationService(
                supportTicketDataService,
                userDataService,
                courseDataService,
                centresService,
                centresDataService
            );
        }

        [Test]
        public void GetDashboardInformationForCentre_returns_null_with_null_centre()
        {
            // Given
            A.CallTo(() => centresDataService.GetCentreDetailsById(CentreId)).Returns(null);
            A.CallTo(() => userDataService.GetAdminUserById(AdminId))
                .Returns(UserTestHelper.GetDefaultAdminUser(AdminId));

            // When
            var result = dashboardInformationService.GetDashboardInformationForCentre(CentreId, AdminId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetDashboardInformationForCentre_returns_null_with_null_admin()
        {
            // Given
            A.CallTo(() => centresDataService.GetCentreDetailsById(CentreId))
                .Returns(CentreTestHelper.GetDefaultCentre(CentreId));
            A.CallTo(() => userDataService.GetAdminUserById(AdminId))
                .Returns(null);

            // When
            var result = dashboardInformationService.GetDashboardInformationForCentre(CentreId, AdminId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        [TestCase(false, 5)]
        [TestCase(true, 10)]
        public void GetDashboardInformationForCentre_returns_expected_model_with_correct_ticket_count(
            bool isCentreManager,
            int expectedTicketCount
        )
        {
            // Given
            const int delegateCount = 100;
            const int adminCount = 10;
            const int courseCount = 50;
            const int ticketCountForAdmin = 5;
            const int ticketCountForCentre = 10;
            const int centreRank = 3;
            var adminUser = UserTestHelper.GetDefaultAdminUser(AdminId, isCentreManager: isCentreManager);
            GivenServicesReturnProvidedValues(
                adminUser,
                delegateCount,
                courseCount,
                adminCount,
                centreRank,
                ticketCountForAdmin,
                ticketCountForCentre
            );

            // When
            var result = dashboardInformationService.GetDashboardInformationForCentre(CentreId, AdminId);

            // Then
            var expectedModel = new CentreDashboardInformation(
                CentreTestHelper.GetDefaultCentre(CentreId),
                adminUser,
                delegateCount,
                courseCount,
                adminCount,
                expectedTicketCount,
                centreRank
            );

            result.Should().BeEquivalentTo(expectedModel);
        }

        private void GivenServicesReturnProvidedValues(
            AdminUser? adminUser,
            int delegateCount,
            int courseCount,
            int adminCount,
            int centreRank,
            int ticketCountForAdmin,
            int ticketCountForCentre
        )
        {
            A.CallTo(() => centresDataService.GetCentreDetailsById(CentreId))
                .Returns(CentreTestHelper.GetDefaultCentre(CentreId));
            A.CallTo(() => userDataService.GetAdminUserById(AdminId))
                .Returns(adminUser);

            A.CallTo(() => userDataService.GetNumberOfApprovedDelegatesAtCentre(CentreId)).Returns(delegateCount);
            A.CallTo(
                () =>
                    courseDataService.GetNumberOfActiveCoursesAtCentreFilteredByCategory(
                        CentreId,
                        adminUser!.CategoryId
                    )
            ).Returns(courseCount);
            A.CallTo(() => userDataService.GetNumberOfActiveAdminsAtCentre(CentreId)).Returns(adminCount);
            A.CallTo(() => centresService.GetCentreRankForCentre(CentreId)).Returns(centreRank);
            A.CallTo(() => supportTicketDataService.GetNumberOfUnarchivedTicketsForAdminId(AdminId))
                .Returns(ticketCountForAdmin);
            A.CallTo(() => supportTicketDataService.GetNumberOfUnarchivedTicketsForCentreId(CentreId))
                .Returns(ticketCountForCentre);
        }
    }
}
