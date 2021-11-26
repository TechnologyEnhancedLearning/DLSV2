namespace DigitalLearningSolutions.Data.Helpers.ExternalApis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Models.LearningHubApiClient;
    using Microsoft.Extensions.Configuration;
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

        Task<BulkResourceReference> GetBulkResourcesByReferenceIds(
            IEnumerable<int> resourceReferenceIds
        );
    }

    public class LearningHubApiClient : ILearningHubApiClient
    {
        private readonly HttpClient client;

        public LearningHubApiClient(HttpClient httpClient, IConfiguration config)
        {
            string learningHubOpenApiKey = config.GetLearningHubOpenApiKey();
            string learningHubOpenApiBaseUrl = config.GetLearningHubOpenApiBaseUrl();

            client = httpClient;
            client.BaseAddress = new Uri(learningHubOpenApiBaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("X-API-KEY", learningHubOpenApiKey);
        }

        public async Task<ResourceSearchResult> SearchResource(
            string text,
            int? offset = null,
            int? limit = null,
            IEnumerable<string>? resourceTypes = null
        )
        {
            var queryString = GetSearchQueryString(text, offset, limit, resourceTypes);

            var response = await client.GetStringAsync($"/Resource/Search?{queryString}");
            var result = JsonConvert.DeserializeObject<ResourceSearchResult>(response);
            return result;
        }

        public async Task<ResourceReferenceWithResourceDetails> GetResourceByReferenceId(int resourceReferenceId)
        {
            var response = await client.GetStringAsync($"/Resource/{resourceReferenceId}");
            var result = JsonConvert.DeserializeObject<ResourceReferenceWithResourceDetails>(response);
            return result;
        }

        public async Task<BulkResourceReference> GetBulkResourcesByReferenceIds(
            IEnumerable<int> resourceReferenceIds
        )
        {
            var referenceIdQueryStrings =
                resourceReferenceIds.Select(id => GetQueryString("resourceReferenceIds", id.ToString()));
            var queryString = string.Join("&", referenceIdQueryStrings);

            var response = await client.GetStringAsync($"/Resource/Bulk?{queryString}");
            var result = JsonConvert.DeserializeObject<BulkResourceReference>(response);
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
    }
}
