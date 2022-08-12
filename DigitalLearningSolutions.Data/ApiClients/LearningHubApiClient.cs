namespace DigitalLearningSolutions.Data.ApiClients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using JsonSerializer = System.Text.Json.JsonSerializer;

    public interface ILearningHubApiClient
    {
        Task<ResourceSearchResult> SearchResource(
            string text,
            int? catalogueId,
            int? offset = null,
            int? limit = null,
            IEnumerable<string>? resourceTypes = null
        );

        Task<ResourceReferenceWithResourceDetails> GetResourceByReferenceId(int resourceReferenceId);

        Task<BulkResourceReferences> GetBulkResourcesByReferenceIds(
            IEnumerable<int> resourceReferenceIds
        );

        Task<CataloguesResult> GetCatalogues();
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
            int? catalogueId = null,
            int? offset = null,
            int? limit = null,
            IEnumerable<string>? resourceTypes = null
        )
        {
            var queryString = GetSearchQueryString(text, catalogueId, offset, limit, resourceTypes);

            var response = await GetStringAsync($"/Resource/Search?{queryString}");
            var result = JsonConvert.DeserializeObject<ResourceSearchResult>(response);
            return result;
        }

        public async Task<ResourceReferenceWithResourceDetails> GetResourceByReferenceId(int resourceReferenceId)
        {
            var response = await GetStringAsync($"/Resource/{resourceReferenceId}");
            var result = JsonConvert.DeserializeObject<ResourceReferenceWithResourceDetails>(response);
            return result;
        }

        public async Task<BulkResourceReferences> GetBulkResourcesByReferenceIds(
            IEnumerable<int> resourceReferenceIds
        )
        {
            var referenceIds = resourceReferenceIds.ToList();
            var objectWithReferenceIds = new { referenceIds = referenceIds.ToList() };

            var jsonString = JsonSerializer.Serialize(objectWithReferenceIds);

            var response = await GetStringAsync($"/Resource/Bulk?{jsonString}");
            var result = JsonConvert.DeserializeObject<BulkResourceReferences>(response);
            return result;
        }

        public async Task<CataloguesResult> GetCatalogues()
        {
            var response = await GetStringAsync("/Catalogues");
            var result = JsonConvert.DeserializeObject<CataloguesResult>(response);
            return result;
        }

        private static string GetSearchQueryString(
            string text,
            int? catalogueId = null,
            int? offset = null,
            int? limit = null,
            IEnumerable<string>? resourceTypes = null
        )
        {
            var textQueryString = GetQueryString("text", text);
            var catalogueIdString = GetQueryString("catalogueId", catalogueId?.ToString());
            var offSetQueryString = GetQueryString("offset", offset?.ToString());
            var limitQueryString = GetQueryString("limit", limit?.ToString());

            var queryStrings = new List<string>
                { textQueryString, catalogueIdString, offSetQueryString, limitQueryString };

            if (resourceTypes != null)
            {
                var resourceTypesQueryStrings = resourceTypes
                    .Where(x => !string.IsNullOrEmpty(x))
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

        private async Task<string?> GetStringAsync(string requestUri)
        {
            var response = await client.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            var message =
                "Learning Hub Open Api call failed. " +
                $"Status code {(int)response.StatusCode} ({response.ReasonPhrase}) " +
                $"when trying {requestUri}";
            logger.LogWarning(message);
            throw new LearningHubResponseException(message, response.StatusCode);
        }
    }
}
