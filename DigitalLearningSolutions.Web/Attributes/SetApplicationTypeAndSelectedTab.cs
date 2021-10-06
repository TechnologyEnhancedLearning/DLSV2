namespace DigitalLearningSolutions.Web.Attributes
{
    using System;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SetApplicationTypeAndSelectedTab : Attribute, IActionFilter
    {
        public string ApplicationTypeName;

        public string TabName;

        public SetApplicationTypeAndSelectedTab(string applicationTypeName, string tabName)
        {
            this.ApplicationTypeName = applicationTypeName;
            this.TabName = tabName;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // TODO: Set applicationTypeName and tabName in ViewData
            var c = context;
        }
    }
}
