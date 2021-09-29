namespace DigitalLearningSolutions.Web.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public static class HtmlHelperExtensions
    {
        public static string IsActionSelected(
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

        public static string IsOneOfActionsSelected(
            this IHtmlHelper htmlHelper,
            IEnumerable<string> actions,
            string selectedCssClass = "selected",
            string? controller = null
        )
        {
            var currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;
            var currentAction = htmlHelper.ViewContext.RouteData.Values["action"] as string;

            return (controller == null || controller == currentController) && actions.Contains(currentAction)
                ? selectedCssClass
                : string.Empty;
        }

        public static string IsControllerSelected(
            this IHtmlHelper htmlHelper,
            string? controller,
            string selectedCssClass = "selected"
        )
        {
            var currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;

            return (controller == currentController)
                ? selectedCssClass
                : string.Empty;
        }

        public static string IsOneOfControllersSelected(
            this IHtmlHelper htmlHelper,
            IEnumerable<string> controllers,
            string selectedCssClass = "selected"
        )
        {
            var currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;

            return (controllers.Contains(currentController))
                ? selectedCssClass
                : string.Empty;
        }

        public static string IsMyAccountTabSelected(
            this IHtmlHelper htmlHelper,
            string selectedCssClass = "selected"
        )
        {
            var currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;

            return (currentController == "MyAccount" || currentController == "ChangePassword" || currentController == "NotificationPreferences")
                ? selectedCssClass
                : string.Empty;
        }
    }
}
