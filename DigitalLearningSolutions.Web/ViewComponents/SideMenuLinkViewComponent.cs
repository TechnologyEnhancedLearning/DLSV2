﻿namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc;

    public class SideMenuLinkViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string aspController,
            string aspAction,
            string href,
            string linkText,
            bool isCurrentPage,
            Dictionary<string, string> aspAllRouteData
        )
        {
            var model = string.IsNullOrWhiteSpace(href)
                ? new SideMenuLinkViewModel(aspController, aspAction, linkText, isCurrentPage, aspAllRouteData)
                : new SideMenuLinkViewModel(href, linkText, isCurrentPage, aspAllRouteData);

            return View(model);
        }
    }
}
