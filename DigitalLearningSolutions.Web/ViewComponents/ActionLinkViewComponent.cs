namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class ActionLinkViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string aspController, string aspAction, Dictionary<string, string> aspAllRouteData, string linkText)
        {
            return View(new LinkViewModel(aspController, aspAction, linkText, aspAllRouteData));
        }
    }
}
