namespace DigitalLearningSolutions.Data.Extensions
{
    using Microsoft.Extensions.Configuration;

    public static class ConfigurationExtensions
    {
        public const string UseSignposting = "FeatureManagement:UseSignposting";

        private const string AppRootPathName = "AppRootPath";
        private const string CurrentSystemBaseUrlName = "CurrentSystemBaseUrl";
        private const string LearningHubOpenApiKey = "LearningHubOpenAPIKey";
        private const string LearningHubOpenApiBaseUrl = "LearningHubOpenAPIBaseUrl";
        private const string PricingPageEnabled = "FeatureManagement:PricingPage";

        private const string LearningHubAuthBaseUrl = "BaseUrl";
        private const string LearningHubAuthLoginEndpoint = "LoginEndpoint";
        private const string LearningHubAuthLinkingEndpoint = "LinkingEndpoint";
        private const string LearningHubAuthClientCode = "ClientCode";

        private const string MapsApiKey = "MapsAPIKey";
        private const string LearningHubSsoSectionKey = "LearningHubSSO";
        private const string LearningHubSsoToleranceKey = "ToleranceInSeconds";
        private const string LearningHubSsoIterationsKey = "HashIterations";
        private const string LearningHubSsoByteLengthKey = "ByteLength";
        private const string LearningHubSsoSecretKey = "SecretKey";

        private const string JavascriptSearchSortFilterPaginateItemLimitKey =
            "JavascriptSearchSortFilterPaginateItemLimit";

        private const string MonthsToPromptUserDetailsCheckKey = "MonthsToPromptUserDetailsCheck";

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

        public static bool IsPricingPageEnabled(this IConfiguration config)
        {
            return bool.Parse(config[PricingPageEnabled]);
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

        public static string GetMapsApiKey(this IConfiguration config)
        {
            return config[MapsApiKey];
        }

        public static int GetJavascriptSearchSortFilterPaginateItemLimit(this IConfiguration config)
        {
            return int.Parse(config[JavascriptSearchSortFilterPaginateItemLimitKey]);
        }

        public static int GetMonthsToPromptUserDetailsCheck(this IConfiguration config)
        {
            return int.Parse(config[MonthsToPromptUserDetailsCheckKey]);
        }
    }
}
