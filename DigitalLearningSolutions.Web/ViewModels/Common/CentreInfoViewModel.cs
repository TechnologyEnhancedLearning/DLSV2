namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;

    public class CentreInfoViewModel
    {
        public CentreInfoViewModel(int centreId, string bannerText)
        {
            Routes = new Dictionary<string, string>
            {
                { "centreId", centreId.ToString() }
            };
            BannerText = bannerText;
        }

        public Dictionary<string, string> Routes { get; set; }
        public string BannerText { get; set; }
    }
}
