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
    public class ValidateAllowedDlsSubApplicationAttribute : Attribute, IActionFilter
    {
        private readonly string applicationArgumentName;

        public ValidateAllowedDlsSubApplicationAttribute(string applicationArgumentName = "dlsSubApplication")
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

            var application = (DlsSubApplication?)
                (context.ActionArguments.ContainsKey(applicationArgumentName)
                    ? context.ActionArguments[applicationArgumentName]
                    : null);

            if (user.IsDelegateOnlyAccount() && !DlsSubApplication.LearningPortal.Equals(application))
            {
                RedirectToLearningPortalVersion(context);
                return;
            }

            if (!user.HasLearningPortalPermissions() && DlsSubApplication.LearningPortal.Equals(application) ||
                !user.HasFrameworksAdminPermissions() && DlsSubApplication.Frameworks.Equals(application) ||
                !user.HasSupervisorAdminPermissions() && DlsSubApplication.Supervisor.Equals(application) ||
                !user.HasCentreAdminPermissions() && DlsSubApplication.TrackingSystem.Equals(application) ||
                !user.HasSuperAdminPermissions() && DlsSubApplication.SuperAdmin.Equals(application))
            {
                RedirectToAccessDenied(context);
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
                [applicationArgumentName] = DlsSubApplication.LearningPortal,
            };
            context.Result = new RedirectToActionResult(descriptor.ActionName, descriptor.ControllerName, routeValues);
        }

        private void RedirectToAccessDenied(ActionExecutingContext context)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "LearningSolutions", new { });
        }
    }
}
