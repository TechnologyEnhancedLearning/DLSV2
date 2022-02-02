﻿namespace DigitalLearningSolutions.Data.Helpers
{
    using Microsoft.Extensions.Configuration;

    public static class ConfigHelper
    {
        public const string AppRootPathName = "AppRootPath";
        public const string CurrentSystemBaseUrlName = "CurrentSystemBaseUrl";
        private const string LearningHubOpenApiKey = "LearningHubOpenAPIKey";
        private const string LearningHubOpenApiBaseUrl = "LearningHubOpenAPIBaseUrl";
        public const string UseSignposting = "FeatureManagement:UseSignposting";

        public const string LearningHubAuthBaseUrl = "BaseUrl";
        public const string LearningHubAuthLoginEndpoint = "LoginEndpoint";
        public const string LearningHubAuthLinkingEndpoint = "LinkingEndpoint";
        public const string LearningHubAuthClientCode = "ClientCode";

        public const string LearningHubSsoSectionKey = "LearningHubSSO";
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
            return config[$"{LearningHubSsoSectionKey}:{LearningHubAuthBaseUrl}"];
        }

        public static string GetLearningHubAuthApiClientCode(this IConfiguration config)
        {
            return config[$"{LearningHubSsoSectionKey}:{LearningHubAuthClientCode}"];
        }

        public static string GetLearningHubAuthApiLoginEndpoint(this IConfiguration config)
        {
            return config[$"{LearningHubSsoSectionKey}:{LearningHubAuthLoginEndpoint}"];
        }

        public static string GetLearningHubAuthApiLinkingEndpoint(this IConfiguration config)
        {
            return config[$"{LearningHubSsoSectionKey}:{LearningHubAuthLinkingEndpoint}"];
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
