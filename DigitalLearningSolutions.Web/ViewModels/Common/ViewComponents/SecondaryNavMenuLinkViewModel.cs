namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;

    public class SecondaryNavMenuLinkViewModel : LinkViewModel
    {
        public readonly bool IsCurrentPage;

        public SecondaryNavMenuLinkViewModel(
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
