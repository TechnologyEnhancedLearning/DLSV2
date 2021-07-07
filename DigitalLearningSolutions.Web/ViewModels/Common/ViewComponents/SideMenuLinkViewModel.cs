namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;

    public class SideMenuLinkViewModel : LinkViewModel
    {
        public readonly bool IsCurrentPage;

        public SideMenuLinkViewModel(
            string aspController,
            string aspAction,
            string linkText,
            bool isCurrentPage,
            Dictionary<string, string>? aspAllRouteData
        ) : base(aspController, aspAction, linkText, aspAllRouteData)
        {
            IsCurrentPage = isCurrentPage;
        }
    }
}
