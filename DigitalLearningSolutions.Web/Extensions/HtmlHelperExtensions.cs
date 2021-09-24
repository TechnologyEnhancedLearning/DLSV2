namespace DigitalLearningSolutions.Web.Extensions
{
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
    }
}
