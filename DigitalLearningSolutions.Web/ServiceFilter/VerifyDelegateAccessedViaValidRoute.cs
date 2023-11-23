namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class VerifyDelegateAccessedViaValidRoute : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller))
            {
                return;
            }

            var accessedVia = context.RouteData.Values["accessedVia"].ToString();

            if (!DelegateAccessRoute.CourseDelegates.Name.Equals(accessedVia) &&
                !DelegateAccessRoute.ViewDelegate.Name.Equals(accessedVia))
            {
                context.Result = new NotFoundResult();
            }
        }
    }
}
