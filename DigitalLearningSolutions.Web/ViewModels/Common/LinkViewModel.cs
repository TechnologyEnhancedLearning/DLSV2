namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;

    public class LinkViewModel
    {
        public readonly string AspAction;

        public readonly string AspController;

        public readonly Dictionary<string, string>? AspAllRouteData;

        public LinkViewModel(string aspController, string aspAction, Dictionary<string, string>? aspAllRouteData)
        {
            AspAction = aspAction;
            AspController = aspController;
            AspAllRouteData = aspAllRouteData;
        }
    }
}
