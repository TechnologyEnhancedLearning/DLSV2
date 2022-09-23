namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using System;
    using System.Net;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary>
    ///     Redirects to 410 error page if there is no
    ///     TempData Guid or MultiPageFormData for the feature set.
    /// </summary>
    public class RedirectToErrorEmptySessionData : IActionFilter
    {
        private readonly MultiPageFormDataFeature feature;
        private readonly IMultiPageFormService multiPageFormService;

        public RedirectToErrorEmptySessionData(
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
                if (tempDataObject == null || !(tempDataObject is Guid tempDataGuid) || !multiPageFormService.FormDataExistsForGuidAndFeature(feature, tempDataGuid))
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.Gone);
                }
            }
        }
    }
}
