namespace DigitalLearningSolutions.Web.ViewModels.FindYourCentre
{
    using DigitalLearningSolutions.Web.Helpers;

    public class FindYourCentreViewModel
    {
        public string Url { get; set; }

        public FindYourCentreViewModel()
        {
            Url = ConfigHelper.GetAppConfig()["CurrentSystemBaseUrl"] + "/findyourcentre?nonav=true";
        }

        public FindYourCentreViewModel(string centreId)
        {
            Url = ConfigHelper.GetAppConfig()["CurrentSystemBaseUrl"] + "/findyourcentre?nonav=true&centreid=" + centreId;
        }
    }
}
