namespace DigitalLearningSolutions.Data.Extensions
{
    using Microsoft.Extensions.Configuration;

    public static class ConfigurationExtensions
    {
        private const string UseSignposting = "FeatureManagement:UseSignposting";

        private const string AppRootPathName = "AppRootPath";
        private const string CurrentSystemBaseUrlName = "CurrentSystemBaseUrl";
        private const string LearningHubOpenApiKey = "LearningHubOpenAPIKey";
        private const string LearningHubOpenApiBaseUrl = "LearningHubOpenAPIBaseUrl";
        private const string PricingPageEnabled = "FeatureManagement:PricingPage";

        private const string LearningHubAuthBaseUrl = "BaseUrl";
        private const string LearningHubAuthLoginEndpoint = "LoginEndpoint";
        private const string LearningHubAuthLinkingEndpoint = "LinkingEndpoint";
        private const string LearningHubAuthClientCode = "ClientCode";
        private const string LearningHubAuthSsoClientCode = "ClientCodeSso";

        private const string MapsApiKey = "MapsAPIKey";
        private const string LearningHubSsoSectionKey = "LearningHubSSO";
        private const string LearningHubSsoToleranceKey = "ToleranceInSeconds";
        private const string LearningHubSsoIterationsKey = "HashIterations";
        private const string LearningHubSsoByteLengthKey = "ByteLength";
        private const string LearningHubSsoSecretKey = "SecretKey";

        private const string CookieBannerConsentCookieName = "CookieBannerConsent:CookieName";
        private const string CookieBannerConsentExpiryDays = "CookieBannerConsent:ExpiryDays";

        private const string JavascriptSearchSortFilterPaginateItemLimitKey =
            "JavascriptSearchSortFilterPaginateItemLimit";

        private const string ExcelPassword = "ExcelPassword";

        private const string MonthsToPromptUserDetailsCheckKey = "MonthsToPromptUserDetailsCheck";
        private const string LearningHubReportAPIBaseUrl = "LearningHubReportAPIConfig:BaseUrl";
        private const string LearningHubReportAPIClientId = "LearningHubReportAPIConfig:ClientId";
        private const string LearningHubReportAPIClientIdentityKey = "LearningHubReportAPIConfig:ClientIdentityKey";
        private const string ExportQueryRowLimitKey = "FeatureManagement:ExportQueryRowLimit";
        private const string MaxBulkUploadRowsLimitKey = "FeatureManagement:MaxBulkUploadRows";

        private const string FreshdeskCreateTicketGroupId = "FreshdeskAPIConfig:GroupId";
        private const string FreshdeskCreateTicketProductId = "FreshdeskAPIConfig:ProductId";

        private const string LearningHubAuthenticationAuthority = "LearningHubAuthentication:Authority";
        private const string LearningHubAuthenticationClientId = "learningHubAuthentication:ClientId";
        private const string LearningHubAuthenticationClientSecret = "LearningHubAuthentication:ClientSecret";

        private const string LearningHubUserAPIUserAPIUrl = "LearningHubUserApi:UserApiUrl";
        private const string UserResearchUrlName = "UserResearchUrl";
        private const string TableauSectionKey = "TableauDashboards";
        private const string TableauClientId = "ClientId";
        private const string TableauClientSecret = "ClientSecret";
        private const string TableauUsername = "Username";
        private const string TableauClientName = "ClientName";

        public static string GetAppRootPath(this IConfiguration config)
        {
            return config[AppRootPathName]!;
        }

        public static string GetCurrentSystemBaseUrl(this IConfiguration config)
        {
            return config[CurrentSystemBaseUrlName]!;
        }

        public static string GetLearningHubOpenApiKey(this IConfiguration config)
        {
            return config[LearningHubOpenApiKey]!;
        }

        public static string GetLearningHubOpenApiBaseUrl(this IConfiguration config)
        {
            return config[LearningHubOpenApiBaseUrl]!;
        }

        public static string GetLearningHubAuthApiBaseUrl(this IConfiguration config)
        {
            return config[$"{LearningHubSsoSectionKey}:{LearningHubAuthBaseUrl}"]!;
        }

        public static string GetLearningHubAuthApiClientCode(this IConfiguration config)
        {
            return config[$"{LearningHubSsoSectionKey}:{LearningHubAuthClientCode}"]!;
        }

        public static string GetLearningHubAuthApiSsoClientCode(this IConfiguration config)
        {
            return config[$"{LearningHubSsoSectionKey}:{LearningHubAuthSsoClientCode}"]!;
        }

        public static string GetLearningHubAuthApiLoginEndpoint(this IConfiguration config)
        {
            return config[$"{LearningHubSsoSectionKey}:{LearningHubAuthLoginEndpoint}"]!;
        }

        public static string GetLearningHubAuthApiLinkingEndpoint(this IConfiguration config)
        {
            return config[$"{LearningHubSsoSectionKey}:{LearningHubAuthLinkingEndpoint}"]!;
        }

