using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.Common;
using FreshdeskApi.Client;
using FreshdeskApi.Client.CommonModels;
using FreshdeskApi.Client.Exceptions;
using FreshdeskApi.Client.Tickets.Models;
using FreshdeskApi.Client.Tickets.Requests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.ApiClients
{
    public interface IFreshdeskApiClient
    {
        Task<FreshDeskApiResponse> CreateNewTicket(CreateTicketRequest ticketRequest);

    }
    public class FreshdeskApiClient : IFreshdeskApiClient
    {
        private readonly FreshdeskHttpClient freshdeskHttpClient;
        private readonly ILogger<IFreshdeskApiClient> logger;
        private readonly IConfigDataService configDataService;

        public FreshdeskApiClient(ILogger<IFreshdeskApiClient> logger, IConfigDataService configDataService)
        {

            this.configDataService = configDataService;
            string freshdeskApiKey = configDataService.GetConfigValue(ConfigDataService.FreshdeskApiKey);
            string freshdeskApiBaseUri = configDataService.GetConfigValue(ConfigDataService.FreshdeskApiBaseUri);

            freshdeskHttpClient = FreshdeskHttpClient.Create(freshdeskApiBaseUri, freshdeskApiKey);

            this.logger = logger;
        }
        public async Task<FreshDeskApiResponse> CreateNewTicket(CreateTicketRequest ticketRequest)
        {

            FreshDeskApiResponse createRequestApiResponse = new FreshDeskApiResponse();
            string freshdeskAPICreateTicketUri = configDataService.GetConfigValue(ConfigDataService.FreshdeskAPICreateTicketUri);

            try
            {
                var ticketInfo = await freshdeskHttpClient.ApiOperationAsync<Ticket, CreateTicketRequest>
                     (HttpMethod.Post, freshdeskAPICreateTicketUri, ticketRequest, default).ConfigureAwait(false);

                createRequestApiResponse.StatusCode = 200;
                createRequestApiResponse.TicketId = ticketInfo.Id;
                return createRequestApiResponse;
            }
            catch (AuthenticationFailureException ex)
            {
                createRequestApiResponse = await FormatErrorMessage(await ex.Response.Content.ReadAsStreamAsync(default));
                logger.LogError(ex, "Freshdesk Api call failed with following message: {Message}", createRequestApiResponse);
            }
            catch (GeneralApiException ex)
            {
                createRequestApiResponse = await FormatErrorMessage(await ex.Response.Content.ReadAsStreamAsync(default));
                logger.LogError(ex, "Freshdesk Api call failed with following message: {Message}", createRequestApiResponse);
            }
            catch (InvalidFreshdeskRequest ex)
            {
                createRequestApiResponse = await FormatErrorMessage(await ex.Response.Content.ReadAsStreamAsync(default));
                logger.LogError(ex, "Freshdesk Api call failed with following message: {Message}", createRequestApiResponse);
            }
            catch (AuthorizationFailureException ex)
            {
                createRequestApiResponse = await FormatErrorMessage(await ex.Response.Content.ReadAsStreamAsync(default));
                logger.LogError(ex, "Freshdesk Api call failed with following message: {Message}", createRequestApiResponse);
            }
            catch (ResourceNotFoundException ex)
            {
                createRequestApiResponse = await FormatErrorMessage(await ex.Response.Content.ReadAsStreamAsync(default));
                logger.LogError(ex, "Freshdesk Api call failed with following message: {Message}", createRequestApiResponse);
            }
            catch (ResourceConflictException ex)
            {
                createRequestApiResponse = await FormatErrorMessage(await ex.Response.Content.ReadAsStreamAsync(default));
                logger.LogError(ex, "Freshdesk Api call failed with following message: {Message}", createRequestApiResponse);
            }
            catch (Exception e)
            {
                var message =
                    "Freshdesk Api call failed. " + $"Error message code {e.Message} " + $"when trying {freshdeskAPICreateTicketUri}";

                createRequestApiResponse = new FreshDeskApiResponse
                { StatusCode = 400, StatusMeaning = e.Message, FullErrorDetails = message };

                logger.LogError(e, "Freshdesk Api call failed with following message: {Message}", createRequestApiResponse);
            }

            return createRequestApiResponse;

        }

        private async Task<FreshDeskApiResponse> FormatErrorMessage(Stream contentStream)
        {
            using var streamReader = new StreamReader(contentStream);
            var message = await streamReader.ReadToEndAsync();
            string? fullErrorMessage = string.Empty;
            string? statusMeaning = string.Empty;

            var result = JsonConvert.DeserializeObject<ExportCsv>(message);

            if (result.ListExportErrors is { Count: > 0 })
            {
                statusMeaning = result.Description;
               
                foreach (var resultListExportError in result.ListExportErrors)
                {
                    fullErrorMessage += resultListExportError.Field + ": " + resultListExportError.Message + ";\n"; 
                }
            }
            else
            {
                statusMeaning = result.Message;
                fullErrorMessage = result.CodeError + ": " + result.Message;
            }

            FreshDeskApiResponse freshDeskApiResponse = new FreshDeskApiResponse
            {
                StatusCode = 400,
                StatusMeaning = statusMeaning,
                FullErrorDetails = fullErrorMessage
            };

            return freshDeskApiResponse;
        }
    }
}
