namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class SupportTicketDataServiceTests
    {
        private ISupportTicketDataService supportTicketDataService = null!;
        
        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            supportTicketDataService = new SupportTicketDataService(connection);
        }

        [Test]
        public void GetNumberOfUnarchivedTicketsForCentreId_returns_expected_count()
        {
            // When
            var count = supportTicketDataService.GetNumberOfUnarchivedTicketsForCentreId(615);

            // Then
            count.Should().Be(5);
        }

        [Test]
        public void GetNumberOfUnarchivedTicketsForAdminId_returns_expected_count()
        {
            // When
            var count = supportTicketDataService.GetNumberOfUnarchivedTicketsForAdminId(3534);

            // Then
            count.Should().Be(5);
        }
    }
}
