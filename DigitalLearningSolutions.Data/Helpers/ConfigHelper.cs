namespace DigitalLearningSolutions.Data.Helpers
{
    using Microsoft.Extensions.Configuration;

    public static class ConfigHelper
    {
        public const string AppRootPathName = "AppRootPath";
        public const string CurrentSystemBaseUrlName = "CurrentSystemBaseUrl";
        private const string LearningHubOpenApiKey = "LearningHubOpenAPIKey";
        private const string LearningHubOpenApiBaseUrl = "LearningHubOpenAPIBaseUrl";
        public const string LearningHubAuthApiClientCode = "LearningHubAuthAPIClientCode";
        public const string LearningHubAuthApiBaseUrl = "LearningHubAuthAPIBaseUrl";
        public const string UseSignposting = "FeatureManagement:UseSignposting";

        private const string LearningHubSsoSectionKey = "LearningHubSSO";
        private const string LearningHubSsoToleranceKey = "ToleranceInSeconds";
        private const string LearningHubSsoIterationsKey = "HashIterations";
        private const string LearningHubSsoByteLengthKey = "ByteLength";
        private const string LearningHubSsoSecretKey = "SecretKey";

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

        public static string GetLearningHubAuthApiBaseUrl(this IConfiguration config)
        {
            return config[LearningHubAuthApiBaseUrl];
        }

        public static string GetLearningHubAuthApiClientCode(this IConfiguration config)
        {
            return config[LearningHubAuthApiClientCode];
        }

        public static bool IsSignpostingUsed(this IConfiguration config)
        {
            return bool.Parse(config[UseSignposting]);
        }

        public static int GetLearningHubSsoHashTolerance(this IConfiguration config)
        {
            return int.Parse(config[$"{LearningHubSsoSectionKey}:{LearningHubSsoToleranceKey}"]);
        }

        public static int GetLearningHubSsoHashIterations(this IConfiguration config)
        {
            return int.Parse(config[$"{LearningHubSsoSectionKey}:{LearningHubSsoIterationsKey}"]);
        }

        public static int GetLearningHubSsoByteLength(this IConfiguration config)
        {
            return int.Parse(config[$"{LearningHubSsoSectionKey}:{LearningHubSsoByteLengthKey}"]);
        }

        public static string GetLearningHubSsoSecretKey(this IConfiguration config)
        {
            return config[$"{LearningHubSsoSectionKey}:{LearningHubSsoSecretKey}"];
        }
    }
}
