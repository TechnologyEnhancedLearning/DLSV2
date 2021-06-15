namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using DigitalLearningSolutions.Web.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary>
    /// Redirects to the Index action of the current controller if there is no TempData of type T set.
    /// Note that the temp data should be set using <see cref="TempDataExtension.Set{T}"/> for this to recognise it.
    /// This service filter must be registered in <see cref="Startup.ConfigureServices"/> once for each type T it's used with.
    /// </summary>
    /// <typeparam name="T">The type required in TempData</typeparam>
    public class RedirectEmptySessionData<T>: IActionFilter where T : class
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is Controller controller)
            {
                var userSessionData = controller.TempData.Peek<T>();
                if (userSessionData == null)
                {
                    // ReSharper disable once Mvc.ActionNotResolved
                    context.Result = controller.RedirectToAction("Index");
                }
            }
        }
    }

    /// <summary>
    /// Redirects to the Index action of the current controller unless there is strictly one TempData of either type TOne or TTwo set.
    /// Note that the temp data should be set using <see cref="TempDataExtension.Set{T}"/> for this to recognise it.
    /// This service filter must be registered in <see cref="Startup.ConfigureServices"/> once for each type pair TOne, TTwo it's used with.
    /// </summary>
    /// <typeparam name="TOne">One optional type required in TempData</typeparam>
    /// <typeparam name="TTwo">Second optional type required in TempData</typeparam>
    public class RedirectEmptySessionDataXor<TOne, TTwo> : IActionFilter
        where TOne : class
        where TTwo : class
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is Controller controller)
            {
                var userSessionDataOne = controller.TempData.Peek<TOne>();
                var userSessionDataTwo = controller.TempData.Peek<TTwo>();
                if (!(userSessionDataOne == null ^ userSessionDataTwo == null))
                {
                    // ReSharper disable once Mvc.ActionNotResolved
                    context.Result = controller.RedirectToAction("Index");
                }
            }
        }
    }
}
