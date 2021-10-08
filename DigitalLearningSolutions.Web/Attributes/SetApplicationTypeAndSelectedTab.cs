namespace DigitalLearningSolutions.Web.Attributes
{
    using System;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SetApplicationTypeAndSelectedTab : Attribute, IActionFilter
    {
        private readonly string? applicationTypeName;

        private readonly string? tabName;

        public SetApplicationTypeAndSelectedTab(string? applicationTypeName, string? tabName)
        {
            this.applicationTypeName = applicationTypeName;
            this.tabName = tabName;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            if (tabName != null)
            {
                controller.ViewData["SelectedTab"] = (Tab)tabName;
            }

            if (applicationTypeName == null)
            {
                controller.ViewData["ApplicationType"] = context.ActionArguments["application"];
            }
            else
            {
                controller.ViewData["ApplicationType"] = (ApplicationType)applicationTypeName;
            }
        }
    }
}
