namespace DigitalLearningSolutions.Web.Helpers
{
    public class SupportLinksHelper
    {
        public static string GetSupportTicketsIframeUrl(string currentSystemBaseUrl) => $"{currentSystemBaseUrl}/tracking/tickets?nonav=true";
        public static string GetHelpUrl(string currentSystemBaseUrl) => $"{currentSystemBaseUrl}/help/Introduction.html";
        public static string GetChangeRequestsUrl =>
            "https://github.com/TechnologyEnhancedLearning/DLSV2/projects/1?fullscreen=true";
    }
}
