using DigitalLearningSolutions.Data.ApiClients;
using DigitalLearningSolutions.Data.Models.Common;
using DigitalLearningSolutions.Web.ViewModels.Support.RequestSupportTicket;
using FreshdeskApi.Client.CommonModels;
using FreshdeskApi.Client.Tickets.Models;
using FreshdeskApi.Client.Tickets.Requests;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using DigitalLearningSolutions.Web.Models;

namespace DigitalLearningSolutions.Web.Services
{
    using System.Linq;

    public interface IFreshdeskService
    {
        FreshDeskApiResponse CreateNewTicket(RequestSupportTicketData ticketInfo);
    }

    public class FreshdeskService : IFreshdeskService
    {
        private readonly IFreshdeskApiClient freshdeskApiClient;
        private readonly ILogger<IFreshdeskService> logger;

        public FreshdeskService(
            IFreshdeskApiClient freshdeskApiClient,
            ILogger<IFreshdeskService> logger
        )
        {
            this.freshdeskApiClient = freshdeskApiClient;
            this.logger = logger;
        }

        public FreshDeskApiResponse CreateNewTicket(RequestSupportTicketData ticketDetails)
        {
            FreshDeskApiResponse freshDeskApiResponse = new FreshDeskApiResponse();

            try
            {
                CreateTicketRequest ticketInfo = MapToCreateTicketRequest(ticketDetails);
                freshDeskApiResponse = Task.Run(() => freshdeskApiClient.CreateNewTicket(ticketInfo)).Result; 
            }
            catch (Exception e)
            {
                freshDeskApiResponse.FullErrorDetails = e.Message;
                freshDeskApiResponse.StatusCode = 400;
            }

            return freshDeskApiResponse;
        }

        private CreateTicketRequest MapToCreateTicketRequest(RequestSupportTicketData ticketDetails)
        {
            List<FileAttachment> filesAttachment = new List<FileAttachment>();

            if (ticketDetails.RequestAttachment != null && !ticketDetails.RequestAttachment.Any())
            {
                foreach (var requestAttachment in ticketDetails.RequestAttachment)
                {
                    FileAttachment fileAttachment = new FileAttachment
                    {
                        FileBytes = requestAttachment.Content,
                        Name = requestAttachment.FileName
                    };

                    filesAttachment.Add(fileAttachment);
                }
            }

            Dictionary<string, object> customFieldsDictionary = new Dictionary<string, object>();
            if (ticketDetails.CentreName != null)
            {
                customFieldsDictionary.Add("cf_fsm_service_location", ticketDetails.CentreName);
            }

            CreateTicketRequest ticketRequest = new CreateTicketRequest(
                TicketStatus.Open,
                TicketPriority.Medium,
                TicketSource.Portal,
                ticketDetails.RequestDescription!,
                "",
                null,
                email: ticketDetails.UserCentreEmail,
                files: filesAttachment,
                subject: ticketDetails.RequestSubject,
                groupId: 80000639888,
                productId: 80000003097,
                ticketType:ticketDetails.FreshdeskRequestType,
                customFields: customFieldsDictionary
            );

            return ticketRequest;
        }
    }
}
