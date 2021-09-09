namespace DigitalLearningSolutions.Web.Attributes
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

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
                RedirectToLogin(context);
                return;
            }

            if (HasModelBindingError(context))
            {
                context.Result = new NotFoundResult();
                return;
            }

            var application = (ApplicationType?)
                (context.ActionArguments.ContainsKey(applicationArgumentName)
                    ? context.ActionArguments[applicationArgumentName]
                    : null);

            if (user.IsDelegateOnlyAccount() && !ApplicationType.LearningPortal.Equals(application))
            {
                RedirectToLearningPortalVersion(context);
                return;
            }

            if (user.HasCentreAdminPermissions() && ApplicationType.Main.Equals(application))
            {
                RedirectToNullVersion(context);
                return;
            }

            if (!user.HasLearningPortalPermissions() && ApplicationType.LearningPortal.Equals(application) ||
                !user.HasFrameworksAdminPermissions() && ApplicationType.Frameworks.Equals(application) ||
                !user.HasCentreAdminPermissions() && (ApplicationType.TrackingSystem.Equals(application) ||
                                                      ApplicationType.Main.Equals(application) ||
                                                      application is null))
            {
                RedirectToHome(context);
            }
        }

        private bool HasModelBindingError(ActionExecutingContext context)
        {
            return context.ModelState.GetValidationState(applicationArgumentName) == ModelValidationState.Invalid;
        }

        private void RedirectToLogin(ActionExecutingContext context)
        {
            context.Result = new RedirectToActionResult("Index", "Login", new { });
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

        private void RedirectToNullVersion(ActionExecutingContext context)
        {
            var descriptor = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor;
            context.Result = new RedirectToActionResult(descriptor.ActionName, descriptor.ControllerName, new { });
        }

        private void RedirectToHome(ActionExecutingContext context)
        {
            context.Result = new RedirectToActionResult("Welcome", "Home", new { });
        }
    }
}
