namespace DigitalLearningSolutions.Web.ViewModels.Support
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class SupportViewModel
    {
        protected readonly string CurrentSystemBaseUrl;

        public SupportViewModel(
            DlsSubApplication dlsSubApplication,
            SupportPage currentPage,
            string currentSystemBaseUrl
        )
        {
            CurrentPage = currentPage;
            this.CurrentSystemBaseUrl = currentSystemBaseUrl;
            DlsSubApplication = dlsSubApplication;
        }

        public SupportPage CurrentPage { get; set; }
        public DlsSubApplication DlsSubApplication { get; set; }

        public string HelpUrl => $"{CurrentSystemBaseUrl}/help/Introduction.html";
        public string FaqUrl => $"{CurrentSystemBaseUrl}/tracking/faqs";
        public string ResourcesUrl => $"{CurrentSystemBaseUrl}/tracking/resources";
        public string SupportTicketsUrl => $"{CurrentSystemBaseUrl}/tracking/tickets";

        public string ChangeRequestsUrl =>
            "https://github.com/TechnologyEnhancedLearning/DLSV2/projects/1?fullscreen=true";
    }
}
