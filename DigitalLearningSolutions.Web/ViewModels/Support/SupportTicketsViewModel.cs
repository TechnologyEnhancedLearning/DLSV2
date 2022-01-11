namespace DigitalLearningSolutions.Web.ViewModels.Support
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class SupportTicketsViewModel : BaseSupportViewModel
    {
        public SupportTicketsViewModel(
            DlsSubApplication dlsSubApplication,
            SupportPage currentPage,
            string currentSystemBaseUrl
        ) : base(dlsSubApplication, currentPage, currentSystemBaseUrl) { }
    }
}
