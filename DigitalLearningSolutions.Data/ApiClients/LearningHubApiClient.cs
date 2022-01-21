namespace DigitalLearningSolutions.Data.ApiClients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public interface ILearningHubApiClient
    {
        Task<ResourceSearchResult> SearchResource(
            string text,
            int? offset = null,
            int? limit = null,
            IEnumerable<string>? resourceTypes = null
        );

        Task<ResourceReferenceWithResourceDetails> GetResourceByReferenceId(int resourceReferenceId);

        Task<BulkResourceReferences> GetBulkResourcesByReferenceIds(
            IEnumerable<int> resourceReferenceIds
        );
    }

    public class LearningHubApiClient : ILearningHubApiClient
    {
        private readonly HttpClient client;
        private readonly ILogger<ILearningHubApiClient> logger;

        public LearningHubApiClient(HttpClient httpClient, ILogger<ILearningHubApiClient> logger, IConfiguration config)
        {
            string learningHubOpenApiKey = config.GetLearningHubOpenApiKey();
            string learningHubOpenApiBaseUrl = config.GetLearningHubOpenApiBaseUrl();

            client = httpClient;
            client.BaseAddress = new Uri(learningHubOpenApiBaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("X-API-KEY", learningHubOpenApiKey);

            this.logger = logger;
        }

        public async Task<ResourceSearchResult> SearchResource(
            string text,
            int? offset = null,
            int? limit = null,
            IEnumerable<string>? resourceTypes = null
        )
        {
            var queryString = GetSearchQueryString(text, offset, limit, resourceTypes);

            var response = await GetAsyncAndProcessResult($"/Resource/Search?{queryString}");
            var result = JsonConvert.DeserializeObject<ResourceSearchResult>(response);
            return result;
        }

        public async Task<ResourceReferenceWithResourceDetails> GetResourceByReferenceId(int resourceReferenceId)
        {
            var response = await GetAsyncAndProcessResult($"/Resource/{resourceReferenceId}");
            var result = JsonConvert.DeserializeObject<ResourceReferenceWithResourceDetails>(response);
            return result;
        }

        public async Task<BulkResourceReferences> GetBulkResourcesByReferenceIds(
            IEnumerable<int> resourceReferenceIds
        )
        {
            var referenceIdQueryStrings =
                resourceReferenceIds.Select(id => GetQueryString("resourceReferenceIds", id.ToString()));
            var queryString = string.Join("&", referenceIdQueryStrings);

            var response = await GetAsyncAndProcessResult($"/Resource/Bulk?{queryString}");
            var result = JsonConvert.DeserializeObject<BulkResourceReferences>(response);
            return result;
        }

        private static string GetSearchQueryString(
            string text,
            int? offset = null,
            int? limit = null,
            IEnumerable<string>? resourceTypes = null
        )
        {
            var textQueryString = GetQueryString("text", text);
            var offSetQueryString = GetQueryString("offset", offset.ToString());
            var limitQueryString = GetQueryString("limit", limit.ToString());

            var queryStrings = new List<string> { textQueryString, offSetQueryString, limitQueryString };

            if (resourceTypes != null)
            {
                var resourceTypesQueryStrings = resourceTypes.Where(x => !string.IsNullOrEmpty(x))
                    .Select(r => GetQueryString("resourceTypes", r.ToString()));
                queryStrings.AddRange(resourceTypesQueryStrings);
            }

            var validQueryStrings = queryStrings.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return string.Join("&", validQueryStrings);
        }

        private static string GetQueryString(string key, string? value)
        {
            return string.IsNullOrEmpty(value) ? "" : $"{key}={value}";
        }

        private async Task<string?> GetAsyncAndProcessResult(string requestUri)
        {
            var response = await client.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            var message =
                $"Learning Hub Open Api call failed. Status code {response.StatusCode.GetHashCode()} ({response.StatusCode}) when trying {requestUri}";
            logger.LogWarning(message);
            throw new LearningHubResponseException(message, response.StatusCode);
        }
    }
}
