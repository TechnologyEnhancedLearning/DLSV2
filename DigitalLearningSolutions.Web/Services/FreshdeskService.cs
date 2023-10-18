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
        Task<FreshDeskApiResponse> CreateNewTicketAsync(RequestSupportTicketData ticketInfo);
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

        public async Task<FreshDeskApiResponse> CreateNewTicketAsync(RequestSupportTicketData ticketDetails)
        {
            FreshDeskApiResponse freshDeskApiResponse = new FreshDeskApiResponse();

            try
            {
                CreateTicketRequest ticketInfo = MapToCreateTicketRequest(ticketDetails);
                freshDeskApiResponse = await freshdeskApiClient.CreateNewTicket(ticketInfo);
            }
            catch (Exception e)
            {
                freshDeskApiResponse.FullErrorDetails = e.Message;
                freshDeskApiResponse.StatusCode = 400;
            }

            return freshDeskApiResponse;
        }

        private CreateTicketRequest MapToCreateTicketRequest(RequestSupportTicketData ticketInfoModel)
        {
            List<FileAttachment> filesAttachment = new List<FileAttachment>();

            if (ticketInfoModel.RequestAttachment != null && !ticketInfoModel.RequestAttachment.Any())
            {
                foreach (var requestAttachment in ticketInfoModel.RequestAttachment)
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
            if (ticketInfoModel.CentreName != null)
            {
                customFieldsDictionary.Add("cf_fsm_service_location", ticketInfoModel.CentreName);
            }

            CreateTicketRequest ticketRequest = new CreateTicketRequest(
                TicketStatus.Open,
                TicketPriority.Medium,
                TicketSource.Portal,
                ticketInfoModel.RequestDescription!,
                "",
                null,
                email: ticketInfoModel.UserCentreEmail,
                files: filesAttachment,
                subject: ticketInfoModel.RequestSubject,
                groupId: 80000639888,
                productId: 80000003097,
                customFields: customFieldsDictionary
            );

            return ticketRequest;
        }
    }
}
