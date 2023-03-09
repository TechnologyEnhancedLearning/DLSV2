namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using System;
   // using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Services;
    using GDS.MultiPageFormData;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using GDS.MultiPageFormData.Enums;
    /// <summary>
    ///     Redirects to the Index action of the current controller if there is no
    ///     TempData Guid or MultiPageFormData for the feature set.
    /// </summary>
    public class RedirectMissingMultiPageFormData : IActionFilter
    {
        private readonly MultiPageFormDataFeature feature;
        private readonly IMultiPageFormService multiPageFormService;

        public RedirectMissingMultiPageFormData(
            IMultiPageFormService multiPageFormService,
            string feature
        )
        {
            this.feature = feature;
            this.multiPageFormService = multiPageFormService;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is Controller controller)
            {
                var tempDataObject = controller.TempData.Peek(feature.TempDataKey);
                if (tempDataObject == null || !(tempDataObject is Guid tempDataGuid))
                {
                    RedirectToIndex(context, controller);
                    return;
                }

                if (!multiPageFormService.FormDataExistsForGuidAndFeature(feature, tempDataGuid).GetAwaiter().GetResult())
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
