namespace DigitalLearningSolutions.Web.ServiceFilter
{
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
    using Microsoft.FeatureManagement;

    public class ValidateAllowedDlsSubApplication : IActionFilter
    {
        private readonly IFeatureManager featureManager;
        private readonly IEnumerable<DlsSubApplication> validApplications;

        public ValidateAllowedDlsSubApplication(
            IFeatureManager featureManager,
            string[]? validApplicationNames = null
        )
        {
            validApplications = validApplicationNames == null
                ? new DlsSubApplication[] { }
                : validApplicationNames.Select(Enumeration.FromName<DlsSubApplication>);
            this.featureManager = featureManager;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public async void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                SetLoginRedirectResult(context);
                return;
            }

            var dlsSubApplicationParameterName = context.ActionDescriptor.Parameters?
                .FirstOrDefault(x => x.ParameterType == typeof(DlsSubApplication))?.Name;

            if (!string.IsNullOrEmpty(dlsSubApplicationParameterName) &&
                HasModelBindingError(context, dlsSubApplicationParameterName!))
            {
                SetNotFoundResult(context);
                return;
            }

            var application = dlsSubApplicationParameterName == null
                ? null
                : GetDlsSubApplicationFromContext(context, dlsSubApplicationParameterName);

            if (DlsSubApplication.TrackingSystem.Equals(application) &&
                !await featureManager.IsEnabledAsync(FeatureFlags.RefactoredTrackingSystem) ||
                DlsSubApplication.SuperAdmin.Equals(application) &&
                !await featureManager.IsEnabledAsync(FeatureFlags.RefactoredSuperAdminInterface))
            {
                SetNotFoundResult(context);
                return;
            }

            if (validApplications.Any() && !validApplications.Contains(application))
            {
                SetNotFoundResult(context);
                return;
            }

            if (user.IsDelegateOnlyAccount() && DlsSubApplication.Main.Equals(application))
            {
                SetLearningPortalVersionRedirectResult(context, dlsSubApplicationParameterName!);
                return;
            }

            if (!user.HasLearningPortalPermissions() && DlsSubApplication.LearningPortal.Equals(application) ||
                !user.HasFrameworksAdminPermissions() && DlsSubApplication.Frameworks.Equals(application) ||
                !user.HasSupervisorAdminPermissions() && DlsSubApplication.Supervisor.Equals(application) ||
                !user.HasCentreAdminPermissions() && DlsSubApplication.TrackingSystem.Equals(application) ||
                !user.HasSuperAdminPermissions() && DlsSubApplication.SuperAdmin.Equals(application))
            {
                SetAccessDeniedRedirectResult(context);
            }
        }

        private bool HasModelBindingError(ActionExecutingContext context, string applicationArgumentName)
        {
            return context.ModelState.GetValidationState(applicationArgumentName) == ModelValidationState.Invalid;
        }

        private void SetLoginRedirectResult(ActionExecutingContext context)
        {
            context.Result = new RedirectToActionResult(
                nameof(LoginController.Index),
                ControllerHelper.GetControllerNameForAspHttpMethods(typeof(LoginController)),
                new { }
            );
        }

        private void SetLearningPortalVersionRedirectResult(ActionExecutingContext context, string applicationArgumentName)
        {
            var descriptor = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor;
            var routeValues = new Dictionary<string, object>
            {
                [applicationArgumentName] = DlsSubApplication.LearningPortal,
            };
            context.Result = new RedirectToActionResult(descriptor.ActionName, descriptor.ControllerName, routeValues);
        }

        private void SetAccessDeniedRedirectResult(ActionExecutingContext context)
        {
            context.Result = new RedirectToActionResult(
                nameof(LearningSolutionsController.AccessDenied),
                ControllerHelper.GetControllerNameForAspHttpMethods(typeof(LearningSolutionsController)),
                new { }
            );
        }

        private void SetNotFoundResult(ActionExecutingContext context)
        {
            context.Result = new NotFoundResult();
        }

        private DlsSubApplication? GetDlsSubApplicationFromContext(
            ActionExecutingContext context,
            string dlsSubApplicationParameterName
        )
        {
            return (DlsSubApplication?)
                (context.ActionArguments.ContainsKey(dlsSubApplicationParameterName)
                    ? context.ActionArguments[dlsSubApplicationParameterName]
                    : null);
        }
    }
}
