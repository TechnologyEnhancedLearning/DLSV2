namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using NUnit.Framework;

    public class FreshdeskServiceTests
    {
        private IFreshdeskApiClient freshdeskApiClient = null!;
        private IFreshdeskService freshdeskService = null!;

        [SetUp]
        public void SetUp()
        {
            freshdeskApiClient = A.Fake<IFreshdeskApiClient>();
            freshdeskService = A.Fake<IFreshdeskService>();
        }

        [Test]
        public void CreateNewTicket_returns_ticket_id_API_success()
        {

            // Arrange
            var ticketRequest = FreshedeshCreateTicketRequestHelper.FreshedeshCreateNewTicket();

            var expectedResponse = Builder<FreshDeskApiResponse>.CreateNew()
                .With(r => r.TicketId = 1)
                .And(r => r.StatusCode = 200)
                .Build();

            
            A.CallTo(() => freshdeskService.CreateNewTicket(ticketRequest)).Returns(expectedResponse);

            // Act
            var apiResponse = freshdeskService.CreateNewTicket(ticketRequest);

            //Assert 
            Assert.That(apiResponse, Is.EqualTo(expectedResponse));
        }
        [Test]
        public void CreateNewTicket_returns_API_Authentication_error()
        {
            // Arrange
            var ticketRequest = FreshedeshCreateTicketRequestHelper.FreshedeshCreateNewTicket();

            var expectedResponse = Builder<FreshDeskApiResponse>.CreateNew()
                .With(r => r.FullErrorDetails = "Authentication Error!")
                .And(r => r.StatusCode = 400)
                .Build();


            A.CallTo(() => freshdeskService.CreateNewTicket(ticketRequest)).Returns(expectedResponse);

            // Act
            var apiResponse = freshdeskService.CreateNewTicket(ticketRequest);

            //Assert 
            Assert.That(apiResponse, Is.EqualTo(expectedResponse));
        }
        [Test]
        public void CreateNewTicket_returns_API_error()
        {
            // Arrange
            var ticketRequest = FreshedeshCreateTicketRequestHelper.FreshedeshCreateNewTicket();

            var expectedResponse = Builder<FreshDeskApiResponse>.CreateNew()
                .With(r => r.FullErrorDetails = "It should be one of these values: 'Question,Incident,Problem,Feature Request,Refunds and Returns,Bulk orders,Refund,Service Task")
                .And(r => r.StatusCode = 400)
                .Build();


            A.CallTo(() => freshdeskService.CreateNewTicket(ticketRequest)).Returns(expectedResponse);

            // Act
            var apiResponse = freshdeskService.CreateNewTicket(ticketRequest);

            //Assert 
            Assert.That(apiResponse, Is.EqualTo(expectedResponse));
        }
        [Test]
        public async Task CreateNewTicket_returns_API_data_if_retrieved()
        {
            // Arrange
            var ticketRequest = FreshedeshCreateTicketRequestHelper.FreshedeshCreateNewTicket();

            var expectedResponse = Builder<FreshDeskApiResponse>.CreateNew()
                .With(r => r.TicketId = 120)
                .And(r => r.StatusCode = 200)
                .Build();


            A.CallTo(() => freshdeskService.CreateNewTicket(ticketRequest)).Returns(expectedResponse);

            // Act
            var apiResponse = freshdeskService.CreateNewTicket(ticketRequest);

            //Assert 
            Assert.That(apiResponse, Is.EqualTo(expectedResponse));
        }
    }
}
