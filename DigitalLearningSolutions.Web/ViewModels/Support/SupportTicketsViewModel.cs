namespace DigitalLearningSolutions.Web.ViewModels.Support
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class SupportTicketsViewModel : SupportViewModel
    {
        public SupportTicketsViewModel(
            DlsSubApplication dlsSubApplication,
            SupportPage currentPage,
            string currentSystemBaseUrl
        ) : base(dlsSubApplication, currentPage, currentSystemBaseUrl) { }

        public string IframeUrl => $"{CurrentSystemBaseUrl}/tracking/tickets?nonav=true";
    }
}
