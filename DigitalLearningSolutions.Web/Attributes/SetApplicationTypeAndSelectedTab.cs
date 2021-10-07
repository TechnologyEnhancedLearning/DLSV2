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
        private readonly ApplicationType applicationTypeName;

        private readonly Tab? tabName;

        public SetApplicationTypeAndSelectedTab(string applicationTypeName, string tabName)
        {
            this.applicationTypeName = applicationTypeName;
            this.tabName = tabName;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller) || tabName == null)
            {
                return;
            }

            controller.ViewData["SelectedTab"] = tabName;

            if (tabName.Equals(Tab.MyAccount))
            {
                controller.ViewData["ApplicationType"] = context.ActionArguments["application"];
            }
            else
            {
                controller.ViewData["ApplicationType"] = applicationTypeName;
            }
        }
    }
}
