namespace DigitalLearningSolutions.Web.ViewModels.Support
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class SupportTicketsViewModel : SupportViewModel
    {
        public SupportTicketsViewModel(DlsSubApplication dlsSubApplication, SupportPage currentPage, string currentSystemBaseUrl) : base(dlsSubApplication, currentPage, currentSystemBaseUrl)
        {
        }

        public string IframeUrl => $"{currentSystemBaseUrl}/tracking/tickets?nonav=true";
    }
}
