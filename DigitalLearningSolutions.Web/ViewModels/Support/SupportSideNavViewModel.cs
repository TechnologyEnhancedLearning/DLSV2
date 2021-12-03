namespace DigitalLearningSolutions.Web.ViewModels.Support
{
    public class SupportSideNavViewModel
    {
        private readonly string currentSystemBaseUrl;

        public SupportSideNavViewModel(string currentSystemBaseUrl)
        {
            this.currentSystemBaseUrl = currentSystemBaseUrl;
        }

        public string HelpUrl => $"{currentSystemBaseUrl}/help/Introduction.html";
        public string SupportTicketsUrl => $"{currentSystemBaseUrl}/tracking/tickets";
        public string ChangeRequestsUrl =>
            "https://github.com/TechnologyEnhancedLearning/DLSV2/projects/1?fullscreen=true";
    }
}
