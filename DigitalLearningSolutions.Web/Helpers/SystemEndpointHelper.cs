namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Extensions;
    using Microsoft.Extensions.Configuration;

    public static class SystemEndpointHelper
    {
        public static string GetEvaluateUrl(IConfiguration config, int progressId)
        {
            return $"{config.GetCurrentSystemBaseUrl()}/tracking/finalise?ProgressID={progressId}";
        }

        public static string GetTrackingUrl(IConfiguration config)
        {
            return $"{config.GetCurrentSystemBaseUrl()}/tracking/tracker";
        }

        public static string GetScormPlayerUrl(IConfiguration config)
        {
            return $"{config.GetCurrentSystemBaseUrl()}/scoplayer/sco";
        }

        public static string GetDownloadSummaryUrl(IConfiguration config, int progressId)
        {
            return $"{config.GetCurrentSystemBaseUrl()}/tracking/summary?ProgressID={progressId}";
        }

        public static string GetConsolidationPathUrl(IConfiguration config, string consolidationPath)
        {
            return $"{config.GetCurrentSystemBaseUrl()}/tracking/dlconsolidation?client={consolidationPath}";
        }
    }
}
