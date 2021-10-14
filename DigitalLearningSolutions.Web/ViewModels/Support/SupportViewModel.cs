namespace DigitalLearningSolutions.Web.ViewModels.Support
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class SupportViewModel
    {
        private readonly string currentSystemBaseUrl;

        public SupportViewModel(DlsSubApplication dlsSubApplication, SupportPage currentPage, string currentSystemBaseUrl)
        {
            CurrentPage = currentPage;
            this.currentSystemBaseUrl = currentSystemBaseUrl;
            DlsSubApplication = dlsSubApplication;
        }

        public SupportPage CurrentPage { get; set; }
        public DlsSubApplication DlsSubApplication { get; set; }

        public string HelpUrl => $"{currentSystemBaseUrl}/help/Introduction.html";
        public string FaqUrl => $"{currentSystemBaseUrl}/tracking/faqs";
        public string ResourcesUrl => $"{currentSystemBaseUrl}/tracking/resources";
        public string SupportTicketsUrl => $"{currentSystemBaseUrl}/tracking/tickets";
        public string ChangeRequestsUrl =>
            "https://github.com/TechnologyEnhancedLearning/DLSV2/projects/1?fullscreen=true";
    }
}
