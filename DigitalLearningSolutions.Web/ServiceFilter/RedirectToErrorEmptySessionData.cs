namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using System;
    using System.Net;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Services;
    using GDS.MultiPageFormData;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary>
    ///     Redirects to 410 error page if there is no
    ///     TempData Guid or MultiPageFormData for the feature set.
    /// </summary>
    public class RedirectToErrorEmptySessionData : IActionFilter
    {
        private readonly GDS.MultiPageFormData.Enums.MultiPageFormDataFeature _feature;
        private readonly IMultiPageFormService _multiPageFormService;

        public RedirectToErrorEmptySessionData(
            IMultiPageFormService multiPageFormService,
            string feature
        )
        {
            _feature = feature;
            _multiPageFormService = multiPageFormService;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            if (context.ActionArguments.ContainsKey("actionname") && context.ActionArguments["actionname"].ToString() == "Edit")
            {
                return;
            }

            if (_feature.TempDataKey != string.Empty && (controller.TempData == null || controller.TempData.Peek(_feature.TempDataKey) == null))
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Gone);
            }
            else
            {
                var tempDataKey = controller.TempData.Peek(_feature.TempDataKey);
                var tempDataGuid = Guid.Parse(tempDataKey.ToString()!);

                if (!_multiPageFormService.FormDataExistsForGuidAndFeature(_feature, tempDataGuid).GetAwaiter().GetResult () )
                {
                    return;
                }

                if (controller.TempData == null)
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.Gone);
                }
            }
        }
    }
}
