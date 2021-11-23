namespace DigitalLearningSolutions.Data.Helpers.ExternalApis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Models.LearningHubApiClient;
    using Microsoft.AspNetCore.WebUtilities;
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

        Task<ResourceReferenceWithReferenceDetails> GetResourceByReferenceId(int resourceReferenceId);

        Task<ResourceReferenceWithReferenceDetails> GetBulkResourcesByReferenceIds(
            IEnumerable<int> resourceReferenceIds
        );
    }

    public class LearningHubApiClient : ILearningHubApiClient
    {
        private readonly HttpClient client;
        private readonly string learningHubApiBaseUrl;
        private readonly string learningHubApiKey;

        public LearningHubApiClient(HttpClient httpClient, IConfiguration config)
        {
            learningHubApiKey = config.GetLearningHubApiKey();
            learningHubApiBaseUrl =
                "https://uks-learninghubnhsuk-openapi-dev.azurewebsites.net";

            client = new HttpClient { BaseAddress = new Uri(learningHubApiBaseUrl) };
            client.DefaultRequestHeaders.Accept.Clear();
        }

        public async Task<ResourceSearchResult> SearchResource(
            string text,
            int? offset = null,
            int? limit = null,
            IEnumerable<string>? resourceTypes = null
        )
        {
            const string searchEndpoint = "/Resource/Search";

            var endpointPath = learningHubApiBaseUrl + searchEndpoint;
            var queryParams = GetSearchQueryParams(text, offset, limit, resourceTypes);
            var requestUrl = QueryHelpers.AddQueryString(endpointPath, queryParams);

            var response = await client.GetStringAsync(requestUrl);
            var result = JsonConvert.DeserializeObject<ResourceSearchResult>(response);
            return result;
        }

        public async Task<ResourceReferenceWithReferenceDetails> GetResourceByReferenceId(int resourceReferenceId)
        {
            const string resourceEndpoint = "/Resource/";
            var requestUrl = learningHubApiBaseUrl + resourceEndpoint + resourceReferenceId;

            var response = await client.GetStringAsync(requestUrl);
            var result = JsonConvert.DeserializeObject<ResourceReferenceWithReferenceDetails>(response);
            return result;
        }

        public async Task<ResourceReferenceWithReferenceDetails> GetBulkResourcesByReferenceIds(
            IEnumerable<int> resourceReferenceIds
        )
        {
            const string bulkEndpoint = "/Resource/Bulk";
            var endpointPath = learningHubApiBaseUrl + bulkEndpoint;
            var queryParams = GetBulkQueryParams(resourceReferenceIds);
            var requestUrl = QueryHelpers.AddQueryString(endpointPath, queryParams);

            var response = await client.GetStringAsync(requestUrl);
            var result = JsonConvert.DeserializeObject<ResourceReferenceWithReferenceDetails>(response);
            return result;
        }

        private Dictionary<string, string> GetSearchQueryParams(
            string text,
            int? offset = null,
            int? limit = null,
            IEnumerable<string>? resourceTypes = null
        )
        {
            var queryParams = new Dictionary<string, string>
            {
                { "token", learningHubApiKey },
                { "q", text },
                { "hits", limit.ToString() },
                { "offset", offset.ToString() },
            };

            if (resourceTypes.Any())
            {
                foreach (var resourceType in resourceTypes)
                {
                    queryParams.Add("resource_type", resourceType);
                }
            }

            return queryParams;
        }

        private Dictionary<string, string> GetBulkQueryParams(IEnumerable<int>? resourceIds = null)
        {
            var queryParams = new Dictionary<string, string>();

            if (resourceIds.Any())
            {
                foreach (var id in resourceIds)
                {
                    queryParams.Add("reference_id", id.ToString());
                }
            }

            return queryParams;
        }
    }
}
