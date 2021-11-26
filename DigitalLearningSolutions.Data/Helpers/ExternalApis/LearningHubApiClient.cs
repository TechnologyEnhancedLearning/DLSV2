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

        Task<ResourceReferenceWithReferenceDetails> GetResourceByReferenceId(int resourceReferenceId);

        Task<BulkResourceReferences> GetBulkResourcesByReferenceIds(
            IEnumerable<int> resourceReferenceIds
        );
    }

    public class LearningHubApiClient : ILearningHubApiClient
    {
        private readonly HttpClient client;

        public LearningHubApiClient(HttpClient httpClient, IConfiguration config)
        {
            string learningHubApiKey = config.GetLearningHubApiKey();
            string learningHubApiBaseUrl = config.GetLearningHubApiBaseUrl();

            client = httpClient;
            client.BaseAddress = new Uri(learningHubApiBaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("X-API-KEY", learningHubApiKey);
        }

        public async Task<ResourceSearchResult> SearchResource(
            string text,
            int? offset = null,
            int? limit = null,
            IEnumerable<string>? resourceTypes = null
        )
        {
            var queryParams = GetSearchQueryParams(text, offset, limit, resourceTypes);

            var response = await client.GetStringAsync($"/Resource/Search?{queryParams}");
            var result = JsonConvert.DeserializeObject<ResourceSearchResult>(response);
            return result;
        }

        public async Task<ResourceReferenceWithReferenceDetails> GetResourceByReferenceId(int resourceReferenceId)
        {
            var response = await client.GetStringAsync($"/Resource/{resourceReferenceId}");
            var result = JsonConvert.DeserializeObject<ResourceReferenceWithReferenceDetails>(response);
            return result;
        }

        public async Task<BulkResourceReferences> GetBulkResourcesByReferenceIds(
            IEnumerable<int> resourceReferenceIds
        )
        {
            var referenceIdQueryStrings =
                resourceReferenceIds.Select(id => GetQueryString("resourceReferenceIds", id.ToString()));
            var queryString = JoinQueryStrings(referenceIdQueryStrings);

            var response = await client.GetStringAsync($"/Resource/Bulk?{queryString}");
            var result = JsonConvert.DeserializeObject<BulkResourceReferences>(response);
            return result;
        }

        private static string GetSearchQueryParams(
            string text,
            int? offset = null,
            int? limit = null,
            IEnumerable<string>? resourceTypes = null
        )
        {
            var textQueryString = GetQueryString("text", text);
            var offSetQueryString = GetQueryString("offset", offset.ToString());
            var limitQueryString = GetQueryString("limit", limit.ToString());

            var resourceTypesQueryString = "";
            if (resourceTypes != null)
            {
                var resourceTypesQueryStrings =
                    resourceTypes.Select(r => GetQueryString("resourceTypes", r.ToString()));
                resourceTypesQueryString = JoinQueryStrings(resourceTypesQueryStrings);
            }

            var queryStrings = new[] { textQueryString, offSetQueryString, limitQueryString, resourceTypesQueryString };
            return JoinQueryStrings(queryStrings);
        }

        private static string GetQueryString(string key, string? value)
        {
            return string.IsNullOrEmpty(value) ? "" : $"{key}={value}";
        }

        private static string JoinQueryStrings(IEnumerable<string> queryStrings)
        {
            var validQueryStrings = queryStrings.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return string.Join("&", validQueryStrings);
        }
    }
}
