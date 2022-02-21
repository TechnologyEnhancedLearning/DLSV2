namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class SupportTicketServiceTests
    {
        private ISupportTicketDataService supportTicketDataService = null!;
        private ISupportTicketService supportTicketService = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            supportTicketDataService = A.Fake<ISupportTicketDataService>();
            userDataService = A.Fake<IUserDataService>();
            supportTicketService = new SupportTicketService(supportTicketDataService, userDataService);
        }

        [Test]
        [TestCase(true, 10)]
        [TestCase(false, 5)]
        public void GetNumberOfTicketsForCentreAdmin_returns_expected_count(bool isCentreManager, int expectedCount)
        {
            // Given
            const int adminId = 7;
            const int centreId = 2;
            const int centreTicketCount = 10;
            const int adminTicketCount = 5;
            A.CallTo(() => userDataService.GetAdminUserById(adminId)).Returns(
                UserTestHelper.GetDefaultAdminUser(isCentreManager: isCentreManager)
            );
            A.CallTo(() => supportTicketDataService.GetNumberOfUnarchivedTicketsForCentreId(centreId))
                .Returns(centreTicketCount);
            A.CallTo(() => supportTicketDataService.GetNumberOfUnarchivedTicketsForAdminId(adminId))
                .Returns(adminTicketCount);

            // When
            var result = supportTicketService.GetNumberOfTicketsForCentreAdmin(centreId, adminId);

            // Then
            result.Should().Be(expectedCount);
        }

        [Test]
        public void GetNumberOfTicketsForCentreAdmin_returns_zero_with_null_admin()
        {
            // Given
            const int adminId = 7;
            const int centreId = 2;
            A.CallTo(() => userDataService.GetAdminUserById(adminId)).Returns(null);

            // When
            var result = supportTicketService.GetNumberOfTicketsForCentreAdmin(centreId, adminId);

            // Then
            result.Should().Be(0);
        }
    }
}
