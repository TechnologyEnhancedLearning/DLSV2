namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;

    public class SideMenuLinkWithHrefViewModel
    {
        public readonly string? Href;

        public readonly string LinkText;

        public readonly bool IsCurrentPage;

        public readonly Dictionary<string, string>? AspAllRouteData;

        public SideMenuLinkWithHrefViewModel(string hRef, string linkText, bool isCurrentPage,  Dictionary<string, string>? aspAllRouteData)
        {
            Href = hRef;
            LinkText = linkText;
            IsCurrentPage = isCurrentPage;
            AspAllRouteData = aspAllRouteData;
        }
    }
}
