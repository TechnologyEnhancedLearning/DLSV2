namespace DigitalLearningSolutions.Web.Extensions
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

        public static string IsSelectedTab(
            this IHtmlHelper htmlHelper,
            Tab currentTab,
            string selectedCssClass = "selected"
        )
        {
            if (!(htmlHelper.ViewContext.ViewData["SelectedTab"] is Tab selectedTab))
            {
                return "";
            }

            var c = currentTab;
            var s = selectedTab;

            return selectedTab.Equals(currentTab)
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
