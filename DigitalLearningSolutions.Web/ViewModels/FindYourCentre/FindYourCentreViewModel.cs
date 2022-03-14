namespace DigitalLearningSolutions.Web.ViewModels.FindYourCentre
{
    using DigitalLearningSolutions.Data.Extensions;
    using Microsoft.Extensions.Configuration;

    public class FindYourCentreViewModel
    {
        public string Url { get; set; }

        public FindYourCentreViewModel(IConfiguration config)
        {
            Url = $"{config.GetCurrentSystemBaseUrl()}/findyourcentre?nonav=true";
        }

        public FindYourCentreViewModel(string centreId, IConfiguration config)
        {
            Url = $"{config.GetCurrentSystemBaseUrl()}/findyourcentre?nonav=true&centreid={centreId}";
        }
    }
}
