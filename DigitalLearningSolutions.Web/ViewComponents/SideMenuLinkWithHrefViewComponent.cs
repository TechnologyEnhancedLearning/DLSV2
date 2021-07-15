namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class SideMenuLinkWithHrefViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string href,
            string linkText,
            bool isCurrentPage,
            Dictionary<string, string> aspAllRouteData
        )
        {
            var model = new SideMenuLinkWithHrefViewModel(href, linkText, isCurrentPage, aspAllRouteData);

            return View(model);
        }
    }
}
