namespace DigitalLearningSolutions.Data.Helpers.ExternalApis
{
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

        Task<ResourceReferenceWithReferenceDetails> GetResourcesBulk(IEnumerable<int> resourceReferenceIds);
    }

    public class LearningHubApiClient : ILearningHubApiClient
    {
        private readonly HttpClient client;
        private readonly string LearningHubApiBaseUrl;

        public LearningHubApiClient(HttpClient httpClient, IConfiguration config)
        {
            var learningHubApiKey = config.GetLearningHubApiKey();
            LHApiBaseUrl =
                "https://uks-learninghubnhsuk-openapi-dev.azurewebsites.net";

            client = httpClient;
        }

        public async Task<ResourceSearchResult> SearchResource(
            string text,
            int? offset = null,
            int? limit = null,
            IEnumerable<string>? resourceTypes = null
        )
        {
            client.DefaultRequestHeaders.Accept.Clear();
            var response = await client.GetStringAsync(mapsApiBaseUrl + postcode);
            var result = JsonConvert.DeserializeObject<ResourceSearchResult>(response);
            return result;
        }

        public async Task<ResourceReferenceWithReferenceDetails> GetResourceByReferenceId(int resourceReferenceId)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            var response = await client.GetStringAsync(mapsApiBaseUrl + postcode);
            var result = JsonConvert.DeserializeObject<ResourceReferenceWithReferenceDetails>(response);
            return result;
        }

        public async Task<ResourceReferenceWithReferenceDetails> GetResourcesBulk(IEnumerable<int> resourceReferenceIds)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            var response = await client.GetStringAsync(mapsApiBaseUrl + postcode);
            var result = JsonConvert.DeserializeObject<ResourceReferenceWithReferenceDetails>(response);
            return result;
        }
    }
}
