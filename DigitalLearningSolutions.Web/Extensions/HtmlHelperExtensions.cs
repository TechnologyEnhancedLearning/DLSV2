namespace DigitalLearningSolutions.Web.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public static class HtmlHelperExtensions
    {
        private const string SelectedCssClass = "selected";

        public static string IsSelected(
            this IHtmlHelper htmlHelper,
            string action,
            string? controller = null
        )
        {
            var currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;
            var currentAction = htmlHelper.ViewContext.RouteData.Values["action"] as string;

            return (controller == null || controller == currentController) && action == currentAction
                ? SelectedCssClass
                : string.Empty;
        }

        public static string GetSelectedCssClassIfTabSelected(
            this IHtmlHelper htmlHelper,
            NavMenuTab currentNavMenuTab
        )
        {
            if (!(htmlHelper.ViewContext.ViewData[LayoutViewDataKeys.SelectedTab] is NavMenuTab selectedTab))
            {
                return "";
            }

            return selectedTab.Equals(currentNavMenuTab)
                ? SelectedCssClass
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

        public static bool AnyFieldIsRequired<TModel>(this IHtmlHelper helper, TModel model)
        {
            return typeof(TModel)
                    .GetProperties()
                    .Any(p => Attribute.IsDefined(p, typeof(RequiredAttribute)));
        }
    }
}
