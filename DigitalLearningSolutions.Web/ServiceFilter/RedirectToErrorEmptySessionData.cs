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
        private readonly MultiPageFormDataFeature _feature;
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
            if (!(context.Controller is Controller controller) || _feature.Name != "AddNewFramework")
            {
                return;
            }

            var tempDataObject = controller.TempData.Peek(_feature.TempDataKey);
            if (tempDataObject == null || !(tempDataObject is Guid tempDataGuid) || !_multiPageFormService.FormDataExistsForGuidAndFeature(_feature, tempDataGuid))
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Gone);
            }
        }
    }
}
