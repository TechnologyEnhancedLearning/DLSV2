namespace DigitalLearningSolutions.Web.ViewModels.FindYourCentre
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using Microsoft.Extensions.Configuration;

    public class FindYourCentreViewModel
    {
        public string Url { get; set; }
        public IEnumerable<CentreSummaryForFindCentre> CentreSummaries { get; set; }

        public FindYourCentreViewModel(IConfiguration config, IEnumerable<CentreSummaryForFindCentre> centreSummaries)
        {
            Url = $"{config.GetCurrentSystemBaseUrl()}/findyourcentre?nonav=true";
            CentreSummaries = centreSummaries;
        }

        /*public FindYourCentreViewModel(string centreId, IConfiguration config)
        {
            Url = $"{config.GetCurrentSystemBaseUrl()}/findyourcentre?nonav=true&centreid={centreId}";
        }*/

    }
}
