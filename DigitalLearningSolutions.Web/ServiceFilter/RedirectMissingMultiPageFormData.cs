namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using System;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary>
    ///     Redirects to the Index action of the current controller if there is no TempData Guid or MultiPageFormData of type T
    ///     set.
    ///     This service filter must be registered in <see cref="Startup.ConfigureServices" /> once for each type T it's used
    ///     with.
    /// </summary>
    /// <typeparam name="T">The type required in MultiPageFormData</typeparam>
    public class RedirectMissingMultiPageFormData<T> : IActionFilter // where T : MultiPageFormDataFeature
    {
        private readonly IMultiPageFormDataService multiPageFormDataService;

        public RedirectMissingMultiPageFormData(IMultiPageFormDataService multiPageFormDataService)
        {
            this.multiPageFormDataService = multiPageFormDataService;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is Controller controller)
            {
                var feature = MultiPageFormDataFeature.GetFeatureByType(typeof(T));
                if (feature == null)
                {
                    RedirectToIndex(context, controller);
                    return;
                }

                var tempDataObject = controller.TempData.Peek(feature.TempDataKey);
                if (tempDataObject == null || !(tempDataObject is Guid tempDataGuid))
                {
                    RedirectToIndex(context, controller);
                    return;
                }

                if (!multiPageFormDataService.FormDataExistsForGuidAndFeature(feature, tempDataGuid))
                {
                    RedirectToIndex(context, controller);
                }
            }
        }

        private static void RedirectToIndex(ActionExecutingContext context, Controller controller)
        {
            // ReSharper disable once Mvc.ActionNotResolved
            context.Result = controller.RedirectToAction(
                "Index",
                context.ActionArguments
            );
        }
    }
}
