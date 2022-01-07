namespace DigitalLearningSolutions.Data.ApiClients
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Helpers;
    using Microsoft.Extensions.Configuration;

    public interface ILearningHubAuthClient
    {
        Task<int> CreateLearningHubUser();
    }

    public class LearningHubAuthClient : ILearningHubAuthClient
    {
        private readonly HttpClient client;

        public LearningHubAuthClient(HttpClient httpClient, IConfiguration config)
        {
            string learningHubAuthKey = config.GetLearningHubAuthApiKey(); // todo do I need this?
            string learningHubAuthBaseUrl = config.GetLearningHubAuthApiBaseUrl();

            client = httpClient;
            client.BaseAddress = new Uri(learningHubAuthBaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("X-API-KEY", learningHubAuthKey); // todo is this right?
        }

        public Task<int> CreateLearningHubUser()
        {
            throw new System.NotImplementedException();
        }
    }
}
