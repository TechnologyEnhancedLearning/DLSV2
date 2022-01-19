namespace DigitalLearningSolutions.Web.Attributes
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary>
    /// Sets a controller/action to be within the specified DlsSubApplication
    /// or, if no DlsSubApplication is specified, attempts to read it from the action parameters
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SetDlsSubApplicationAttribute : Attribute, IActionFilter
    {
        private readonly string? dlsSubApplicationName;

        public SetDlsSubApplicationAttribute(
            string? dlsSubApplicationName = null
        )
        {
            this.dlsSubApplicationName = dlsSubApplicationName;
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
                controller.ViewData[LayoutViewDataKeys.DlsSubApplication] = (DlsSubApplication)dlsSubApplicationName;
            }
            else
            {
                var dlsSubApplicationParameterName = context.ActionDescriptor.Parameters
                    .FirstOrDefault(x => x.ParameterType == typeof(DlsSubApplication))?.Name;
                controller.ViewData[LayoutViewDataKeys.DlsSubApplication] = context.ActionArguments.ContainsKey(dlsSubApplicationParameterName)
                    ? context.ActionArguments[dlsSubApplicationParameterName]
                    : null;
            }
        }
    }
}
