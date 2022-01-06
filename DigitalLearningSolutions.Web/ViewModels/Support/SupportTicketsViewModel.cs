namespace DigitalLearningSolutions.Web.ViewModels.Support
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class SupportTicketsViewModel : SupportViewModel
    {
        public SupportTicketsViewModel(
            DlsSubApplication dlsSubApplication,
            SupportPage currentPage,
            string currentSystemBaseUrl
        ) : base(dlsSubApplication, currentPage, currentSystemBaseUrl)
        {
            SupportTicketsIframeUrl = $"{currentSystemBaseUrl}/tracking/tickets?nonav=true";
        }

        public string SupportTicketsIframeUrl { get; set; }
    }
}
