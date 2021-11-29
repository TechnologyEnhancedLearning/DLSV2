namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
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

            if (!TryParseDlsSubApplication(context, out var application, out var applicationParameterName))
            {
                return;
            }

            if (await ApplicationIsInaccessibleByPage(context, application))
            {
                return;
            }

            ValidateUserHasPermissionForApplication(context, user, application, applicationParameterName!);
        }

        private bool TryParseDlsSubApplication(
            ActionExecutingContext context,
            out DlsSubApplication? application,
            out string? dlsSubApplicationParameterName
        )
        {
            dlsSubApplicationParameterName = context.ActionDescriptor.Parameters?
                .FirstOrDefault(x => x.ParameterType == typeof(DlsSubApplication))?.Name;

            var actionDoesNotRequireApplication = dlsSubApplicationParameterName == null;
            if (actionDoesNotRequireApplication)
            {
                application = null;
                return false;
            }

            if (HasModelBindingError(context, dlsSubApplicationParameterName!))
            {
                SetNotFoundResult(context);
                application = null;
                return false;
            }

            application = GetDlsSubApplicationFromContext(context, dlsSubApplicationParameterName!);
            return true;
        }

        private async Task<bool> ApplicationIsInaccessibleByPage(
            ActionExecutingContext context,
            DlsSubApplication? application
        )
        {
            if (DlsSubApplication.TrackingSystem.Equals(application) &&
                !await featureManager.IsEnabledAsync(FeatureFlags.RefactoredTrackingSystem) ||
                DlsSubApplication.SuperAdmin.Equals(application) &&
                !await featureManager.IsEnabledAsync(FeatureFlags.RefactoredSuperAdminInterface))
            {
                SetNotFoundResult(context);
                return true;
            }

            if (validApplications.Any() && !validApplications.Contains(application))
            {
                SetNotFoundResult(context);
                return true;
            }

            return false;
        }

        private void ValidateUserHasPermissionForApplication(
            ActionExecutingContext context,
            ClaimsPrincipal user,
            DlsSubApplication? application,
            string dlsSubApplicationParameterName
        )
        {
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

        private void SetLearningPortalVersionRedirectResult(
            ActionExecutingContext context,
            string applicationArgumentName
        )
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
            var parsedArgument = context.ActionArguments.ContainsKey(dlsSubApplicationParameterName)
                ? context.ActionArguments[dlsSubApplicationParameterName]
                : null;
            return (DlsSubApplication?)parsedArgument;
        }
    }
}
