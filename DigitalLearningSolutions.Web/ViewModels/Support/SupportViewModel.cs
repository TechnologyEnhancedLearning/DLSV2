namespace DigitalLearningSolutions.Web.ViewModels.Support
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class SupportViewModel : BaseSupportViewModel
    {
        public SupportViewModel(
            DlsSubApplication dlsSubApplication,
            SupportPage currentPage,
            string currentSystemBaseUrl
        ) : base(dlsSubApplication, currentPage, currentSystemBaseUrl) { }
    }
}
