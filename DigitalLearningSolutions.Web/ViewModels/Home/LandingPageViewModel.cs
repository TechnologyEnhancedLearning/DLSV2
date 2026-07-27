namespace DigitalLearningSolutions.Web.ViewModels.Home
{
    using DigitalLearningSolutions.Web.ViewModels.Common.MiniHub;

    public class LandingPageViewModel
    {
        public MiniHubNavigationModel MiniHubNavigationModel { get; set; }
        public bool UserIsLoggedIn { get; set; }
        public bool UserIsLoggedInCentre { get; set; }
        public string CurrentSiteBaseUrl { get; set; }
    }
}
