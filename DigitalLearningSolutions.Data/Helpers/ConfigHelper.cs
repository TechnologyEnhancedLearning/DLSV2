namespace DigitalLearningSolutions.Data.Helpers
{
    using Microsoft.Extensions.Configuration;

    public static class ConfigHelper
    {
        public const string AppRootPathName = "AppRootPath";
        public const string CurrentSystemBaseUrlName = "CurrentSystemBaseUrl";
        private const string LearningHubOpenApiKey = "LearningHubOpenAPIKey";
        private const string LearningHubOpenApiBaseUrl = "LearningHubOpenAPIBaseUrl";

        private const string LearningHubSsoSectionKey = "LearningHubSSO";
        private const string LearningHubSsoToleranceKey = "ToleranceInSeconds";
        private const string LearningHubSsoIterationsKey = "HashIterations";
        private const string LearningHubSsoByteLengthKey = "ByteLength";

        public static string GetAppRootPath(this IConfiguration config)
        {
            return config[AppRootPathName];
        }

        public static string GetCurrentSystemBaseUrl(this IConfiguration config)
        {
            return config[CurrentSystemBaseUrlName];
        }

        public static string GetLearningHubOpenApiKey(this IConfiguration config)
        {
            return config[LearningHubOpenApiKey];
        }

        public static string GetLearningHubOpenApiBaseUrl(this IConfiguration config)
        {
            return config[LearningHubOpenApiBaseUrl];
        }

        public static int GetLearningHubSsoHashTolerance(this IConfiguration config)
        {
            return int.Parse(config[string.Join(':', LearningHubSsoSectionKey, LearningHubSsoToleranceKey)]);
        }

        public static int GetLearningHubSsoHashIterations(this IConfiguration config)
        {
            return int.Parse(config[string.Join(':', LearningHubSsoSectionKey, LearningHubSsoIterationsKey)]);
        }

        public static int GetLearningHubSsoByteLength(this IConfiguration config)
        {
            return int.Parse(config[string.Join(':', LearningHubSsoSectionKey, LearningHubSsoByteLengthKey)]);
        }
    }
}
