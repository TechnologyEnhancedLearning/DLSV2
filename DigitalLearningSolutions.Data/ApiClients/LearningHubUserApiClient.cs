using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DigitalLearningSolutions.Data.Extensions;
using elfhHub.Nhs.Models.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DigitalLearningSolutions.Data.ApiClients
{
    public interface ILearningHubUserApiClient
    {
        Task<bool> hasMultipleUsersForEmailAsync(string emailAddress);
        Task<bool> forgotPasswordAsync(string emailAddress);
    }

    public class LearningHubUserApiClient: ILearningHubUserApiClient
    {
        private readonly HttpClient client;
        private readonly ILogger<ILearningHubApiClient> logger;

        public LearningHubUserApiClient(HttpClient httpClient, ILogger<ILearningHubApiClient> logger, IConfiguration config)
        {
            string learningHubUserApiUrl = config.GetLearningHubUserApiUrl();

            client = httpClient;
            client.BaseAddress = new Uri(learningHubUserApiUrl);

            this.logger = logger;
        }
        public async Task<bool> hasMultipleUsersForEmailAsync(string emailAddress)
        {
            var request = $"ElfhUser/HasMultipleUsersForEmail/{emailAddress}";
            var response = await this.client.GetAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result =  await response.Content.ReadAsStringAsync();
                return bool.Parse(result);
            }

            return false;
        }

        public async Task<bool> forgotPasswordAsync(string emailAddress)
        {
            var json = JsonConvert.SerializeObject(
                new ForgotPasswordViewModel { EmailAddress = emailAddress });
            var stringContent = new StringContent(
                json,
                UnicodeEncoding.UTF8,
                "application/json");

            var request = $"ElfhUser/ForgotPassword";

            var response = await this.client.PostAsync(
                request,
                stringContent).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }
    }
}
