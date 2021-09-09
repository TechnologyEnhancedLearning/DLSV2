namespace DigitalLearningSolutions.Web.Attributes
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateAllowedApplicationTypeAttribute : Attribute, IActionFilter
    {
        private readonly string applicationArgumentName;

        public ValidateAllowedApplicationTypeAttribute(string applicationArgumentName = "application")
        {
            this.applicationArgumentName = applicationArgumentName;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Index", "Login", new { });
                return;
            }

            if (!context.ActionArguments.TryGetValue(applicationArgumentName, out var argumentValue) || !(argumentValue is ApplicationType application))
            {
                return;
            }

            if (user.IsDelegateOnlyAccount() && !ApplicationType.LearningPortal.Equals(application))
            {
                RedirectToLearningPortalVersion(context);
                return;
            }

            if (ApplicationType.LearningPortal.Equals(application) && !user.HasLearningPortalPermissions() ||
                ApplicationType.TrackingSystem.Equals(application) && !user.HasCentreAdminPermissions() ||
                ApplicationType.Frameworks.Equals(application) && !user.HasFrameworksAdminPermissions() ||
                ApplicationType.Main.Equals(application) && !user.HasCentreAdminPermissions()
            )
            {
                context.Result = new RedirectToActionResult("Welcome", "Home", new { });
            }
        }

        private void RedirectToLearningPortalVersion(ActionExecutingContext context)
        {
            var descriptor = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor;
            var routeValues = new Dictionary<string, object>
            {
                [applicationArgumentName] = ApplicationType.LearningPortal
            };
            context.Result = new RedirectToActionResult(descriptor.ActionName, descriptor.ControllerName, routeValues);
        }
    }
}
