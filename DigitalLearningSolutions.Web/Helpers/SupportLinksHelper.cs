namespace DigitalLearningSolutions.Web.Helpers
{
    public class SupportLinksHelper
    {
        public static string GenerateSupportTicketsIframeUrl(string currentSystemBaseUrl) => $"{currentSystemBaseUrl}/tracking/tickets?nonav=true";
        public static string GenerateHelpUrl(string currentSystemBaseUrl) => $"{currentSystemBaseUrl}/help/Introduction.html";
        public static string ChangeRequestsUrl =>
            "https://github.com/TechnologyEnhancedLearning/DLSV2/projects/1?fullscreen=true";
    }
}
