namespace DigitalLearningSolutions.Web.ViewModels.Support
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class SupportViewModel : ISupportViewModel
    {
        public SupportViewModel(
            DlsSubApplication dlsSubApplication,
            SupportPage currentPage,
            string currentSystemBaseUrl
        )
        {
            CurrentPage = currentPage;
            DlsSubApplication = dlsSubApplication;
            SupportSideNavViewModel = new SupportSideNavViewModel(currentSystemBaseUrl);
        }

        public SupportPage CurrentPage { get; set; }
        public DlsSubApplication DlsSubApplication { get; set; }
        public SupportSideNavViewModel SupportSideNavViewModel { get; set; }
    }
}
