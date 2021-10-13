namespace DigitalLearningSolutions.Web.Attributes
{
    using System;
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SetApplicationTypeAttribute : Attribute, IActionFilter
    {
        private readonly string? determiningRouteParameter;
        private readonly string? applicationTypeName;

        public SetApplicationTypeAttribute(string? applicationTypeName = null, string? determiningRouteParameter = null)
        {
            this.applicationTypeName = applicationTypeName;
            this.determiningRouteParameter = determiningRouteParameter;
            if (applicationTypeName != null && determiningRouteParameter != null)
            {
                throw new Exception(
                    "Only one argument may be passed to SetApplicationTypeAttribute. " +
                    "Define applicationTypeName if the controller is only used in one application, " +
                    "or define applicationRouteParamName if the controller is used across multiple applications."
                );
            }

            if (applicationTypeName == null && determiningRouteParameter == null)
            {
                throw new Exception(
                    "One argument must be passed to SetApplicationTypeAttribute. " +
                    "Define applicationTypeName if the controller is only used in one application, " +
                    "or define applicationRouteParamName if the controller is used across multiple applications."
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

            if (applicationTypeName != null)
            {
                controller.ViewData["ApplicationType"] = (ApplicationType)applicationTypeName;
            }
            else
            {
                controller.ViewData["ApplicationType"] = context.ActionArguments[determiningRouteParameter!];
            }
        }
    }
}
