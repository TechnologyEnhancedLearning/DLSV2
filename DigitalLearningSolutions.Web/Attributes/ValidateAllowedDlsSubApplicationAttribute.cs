namespace DigitalLearningSolutions.Web.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Controllers.LearningSolutions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateAllowedDlsSubApplicationAttribute : Attribute, IActionFilter
    {
        private readonly string applicationArgumentName;
        private readonly IEnumerable<DlsSubApplication> validApplications;

        public ValidateAllowedDlsSubApplicationAttribute(
            string[] validApplicationNames,
            string applicationArgumentName = "dlsSubApplication"
        )
        {
            validApplications = validApplicationNames.Select(Enumeration.FromName<DlsSubApplication>);
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
                SetNotFoundResult(context);
                return;
            }

            var application = (DlsSubApplication?)
                (context.ActionArguments.ContainsKey(applicationArgumentName)
                    ? context.ActionArguments[applicationArgumentName]
                    : null);

            if (validApplications.Any() && !validApplications.Contains(application))
            {
                SetNotFoundResult(context);
                return;
            }

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
            context.Result = new RedirectToActionResult(
                nameof(LoginController.Index),
                ControllerHelper.GetControllerAspName(typeof(LoginController)),
                new { }
            );
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
            context.Result = new RedirectToActionResult(
                nameof(LearningSolutionsController.AccessDenied),
                ControllerHelper.GetControllerAspName(typeof(LearningSolutionsController)),
                new { }
            );
        }

        private void SetNotFoundResult(ActionExecutingContext context)
        {
            context.Result = new NotFoundResult();
        }
    }
}
