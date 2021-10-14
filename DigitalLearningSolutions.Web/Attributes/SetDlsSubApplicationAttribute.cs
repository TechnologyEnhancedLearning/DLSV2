namespace DigitalLearningSolutions.Web.Attributes
{
    using System;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SetDlsSubApplicationAttribute : Attribute, IActionFilter
    {
        private readonly string? determiningRouteParameter;
        private readonly string? dlsSubApplicationName;

        public SetDlsSubApplicationAttribute(
            string? dlsSubApplicationName = null,
            string? determiningRouteParameter = null
        )
        {
            this.dlsSubApplicationName = dlsSubApplicationName;
            this.determiningRouteParameter = determiningRouteParameter;
            if (dlsSubApplicationName != null && determiningRouteParameter != null)
            {
                throw new Exception(
                    "Only one argument may be passed to SetDlsSubApplicationAttribute. " +
                    "Define dlsSubApplicationName if the controller is only used in one application, " +
                    "or define determiningRouteParameter if the controller is used across multiple applications."
                );
            }

            if (dlsSubApplicationName == null && determiningRouteParameter == null)
            {
                throw new Exception(
                    "One argument must be passed to SetDlsSubApplicationAttribute. " +
                    "Define dlsSubApplicationName if the controller is only used in one application, " +
                    "or define determiningRouteParameter if the controller is used across multiple applications."
                );
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            if (dlsSubApplicationName != null)
            {
                controller.ViewData[ViewDataHelper.DlsSubApplication] = (DlsSubApplication)dlsSubApplicationName;
            }
            else
            {
                controller.ViewData[ViewDataHelper.DlsSubApplication] =
                    context.ActionArguments[determiningRouteParameter!];
            }
        }
    }
}
