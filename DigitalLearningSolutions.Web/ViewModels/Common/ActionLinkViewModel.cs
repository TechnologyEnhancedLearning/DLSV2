namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;

    public class ActionLinkViewModel : LinkViewModel
    {
        public readonly string LinkText;

        public ActionLinkViewModel(
                string aspController,
                string aspAction,
                Dictionary<string, string>? aspAllRouteData,
                string linkText)
            : base(aspController, aspAction, aspAllRouteData)
        {
            LinkText = linkText;
        }
    }
}
