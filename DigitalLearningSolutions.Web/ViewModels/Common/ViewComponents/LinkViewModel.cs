namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;

    public class LinkViewModel
    {
        public readonly string AspAction;

        public readonly string AspController;

        public readonly string LinkText;

        public readonly Dictionary<string, string>? AspAllRouteData;

        public LinkViewModel(string aspController, string aspAction, string linkText, Dictionary<string, string>? aspAllRouteData)
        {
            AspAction = aspAction;
            AspController = aspController;
            LinkText = linkText;
            AspAllRouteData = aspAllRouteData;
        }
    }
}