        public static bool IsSignpostingUsed(this IConfiguration config)
        {
            bool.TryParse(config[UseSignposting], out bool isEnabled);
            return isEnabled;
        }

        public static bool IsPricingPageEnabled(this IConfiguration config)
        {
            bool.TryParse(config[PricingPageEnabled], out bool isEnabled);
            return isEnabled;
        }

        public static int GetLearningHubSsoHashTolerance(this IConfiguration config)
        {
            int.TryParse(config[$"{LearningHubSsoSectionKey}:{LearningHubSsoToleranceKey}"], out int ssoHashTolerance);
            return ssoHashTolerance;
        }

        public static int GetLearningHubSsoHashIterations(this IConfiguration config)
        {
            int.TryParse(config[$"{LearningHubSsoSectionKey}:{LearningHubSsoIterationsKey}"], out int ssoIterationsKey);
            return ssoIterationsKey;
        }

        public static int GetLearningHubSsoByteLength(this IConfiguration config)
        {
            int.TryParse(config[$"{LearningHubSsoSectionKey}:{LearningHubSsoByteLengthKey}"], out int ssoByteLength);
            return ssoByteLength;
        }

        public static string GetLearningHubSsoSecretKey(this IConfiguration config)
        {
            return config[$"{LearningHubSsoSectionKey}:{LearningHubSsoSecretKey}"]!;
        }

        public static string GetMapsApiKey(this IConfiguration config)
        {
            return config[MapsApiKey]!;
        }

        public static int GetJavascriptSearchSortFilterPaginateItemLimit(this IConfiguration config)
        {
            int.TryParse(config[JavascriptSearchSortFilterPaginateItemLimitKey], out int filterPaginateItemLimitKey);
            return filterPaginateItemLimitKey;
        }

        public static int GetMonthsToPromptUserDetailsCheck(this IConfiguration config)
        {
            int.TryParse(config[MonthsToPromptUserDetailsCheckKey], out int userDetailsCheckKey);
            return userDetailsCheckKey;
        }

        public static string GetExcelPassword(this IConfiguration config)
        {
            return config[ExcelPassword]!;
        }
        public static string GetLearningHubReportApiBaseUrl(this IConfiguration config)
        {
            return config[LearningHubReportAPIBaseUrl]!;
        }
        public static string GetLearningHubReportApiClientId(this IConfiguration config)
        {
            return config[LearningHubReportAPIClientId]!;
        }
        public static string GetLearningHubReportApiClientIdentityKey(this IConfiguration config)
        {
            return config[LearningHubReportAPIClientIdentityKey]!;
        }
        public static string GetCookieBannerConsentCookieName(this IConfiguration config)
        {
            return config[CookieBannerConsentCookieName]!;
        }

        public static int GetCookieBannerConsentExpiryDays(this IConfiguration config)
        {
            int.TryParse(config[CookieBannerConsentExpiryDays], out int expiryDays);
            return expiryDays;
        }
        public static int GetExportQueryRowLimit(this IConfiguration config)
        {
            int.TryParse(config[ExportQueryRowLimitKey], out int limitKey);
            return limitKey;
        }
        public static int GetMaxBulkUploadRowsLimit(this IConfiguration config)
        {
             int.TryParse(config[MaxBulkUploadRowsLimitKey],out int limitKey);
            return limitKey;
        }

        public static string GetLearningHubAuthenticationAuthority(this IConfiguration config)
        {
            return config[LearningHubAuthenticationAuthority]!;
        }

        public static string GetLearningHubAuthenticationClientId(this IConfiguration config)
        {
            return config[LearningHubAuthenticationClientId]!;
        }

        public static string GetLearningHubAuthenticationClientSecret(this IConfiguration config)
        {
            return config[LearningHubAuthenticationClientSecret]!;
        }

        public static long GetFreshdeskCreateTicketGroupId(this IConfiguration config)
        {
           long.TryParse(config[FreshdeskCreateTicketGroupId], out long ticketGroupId);
            return ticketGroupId;
        }
        public static long GetFreshdeskCreateTicketProductId(this IConfiguration config)
        {
            long.TryParse(config[FreshdeskCreateTicketProductId], out long ticketProductId);
            return ticketProductId;
        }

        public static string GetLearningHubUserApiUrl(this IConfiguration config)
        {
            return config[LearningHubUserAPIUserAPIUrl]!;
        }
        public static string GetUserResearchUrl(this IConfiguration config)
        {
            return config[UserResearchUrlName]!;
        }
        public static string GetTableauClientName(this IConfiguration config)
        {
            return config[$"{TableauSectionKey}:{TableauClientName}"]!;
        }
        public static string GetTableauClientId(this IConfiguration config)
        {
            return config[$"{TableauSectionKey}:{TableauClientId}"]!;
        }
        public static string GetTableauClientSecret(this IConfiguration config)
        {
            return config[$"{TableauSectionKey}:{TableauClientSecret}"]!;
        }
        public static string GetTableauUser(this IConfiguration config)
        {
            return config[$"{TableauSectionKey}:{TableauUsername}"]!;
        }
    }
}
