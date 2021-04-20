namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using DigitalLearningSolutions.Web.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class RedirectEmptySessionData<T>: IActionFilter where T : class
    {
        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is Controller controller)
            {
                var userSessionData = controller.TempData.Get<T>();
                if (userSessionData == null)
                {
                    context.Result = controller.RedirectToAction("Index");
                }
            }
        }
    }
}
