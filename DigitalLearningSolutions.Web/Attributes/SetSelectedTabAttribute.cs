namespace DigitalLearningSolutions.Web.Attributes
{
    using System;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SetSelectedTabAttribute : Attribute, IActionFilter
    {
        private readonly string? tabName;

        public SetSelectedTabAttribute(string? tabName)
        {
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
                controller.ViewData[LayoutViewDataKeys.SelectedTab] = (NavMenuTab)tabName;
            }
        }
    }
}
