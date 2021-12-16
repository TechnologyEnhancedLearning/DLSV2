namespace DigitalLearningSolutions.Data.Services
{
    public interface ILearningHubApiService
    {
        (string resourceName, string resourceLink) GetResourceNameAndLink(int learningHubResourceId);
    }

    public class LearningHubApiService : ILearningHubApiService
    {
        public (string resourceName, string resourceLink) GetResourceNameAndLink(int learningHubResourceId)
        {
            // TODO HEEDLS-652 Use the API client to get these values properly
            return ("Resource", "www.test.com");
        }
    }
}
