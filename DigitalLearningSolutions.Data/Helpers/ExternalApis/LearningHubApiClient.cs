namespace DigitalLearningSolutions.Data.Helpers.ExternalApis
{
    using System;
    using System.Collections.Generic;
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
            IEnumerable<int>? resourceReferenceIds = null
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
            IEnumerable<int>? resourceReferenceIds = null
        )
        {
            var queryParams = GetBulkResourcesQueryParams(resourceReferenceIds);

            var response = await client.GetStringAsync($"/Resource/Bulk{queryParams}");
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
            var resourceTypesQueryString = GetMultipleQueryStrings("resourceTypes", resourceTypes);

            var queryParams =
                $"{textQueryString}{offSetQueryString}{limitQueryString}{resourceTypesQueryString}";

            queryParams = GetStringWithoutLastCharacter(queryParams);

            return queryParams;
        }

        private static string GetBulkResourcesQueryParams(
            IEnumerable<int>? resourceIds = null
        )
        {
            if (resourceIds == null)
            {
                return string.Empty;
            }

            var queryParams = "?";

            foreach (var resourceId in resourceIds)
            {
                queryParams += "resourceReferenceIds=" + resourceId + "&";
            }

            queryParams = GetStringWithoutLastCharacter(queryParams);

            return queryParams;
        }

        private static string GetMultipleQueryStrings(string key, IEnumerable<string>? values)
        {
            var queryString = "";

            if (values == null)
            {
                return queryString;
            }

            foreach (var value in values)
            {
                queryString += string.IsNullOrEmpty(value) ? "" : $"{key}={value}&";
            }

            return queryString;
        }

        private static string GetQueryString(string key, string? value)
        {
            return string.IsNullOrEmpty(value) ? "" : $"{key}={value}&";
        }

        private static string GetStringWithoutLastCharacter(string input)
        {
            return input.Remove(input.Length - 1, 1);
        }
    }
}
