namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.ApiClients;

    public interface ILearningHubApiService
    {
        (string resourceName, string resourceLink) GetResourceNameAndLink(int learningHubResourceId);
    }

    public class LearningHubApiService : ILearningHubApiService
    {
        private readonly ILearningHubApiClient learningHubApiClient;

        public LearningHubApiService(ILearningHubApiClient learningHubApiClient)
        {
            this.learningHubApiClient = learningHubApiClient;
        }

        public (string resourceName, string resourceLink) GetResourceNameAndLink(int learningHubResourceId)
        {
            // TODO HEEDLS-652 Use the API client to get these values properly
            return ("Resource", "www.test.com");
        }
    }
}
