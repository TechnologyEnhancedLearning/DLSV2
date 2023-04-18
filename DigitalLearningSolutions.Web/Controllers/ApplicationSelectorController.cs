namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.ApplicationSelector;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class ApplicationSelectorController : Controller
    {
        [Authorize(Policy = CustomPolicies.UserAdmin)]
        [RedirectDelegateOnlyToLearningPortal]
        [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
        [SetSelectedTab(nameof(NavMenuTab.SwitchApplication))]
        public IActionResult Index()
        {
            var learningPortalAccess = User.GetCustomClaimAsBool(CustomClaimTypes.LearnUserAuthenticated) ?? false;
            var trackingSystemAccess = User.HasCentreAdminPermissions();
            var contentManagementSystemAccess =
                User.GetCustomClaimAsBool(CustomClaimTypes.UserAuthenticatedCm) ?? false;
            var superviseAccess = User.GetCustomClaimAsBool(CustomClaimTypes.IsSupervisor) | User.GetCustomClaimAsBool(CustomClaimTypes.IsNominatedSupervisor) ?? false;
            var contentCreatorAccess = User.GetCustomClaimAsBool(CustomClaimTypes.UserContentCreator) ?? false;
            var frameworksAccess = User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkDeveloper) |
                User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkContributor) |
                User.GetCustomClaimAsBool(CustomClaimTypes.IsWorkforceManager) |
                User.GetCustomClaimAsBool(CustomClaimTypes.IsWorkforceContributor) ?? false;
            var superAdminAccess = User.HasSuperAdminPermissions();

            var model = new ApplicationSelectorViewModel(
                learningPortalAccess,
                trackingSystemAccess,
                contentManagementSystemAccess,
                superviseAccess,
                contentCreatorAccess,
                frameworksAccess,
                superAdminAccess
            );

            return View(model);
        }
    }
}
