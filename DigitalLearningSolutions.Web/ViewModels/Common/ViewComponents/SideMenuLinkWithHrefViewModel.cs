namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;

    public class SideMenuLinkWithHrefViewModel
    {
        public readonly Dictionary<string, string>? AspAllRouteData;
        public readonly string? Href;
        public readonly bool IsCurrentPage;
        public readonly string LinkText;

        public SideMenuLinkWithHrefViewModel(
            string hRef,
            string linkText,
            bool isCurrentPage,
            Dictionary<string, string>? aspAllRouteData
        )
        {
            Href = hRef;
            LinkText = linkText;
            IsCurrentPage = isCurrentPage;
            AspAllRouteData = aspAllRouteData;
        }
    }
}
