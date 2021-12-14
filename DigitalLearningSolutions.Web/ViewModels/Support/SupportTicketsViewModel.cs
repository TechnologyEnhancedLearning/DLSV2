namespace DigitalLearningSolutions.Web.ViewModels.Support
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class SupportTicketsViewModel : ISupportViewModel
    {
        public SupportTicketsViewModel(
            DlsSubApplication dlsSubApplication,
            SupportPage currentPage,
            string currentSystemBaseUrl
        )
        {
            CurrentPage = currentPage;
            DlsSubApplication = dlsSubApplication;
            SupportSideNavViewModel = new SupportSideNavViewModel(currentSystemBaseUrl);
            SupportTicketsIframeUrl = $"{currentSystemBaseUrl}/tracking/tickets?nonav=true";
        }

        public string SupportTicketsIframeUrl { get; set; }

        public SupportPage CurrentPage { get; set; }

        public DlsSubApplication DlsSubApplication { get; set; }

        public SupportSideNavViewModel SupportSideNavViewModel { get; set; }
    }
}
