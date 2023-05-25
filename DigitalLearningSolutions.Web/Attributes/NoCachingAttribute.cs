namespace DigitalLearningSolutions.Web.Attributes
{
    using Microsoft.AspNetCore.Mvc.Filters;

    public class NoCachingAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!context.HttpContext.Response.Headers.ContainsKey("Cache-Control"))
            {
                context.HttpContext.Response.Headers.Add("Cache-Control", "no-store, max-age=0");
            }
        }
    }
}
