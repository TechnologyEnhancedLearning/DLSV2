namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class SideMenuLinkViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string aspController,
            string aspAction,
            string linkText,
            bool isCurrentPage,
            Dictionary<string, string> aspAllRouteData
        )
        {
            var model = new SecondaryNavMenuLinkViewModel(aspController, aspAction, linkText, isCurrentPage, aspAllRouteData);

            return View(model);
        }
    }
}
