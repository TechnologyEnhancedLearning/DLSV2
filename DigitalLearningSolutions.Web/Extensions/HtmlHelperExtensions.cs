namespace DigitalLearningSolutions.Web.Extensions
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    public static class HtmlHelperExtensions
    {
        public static string IsSelected(this IHtmlHelper htmlHelper, string action, string selectedCssClass = "selected")
        {
            var currentAction = htmlHelper.ViewContext.RouteData.Values["action"] as string;

            return action == currentAction ? selectedCssClass : string.Empty;
        }
    }
}
