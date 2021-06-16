namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;

    public class SideMenuLinkViewModel
    {
        public readonly string? AspAction;

        public readonly string? AspController;

        public readonly string? HRef;

        public readonly string LinkText;

        public readonly bool IsCurrentPage;

        public readonly Dictionary<string, string>? AspAllRouteData;

        public SideMenuLinkViewModel(string aspController, string aspAction, string linkText, bool isCurrentPage,  Dictionary<string, string>? aspAllRouteData)
        {
            AspAction = aspAction;
            AspController = aspController;
            LinkText = linkText;
            IsCurrentPage = isCurrentPage;
            AspAllRouteData = aspAllRouteData;
        }

        public SideMenuLinkViewModel(string hRef, string linkText, bool isCurrentPage,  Dictionary<string, string>? aspAllRouteData)
        {
            HRef = hRef;
            LinkText = linkText;
            IsCurrentPage = isCurrentPage;
            AspAllRouteData = aspAllRouteData;
        }
    }
}
