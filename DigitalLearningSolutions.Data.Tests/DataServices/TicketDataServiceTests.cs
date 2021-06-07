namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class TicketDataServiceTests
    {
        private ISupportTicketDataService ticketDataService = null!;
        
        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            ticketDataService = new SupportTicketDataService(connection);
        }

        [Test]
        public void GetNumberOfUnarchivedTicketsForCentreId_returns_expected_count()
        {
            // When
            var count = ticketDataService.GetNumberOfUnarchivedTicketsForCentreId(2);

            // Then
            count.Should().Be(0);
        }
    }
}
