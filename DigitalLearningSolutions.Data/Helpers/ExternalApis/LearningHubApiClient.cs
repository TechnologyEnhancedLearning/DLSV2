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
        private readonly string learningHubApiBaseUrl;

        public LearningHubApiClient(HttpClient httpClient, IConfiguration config)
        {
            string learningHubApiKey = config.GetLearningHubApiKey();
            learningHubApiBaseUrl =
                "https://uks-learninghubnhsuk-openapi-dev.azurewebsites.net";

            client = httpClient;
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
            const string searchEndpoint = "/Resource/Search";

            var endpointPath = learningHubApiBaseUrl + searchEndpoint;
            var requestUrl = GetUrlWithSearchQueryParams(endpointPath, text, offset, limit, resourceTypes);

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

        public async Task<BulkResourceReferences> GetBulkResourcesByReferenceIds(
            IEnumerable<int>? resourceReferenceIds = null
        )
        {
            const string bulkEndpoint = "/Resource/Bulk";
            var endpointPath = learningHubApiBaseUrl + bulkEndpoint;
            var requestUrl = GetUrlWithResourceReferenceIdQueryParams(endpointPath, resourceReferenceIds);

            var response = await client.GetStringAsync(requestUrl);
            var result = JsonConvert.DeserializeObject<BulkResourceReferences>(response);
            return result;
        }

        private static string GetUrlWithSearchQueryParams(
            string endpointPath,
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

            var requestUrl =
                $"{endpointPath}?{textQueryString}{offSetQueryString}{limitQueryString}{resourceTypesQueryString}";

            requestUrl = GetUrlWithoutLastCharacter(requestUrl);

            return requestUrl;
        }

        private static string GetUrlWithResourceReferenceIdQueryParams(
            string endpointPath,
            IEnumerable<int>? resourceIds = null
        )
        {
            if (resourceIds == null)
            {
                return endpointPath;
            }

            var requestUrl = endpointPath + "?";

            foreach (var resourceId in resourceIds)
            {
                requestUrl = requestUrl + "resourceReferenceIds=" + resourceId + "&";
            }

            requestUrl = GetUrlWithoutLastCharacter(requestUrl);

            return requestUrl;
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

        private static string GetUrlWithoutLastCharacter(string url)
        {
            return url.Remove(url.Length - 1, 1);
        }
    }
}
