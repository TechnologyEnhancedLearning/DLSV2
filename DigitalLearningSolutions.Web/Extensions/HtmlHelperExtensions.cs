﻿namespace DigitalLearningSolutions.Web.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public static class HtmlHelperExtensions
    {
        public static string IsSelected(
            this IHtmlHelper htmlHelper,
            string action,
            string selectedCssClass = "selected",
            string? controller = null
        )
        {
            var currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;
            var currentAction = htmlHelper.ViewContext.RouteData.Values["action"] as string;

            return (controller == null || controller == currentController) && action == currentAction
                ? selectedCssClass
                : string.Empty;
        }

        public static string GetSelectedCssClassIfTabSelected(
            this IHtmlHelper htmlHelper,
            NavMenuTab currentNavMenuTab,
            string selectedCssClass = "selected"
        )
        {
            if (!(htmlHelper.ViewContext.ViewData["SelectedTab"] is NavMenuTab selectedTab))
            {
                return "";
            }

            return selectedTab.Equals(currentNavMenuTab)
                ? selectedCssClass
                : string.Empty;
        }

        public static Dictionary<string, string?> GetRouteValues(
            this IHtmlHelper htmlHelper
        )
        {
            var routeValues = htmlHelper.ViewContext.HttpContext.Request.RouteValues;
            return routeValues.ToDictionary(
                kvp => kvp.Key,
                kvp => Convert.ToString(kvp.Value, CultureInfo.InvariantCulture)
            );
        }
    }
}
